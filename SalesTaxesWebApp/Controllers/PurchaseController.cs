using SalesTaxesWebApp.Models.Purchase;
using System;
using System.Web.Mvc;

namespace SalesTaxesWebApp.Controllers
{
    public class PurchaseController : Controller
    {
        public PurchaseController()
        {

        }

        [HttpGet]
        public ActionResult Index()
        {
            var viewModel = new PurchaseViewModel();
            return View("Index", viewModel);
        }

        [HttpPost]
        public ActionResult Index(PurchaseViewModel viewModel)
        {
            if (viewModel.DoClearModel)
            {
                var newModel = new PurchaseViewModel();
                ModelState.Clear();
                return View(newModel);
            }
            CalculateCost(viewModel);
            return View("Index", viewModel);
        }

        #region Ajax Methods
        [HttpPost, Route("Purchase/AddItem")]
        public ActionResult AddItem(ShoppingCartItemDto cartItem)
        {
            return PartialView("_ShoppingCartItem", cartItem);
        }
        #endregion Ajax Methods
     
        public void CalculateCost(PurchaseViewModel viewModel)
        {
            var totalSalesTaxes = 0m;
            var totalCost = 0m;
            if (viewModel.Items != null && viewModel.Items.Count > 0)
            {
                foreach (var item in viewModel.Items)
                {
                    var itemSalesTaxes = 0m;
                    if (item.IsImported && item.IsTaxExempted)
                    {
                        itemSalesTaxes = item.ListPrice * 0.05m;
                    }
                    else if (!item.IsImported && !item.IsTaxExempted)
                    {
                        itemSalesTaxes = item.ListPrice * 0.1m;
                    }
                    else if (item.IsImported)
                    {
                        itemSalesTaxes = item.ListPrice * 0.15m;
                    }
                    // Round up to the nearest 5 cents
                    itemSalesTaxes = Math.Ceiling(itemSalesTaxes / 0.05m) * 0.05m;
                    item.SalesPrice = item.ListPrice + itemSalesTaxes;
                    totalSalesTaxes += itemSalesTaxes * item.Quantity;
                    totalCost += item.SalesPrice * item.Quantity;
                }

                viewModel.TotalTaxes = totalSalesTaxes;
                viewModel.TotalCost = totalCost;
            }
        }

    }
}