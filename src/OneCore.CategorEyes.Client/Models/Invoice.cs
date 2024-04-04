namespace OneCore.CategorEyes.Client.Models
{
    public class Invoice
    {
        public string? ClientName { get; set; }
        public string? ClientAddress { get; set; }
        public string? ProviderName { get; set; }
        public string? ProviderAddress { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? Date { get; set; }
        public List<Product> Products { get; set; }
        public decimal? Total { get; set; }
    }
}
