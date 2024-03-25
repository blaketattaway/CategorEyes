namespace OneCore.CategorEyes.Client.Models
{
    public class Historical
    {
        public long HistoricalId { get; set; }
        public byte LogType { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
