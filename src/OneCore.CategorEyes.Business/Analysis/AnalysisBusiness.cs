using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Extensions.Logging;
using OneCore.CategorEyes.Business.Persistence;
using OneCore.CategorEyes.Business.Services;
using OneCore.CategorEyes.Commons.Blob;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;
using System.Text;
using System.Text.Json;
using static OneCore.CategorEyes.Commons.Consts.OpenAI;

namespace OneCore.CategorEyes.Business.Analysis
{
    public class AnalysisBusiness : IAnalysisBusiness
    {
        private const string DATA = "data:";
        private const string BASE64 = ";base64,";
        private const string JSON_START = "```json";
        private const string JSON_END = "```";
        private const string TEXT_TO_REPLACE = "#{replaceLine}";
        private readonly IOpenAIService _openAIService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobService _blobService;
        private readonly ILogger<AnalysisBusiness> _logger;

        public AnalysisBusiness(IOpenAIService openAIService, IUnitOfWork unitOfWork, IBlobService blobService, ILogger<AnalysisBusiness> logger)
        {
            _openAIService = openAIService;
            _unitOfWork = unitOfWork;
            _blobService = blobService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<AnalysisResponse> Analyze(AnalysisRequest request)
        {
            try
            {
                ValidateRequest(request);

                string fileName = await UploadFileAndLog(request);

                if (fileName.Equals(string.Empty))
                    throw new Exception("Error uploading file");

                await _unitOfWork.HistoricalRepository.AddAsync(new Historical
                {
                    HistoricalType = (byte)HistoricalType.DocumentUpload,
                    Description = fileName
                });

                var openAIResponse = await GetOpenAIResponse(request);

                var response = ProcessOpenAIResponse(openAIResponse, fileName);

                await SaveAnalysisResultLog(response);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Checks if a given string is a valid Base64 encoded string.
        /// </summary>
        /// <param name="base64">The string to validate, of type <see cref="string"/>.</param>
        /// <returns><c>true</c> if the string is a valid Base64 encoded string; otherwise, <c>false</c>, of type <see cref="bool"/>.</returns>
        private static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
        }

        /// <summary>
        /// Retrieves an analysis response from OpenAI based on the provided request.
        /// </summary>
        /// <param name="request">The analysis request, of type <see cref="AnalysisRequest"/>.</param>
        /// <returns>A task representing the asynchronous operation, returning an <see cref="OpenAIAnalysisResponse"/> with OpenAI's response.</returns>
        private async Task<OpenAIAnalysisResponse> GetOpenAIResponse(AnalysisRequest request)
        {
            var openAIRequest = CreateOpenAIRequest(request);
            var openAIResponse = await _openAIService.Analyze(openAIRequest);

            if (!openAIResponse.choices.Any())
                throw new Exception("No response from OpenAI");

            return openAIResponse;
        }
        /// <summary>
        /// Processes the OpenAI response and generates an analysis response.
        /// </summary>
        /// <param name="openAIResponse">OpenAI's response, of type <see cref="OpenAIAnalysisResponse"/>.</param>
        /// <param name="fileName">The name of the analyzed file, of type <see cref="string"/>.</param>
        /// <returns>The processed analysis response, of type <see cref="AnalysisResponse"/>.</returns>
        private static AnalysisResponse ProcessOpenAIResponse(OpenAIAnalysisResponse openAIResponse, string fileName)
        {
            var responseContent = RemoveInvalidChars(openAIResponse.choices.FirstOrDefault()!.message.content);
            var response = ParseTo<AnalysisResponse>(responseContent);

            if (Enum.TryParse(response.DocumentTypeName, out DocumentType documentType))
                response.DocumentType = documentType;

            response.FileName = fileName;

            return response;
        }

        /// <summary>
        /// Saves a log of the analysis result in the historical repository.
        /// </summary>
        /// <param name="response">The analysis response to log, of type <see cref="AnalysisResponse"/>.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SaveAnalysisResultLog(AnalysisResponse response)
        {
            await _unitOfWork.HistoricalRepository.AddAsync(new Historical
            {
                HistoricalType = (byte)HistoricalType.IA,
                Description = JsonSerializer.Serialize(response)
            });

            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Reads the text content from a base64-encoded PDF.
        /// </summary>
        /// <param name="base64String">The base64 string representing the PDF content, of type <see cref="string"/>.</param>
        /// <returns>The extracted text from the PDF, of type <see cref="string"/>.</returns>
        private static string ReadPdf(string base64String)
        {
            byte[] pdfBytes = Convert.FromBase64String(base64String);
            var content = new MemoryStream(pdfBytes);
            var reader = new PdfReader(content);
            var pdfDocument = new PdfDocument(reader);
            var strategy = new LocationTextExtractionStrategy();
            StringBuilder processed = new StringBuilder();

            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
            {
                var page = pdfDocument.GetPage(i);
                string text = PdfTextExtractor.GetTextFromPage(page, strategy);
                processed.Append(text);
            }

            if (string.IsNullOrEmpty(processed.ToString()))
                throw new Exception("The pdf has no text to send");

            return processed.ToString();
        }

        /// <summary>
        /// Deserializes a JSON string into an object of the specified type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize, of type <see cref="string"/>.</param>
        /// <returns>The deserialized object of the specified type.</returns>
        private static T ParseTo<T>(string json)
            => JsonSerializer.Deserialize<T>(json)!;

        /// <summary>
        /// Removes invalid characters from a JSON string.
        /// </summary>
        /// <param name="json">The JSON string from which to remove invalid characters, of type <see cref="string"/>.</param>
        /// <returns>The cleaned JSON string, of type <see cref="string"/>.</returns>
        private static string RemoveInvalidChars(string json)
            => json.Replace(JSON_START, string.Empty).Replace(JSON_END, string.Empty);

        /// <summary>
        /// Creates a request for the OpenAI service based on the provided analysis request.
        /// </summary>
        /// <param name="request">The analysis request, of type <see cref="AnalysisRequest"/>.</param>
        /// <returns>The formatted request for OpenAI, of type <see cref="object"/> due to the dynamic structure expected by the OpenAI service.</returns>
        private static object CreateOpenAIRequest(AnalysisRequest request)
        {
            return request.FileType switch
            {
                FileType.Pdf => CreatePdfRequest(request),
                FileType.Image => CreateImageRequest(request),
                _ => throw new ArgumentException("Unknown file type"),
            };
        }

        /// <summary>
        /// Creates a specific request for PDF document analysis to be sent to the OpenAI service.
        /// </summary>
        /// <param name="request">The analysis request containing the base64-encoded PDF document, of type <see cref="AnalysisRequest"/>.</param>
        /// <returns>A request configured for PDF analysis, ready to be sent to OpenAI, of type <see cref="object"/> due to the dynamic structure expected by the OpenAI service.</returns>
        private static object CreatePdfRequest(AnalysisRequest request)
        {
            var content = new List<object>
            {
                new { type = MessageContentType.TEXT, text = AIInstructions.INSTRUCTIONS.Replace(TEXT_TO_REPLACE, ReplacingLines.PDF_REPLACELINE) },
                new { type = MessageContentType.TEXT, text = ReadPdf(request.Base64File) }
            };

            return CreateBaseRequest(content);
        }

        /// <summary>
        /// Creates a specific request for image analysis to be sent to the OpenAI service.
        /// </summary>
        /// <param name="request">The analysis request containing the base64-encoded image, of type <see cref="AnalysisRequest"/>.</param>
        /// <returns>A request configured for image analysis, ready to be sent to OpenAI, of type <see cref="object"/> due to the dynamic structure expected by the OpenAI service.</returns>
        private static object CreateImageRequest(AnalysisRequest request)
        {
            var content = new List<object>
        {
            new { type = MessageContentType.TEXT, text = AIInstructions.INSTRUCTIONS.Replace(TEXT_TO_REPLACE, ReplacingLines.IMAGE_REPLACELINE) },
            new { type = MessageContentType.IMAGE_URL, image_url = $"{ DATA }{ request.FileTypeName }{ BASE64 }{ request.Base64File }" }
        };

            return CreateBaseRequest(content);
        }

        /// <summary>
        /// Creates the base structure of a request for the OpenAI service, incorporating the specific content for analysis.
        /// </summary>
        /// <param name="content">The specific content of the analysis, whether for a PDF document or an image, represented as a list of <see cref="object"/> following the structure expected by the OpenAI service.</param>
        /// <returns>The base request configured with the specific content for analysis, of type <see cref="object"/> due to the dynamic structure expected by the OpenAI service.</returns>
        private static object CreateBaseRequest(List<object> content)
        {
            return new
            {
                model = OpenAIModels.GPT4_VISION_PREVIEW,
                messages = new List<object> { new { role = Roles.USER, content = content } },
                max_tokens = 1000
            };
        }

        /// <summary>
        /// Validates the analysis request to ensure the file type is supported.
        /// </summary>
        /// <param name="request">The analysis request to validate, of type <see cref="AnalysisRequest"/>.</param>
        private static void ValidateRequest(AnalysisRequest request)
        {
            if (request.FileType == FileType.Unknown)
                throw new ArgumentException("Unknown file type");
            if (string.IsNullOrEmpty(request.Base64File.Trim()))
                throw new ArgumentException("Base64File cannot be null");
            if (!IsBase64String(request.Base64File))
                throw new ArgumentException("Invalid base64 string");
        }

        /// <summary>
        /// Uploads the file provided in the request to the cloud storage service and logs the event.
        /// </summary>
        /// <param name="request">The request containing the file to upload, of type <see cref="AnalysisRequest"/>.</param>
        /// <returns>A task representing the asynchronous operation and returning the uploaded file's name, of type <see cref="string"/>.</returns>
        private async Task<string> UploadFileAndLog(AnalysisRequest request)
        {
            try
            {
                return await _blobService.UploadFile(new FileUpload
                {
                    Base64File = request.Base64File,
                    ContentType = request.FileTypeName,
                    Extension = request.FileTypeName.Split('/')[1]
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return string.Empty;
            }
        }
    }
}
