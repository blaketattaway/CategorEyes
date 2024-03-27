using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Commons.Requests
{
    public class LogRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public string? Filter { get; set; }
    }
}
