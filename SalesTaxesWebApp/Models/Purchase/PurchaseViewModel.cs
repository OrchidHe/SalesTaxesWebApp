using System.Collections.Generic;

namespace SalesTaxesWebApp.Models.Purchase
{
    public class PurchaseViewModel
    {
        public PurchaseViewModel()
        {
            Items = new List<ShoppingCartItemDto>();
        }
        public List<ShoppingCartItemDto> Items { get; set; }
        public decimal TotalTaxes { get; set; }
        public decimal TotalCost { get; set; }
        public bool DoClearModel { get; set; }
    }
}