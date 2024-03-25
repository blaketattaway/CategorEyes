using OneCore.CategorEyes.Commons.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Commons.Requests
{
    public class AnalysisRequest
    {
        public string Base64File { get; set; }
        public FileType FileType { get; set; }
        public string FileTypeName { get; set; }
    }
}
