﻿using OneCore.CategorEyes.Commons.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Commons.Responses
{
    public class AnalysisResponse
    {
        public string DocumentTypeName { get; set; }
        public DocumentType DocumentType { get; set; }
        public string Data { get; set; }
        public string AdditionalData { get; set; }
    }
}
