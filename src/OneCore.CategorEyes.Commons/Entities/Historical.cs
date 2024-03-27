using OneCore.CategorEyes.Commons.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Commons.Entities
{
    public class Historical : BaseEntity
    {
        public byte HistoricalType { get; set; }
        public string Description { get; set; }
    }
}
