namespace OneCore.CategorEyes.Client.Models
{
    public class Invoice
    {
        public string ClientInfo { get; set; }
        public string ProviderInfo { get; set; }
        public string InvoiceNumber { get; set; }
        public string Date { get; set; }
        public List<Product> Products { get; set; }
        public decimal? Total { get; set; }
    }
}
