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
    internal class AnalysisBusiness : IAnalysisBusiness
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

        public async Task<AnalysisResponse> Analyze(AnalysisRequest request)
        {
            try
            {
                string fileName = await UploadFileAndLog(request);

                await _unitOfWork.HistoricalRepository.AddAsync(new Historical
                {
                    HistoricalType = (byte)HistoricalType.DocumentUpload,
                    Description = fileName
                });

                ValidateRequest(request);

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
        /// Obtiene una respuesta de análisis de OpenAI basada en la solicitud proporcionada.
        /// </summary>
        /// <param name="request">La solicitud de análisis, de tipo <see cref="AnalysisRequest"/>.</param>
        /// <returns>Una tarea que representa la operación asincrónica y retorna un <see cref="OpenAIAnalysisResponse"/> con la respuesta de OpenAI.</returns>
        private async Task<OpenAIAnalysisResponse> GetOpenAIResponse(AnalysisRequest request)
        {
            var openAIRequest = CreateOpenAIRequest(request);
            var openAIResponse = await _openAIService.Analyze(openAIRequest);

            if (!openAIResponse.choices.Any())
                throw new Exception("No response from OpenAI");

            return openAIResponse;
        }

        /// <summary>
        /// Procesa la respuesta de OpenAI y genera una respuesta de análisis.
        /// </summary>
        /// <param name="openAIResponse">La respuesta de OpenAI, de tipo <see cref="OpenAIAnalysisResponse"/>.</param>
        /// <param name="fileName">El nombre del archivo analizado, de tipo <see cref="string"/>.</param>
        /// <returns>La respuesta de análisis procesada, de tipo <see cref="AnalysisResponse"/>.</returns>
        private AnalysisResponse ProcessOpenAIResponse(OpenAIAnalysisResponse openAIResponse, string fileName)
        {
            var responseContent = RemoveInvalidChars(openAIResponse.choices.FirstOrDefault()!.message.content);
            var response = ParseTo<AnalysisResponse>(responseContent);

            if (Enum.TryParse(response.DocumentTypeName, out DocumentType documentType))
                response.DocumentType = documentType;

            response.FileName = fileName;

            return response;
        }

        /// <summary>
        /// Guarda un registro del resultado del análisis en el repositorio histórico.
        /// </summary>
        /// <param name="response">La respuesta de análisis a registrar, de tipo <see cref="AnalysisResponse"/>.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
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
        /// Lee el contenido de texto de un PDF codificado en base64.
        /// </summary>
        /// <param name="base64String">La cadena en base64 que representa el contenido del PDF, de tipo <see cref="string"/>.</param>
        /// <returns>El texto extraído del PDF, de tipo <see cref="string"/>.</returns>
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
                throw new Exception("There's no text to send");

            return processed.ToString();
        }

        /// <summary>
        /// Deserializa un JSON a un objeto del tipo especificado.
        /// </summary>
        /// <param name="json">La cadena JSON a deserializar, de tipo <see cref="string"/>.</param>
        /// <returns>El objeto deserializado del tipo especificado.</returns>
        private static T ParseTo<T>(string json)
            => JsonSerializer.Deserialize<T>(json)!;

        /// <summary>
        /// Elimina caracteres inválidos de una cadena JSON.
        /// </summary>
        /// <param name="json">La cadena JSON de la que eliminar los caracteres inválidos, de tipo <see cref="string"/>.</param>
        /// <returns>La cadena JSON limpia, de tipo <see cref="string"/>.</returns>
        private static string RemoveInvalidChars(string json)
            => json.Replace(JSON_START, string.Empty).Replace(JSON_END, string.Empty);

        /// <summary>
        /// Crea una solicitud para el servicio de OpenAI basada en la solicitud de análisis proporcionada.
        /// </summary>
        /// <param name="request">La solicitud de análisis, de tipo <see cref="AnalysisRequest"/>.</param>
        /// <returns>La solicitud formateada para OpenAI, de tipo <see cref="object"/> debido a la estructura dinámica esperada por el servicio de OpenAI.</returns>
        private object CreateOpenAIRequest(AnalysisRequest request)
        {
            return request.FileType switch
            {
                FileType.Pdf => CreatePdfRequest(request),
                FileType.Image => CreateImageRequest(request),
                _ => throw new ArgumentException("Unknown file type"),
            };
        }

        /// <summary>
        /// Crea una solicitud específica para análisis de documentos PDF para ser enviada al servicio de OpenAI.
        /// </summary>
        /// <param name="request">La solicitud de análisis que contiene el documento PDF codificado en Base64, de tipo <see cref="AnalysisRequest"/>.</param>
        /// <returns>Una solicitud configurada para el análisis de PDFs, lista para ser enviada a OpenAI, de tipo <see cref="object"/> debido a la estructura dinámica esperada por el servicio de OpenAI.</returns>
        private object CreatePdfRequest(AnalysisRequest request)
        {
            var content = new List<object>
            {
                new { type = MessageContentType.TEXT, text = AIInstructions.INSTRUCTIONS.Replace(TEXT_TO_REPLACE, ReplacingLines.PDF_REPLACELINE) },
                new { type = MessageContentType.TEXT, text = ReadPdf(request.Base64File) }
            };

            return CreateBaseRequest(content);
        }

        /// <summary>
        /// Crea una solicitud específica para análisis de imágenes para ser enviada al servicio de OpenAI.
        /// </summary>
        /// <param name="request">La solicitud de análisis que contiene la imagen codificada en Base64, de tipo <see cref="AnalysisRequest"/>.</param>
        /// <returns>Una solicitud configurada para el análisis de imágenes, lista para ser enviada a OpenAI, de tipo <see cref="object"/> debido a la estructura dinámica esperada por el servicio de OpenAI.</returns>
        private object CreateImageRequest(AnalysisRequest request)
        {
            var content = new List<object>
        {
            new { type = MessageContentType.TEXT, text = AIInstructions.INSTRUCTIONS.Replace(TEXT_TO_REPLACE, ReplacingLines.IMAGE_REPLACELINE) },
            new { type = MessageContentType.IMAGE_URL, image_url = $"{ DATA }{ request.FileTypeName }{ BASE64 }{ request.Base64File }" }
        };

            return CreateBaseRequest(content);
        }

        /// <summary>
        /// Crea la estructura base de una solicitud para el servicio de OpenAI, incorporando el contenido específico para el análisis.
        /// </summary>
        /// <param name="content">El contenido específico del análisis, ya sea de un documento PDF o una imagen, representado como una lista de objetos <see cref="object"/> que siguen la estructura esperada por el servicio de OpenAI.</param>
        /// <returns>La solicitud base configurada con el contenido específico para el análisis, de tipo <see cref="object"/> debido a la estructura dinámica esperada por el servicio de OpenAI.</returns>
        private object CreateBaseRequest(List<object> content)
        {
            return new
            {
                model = OpenAIModels.GPT4_VISION_PREVIEW,
                messages = new List<object> { new { role = Roles.USER, content = content } },
                max_tokens = 1000
            };
        }

        /// <summary>
        /// Valida la solicitud de análisis para asegurar que el tipo de archivo sea soportado.
        /// </summary>
        /// <param name="request">La solicitud de análisis a validar, de tipo <see cref="AnalysisRequest"/>.</param>
        private void ValidateRequest(AnalysisRequest request)
        {
            if (request.FileType == FileType.Unknown)
                throw new ArgumentException("Unknown file type");
        }

        /// <summary>
        /// Sube el archivo proporcionado en la solicitud al servicio de almacenamiento en la nube y registra el evento.
        /// </summary>
        /// <param name="request">La solicitud que contiene el archivo a subir, de tipo <see cref="AnalysisRequest"/>.</param>
        /// <returns>Una tarea que representa la operación asincrónica y retorna el nombre del archivo subido, de tipo <see cref="string"/>.</returns>
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
