using OneCore.CategorEyes.Commons.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Commons.Requests
{
    public class OpenAIAnalysisRequest
    {
        public string model { get; set; }
        public List<Message> messages { get; set; }
        public int max_tokens { get; set; } = 1000;
    }
}
