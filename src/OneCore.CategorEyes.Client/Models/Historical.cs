using static OneCore.CategorEyes.Client.Models.Consts;

namespace OneCore.CategorEyes.Client.Models
{
    public class Historical
    {
        public long Id { get; set; }
        public byte HistoricalType { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public string HistoricalTypeName { get; set; }
    }
}
