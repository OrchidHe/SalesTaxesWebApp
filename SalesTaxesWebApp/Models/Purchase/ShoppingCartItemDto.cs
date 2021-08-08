
namespace SalesTaxesWebApp.Models.Purchase
{
    public class ShoppingCartItemDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public decimal ListPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsImported { get; set; }
        public bool IsTaxExempted { get; set; }
        public decimal SalesPrice { get; set; }
    }
}