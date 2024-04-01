using OneCore.CategorEyes.Commons.Entities.Common;

namespace OneCore.CategorEyes.Commons.Entities
{
    public class Historical : BaseEntity
    {
        public byte HistoricalType { get; set; }
        public string Description { get; set; }
    }
}
