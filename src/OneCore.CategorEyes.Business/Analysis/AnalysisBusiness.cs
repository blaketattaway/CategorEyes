using itext.pdfimage.Extensions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using OneCore.CategorEyes.Business.Persistence;
using OneCore.CategorEyes.Business.Services;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Responses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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

        public AnalysisBusiness(IOpenAIService openAIService, IUnitOfWork unitOfWork)
        {
            _openAIService = openAIService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AnalysisResponse> Analyze(AnalysisRequest request)
        {
            await _unitOfWork.HistoricalRepository.AddAsync(new Historical
            {
                HistoricalType = (byte)HistoricalType.DocumentUpload,
                Data = JsonSerializer.Serialize(request)
            });

            if (request.FileType == FileType.Unknown)
                throw new ArgumentException("Unknown file type");

            var openAIResponse = await _openAIService.Analyze(CreateOpenAIRequest(request));

            if (!openAIResponse.choices.Any())
                throw new Exception("No response from OpenAI");

            AnalysisResponse response = ParseTo<AnalysisResponse>(RemoveInvalidChars(openAIResponse.choices.FirstOrDefault()!.message.content));

            if (Enum.TryParse(response.DocumentTypeName, out DocumentType documentType))
                response.DocumentType = documentType;

            await _unitOfWork.HistoricalRepository.AddAsync(new Historical
            {
                HistoricalType = (byte)HistoricalType.IA,
                Data = JsonSerializer.Serialize(response)
            });

            await _unitOfWork.CompleteAsync();

            return response;
        }

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

            return processed.ToString();
        }

        private static T ParseTo<T>(string json)
            => JsonSerializer.Deserialize<T>(json)!;

        private static string RemoveInvalidChars(string json)
            => json.Replace(JSON_START, string.Empty).Replace(JSON_END, string.Empty);

        private object CreateOpenAIRequest(AnalysisRequest request)
        {
            return request.FileType switch
            {
                FileType.Pdf => CreatePdfRequest(request),
                FileType.Image => CreateImageRequest(request),
                _ => throw new ArgumentException("Unknown file type"),
            };
        }

        private object CreatePdfRequest(AnalysisRequest request)
        {
            var content = new List<object>
        {
            new { type = MessageContentType.TEXT, text = AIInstructions.INSTRUCTIONS.Replace(TEXT_TO_REPLACE, ReplacingLines.PDF_REPLACELINE) },
            new { type = MessageContentType.TEXT, text = ReadPdf(request.Base64File) }
        };

            return CreateBaseRequest(content);
        }

        private object CreateImageRequest(AnalysisRequest request)
        {
            var content = new List<object>
        {
            new { type = MessageContentType.TEXT, text = AIInstructions.INSTRUCTIONS.Replace(TEXT_TO_REPLACE, ReplacingLines.IMAGE_REPLACELINE) },
            new { type = MessageContentType.IMAGE_URL, image_url = $"{ DATA }{ request.FileTypeName }{ BASE64 }{ request.Base64File }" }
        };

            return CreateBaseRequest(content);
        }

        private object CreateBaseRequest(List<object> content)
        {
            return new
            {
                model = OpenAIModels.GPT4_VISION_PREVIEW,
                messages = new List<object> { new { role = Roles.USER, content = content } },
                max_tokens = 1000
            };
        }
    }
}
