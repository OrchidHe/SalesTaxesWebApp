using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using SalesTaxesWebApp.Controllers;
using SalesTaxesWebApp.Models.Purchase;

namespace SalesTaxesWebApp.Tests.Controllers
{
    [TestFixture]
    public class PurchaseControllerTest
    {
        [SetUp]
        public virtual void SetUp()
        {
        }

        [Test]
        public void TestItemNotImportedAndNotTaxExempted()
        {
            var shoppingCartItem = new ShoppingCartItemDto()
            {
                ItemName = "testObj1",
                ListPrice = 15.5m,
                Quantity = 1,
                IsImported = false,
                IsTaxExempted = false
            };
            var viewModel = new PurchaseViewModel()
            {
                Items = new List<ShoppingCartItemDto>() { shoppingCartItem }
            };

            var totalsalesTaxes = Math.Ceiling((shoppingCartItem.ListPrice * 0.1m) / 0.05m) * 0.05m * shoppingCartItem.Quantity;

            var controller = new PurchaseController();

            var result = controller.Index(viewModel) as ViewResult;
            var model = result.ViewData.Model as PurchaseViewModel;

            // Assert
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.AreEqual(totalsalesTaxes, model.TotalTaxes);          
        }

        [Test]
        public void TestItemImportedButNotTaxExempted()
        {
            var shoppingCartItem = new ShoppingCartItemDto()
            {
                ItemName = "testObj2",
                ListPrice = 15.5m,
                Quantity = 1,
                IsImported = true,
                IsTaxExempted = false
            };
            var viewModel = new PurchaseViewModel()
            {
                Items = new List<ShoppingCartItemDto>() { shoppingCartItem }
            };

            var totalsalesTaxes = Math.Ceiling((shoppingCartItem.ListPrice * 0.15m) / 0.05m) * 0.05m * shoppingCartItem.Quantity;

            var controller = new PurchaseController();

            var result = controller.Index(viewModel) as ViewResult;
            var model = result.ViewData.Model as PurchaseViewModel;

            // Assert
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.AreEqual(totalsalesTaxes, model.TotalTaxes);
        }

        [Test]
        public void TestItemImportedAndTaxExempted()
        {
            var shoppingCartItem = new ShoppingCartItemDto()
            {
                ItemName = "testObj3",
                ListPrice = 15.5m,
                Quantity = 1,
                IsImported = true,
                IsTaxExempted = true
            };
            var viewModel = new PurchaseViewModel()
            {
                Items = new List<ShoppingCartItemDto>() { shoppingCartItem }
            };

            var totalsalesTaxes = Math.Ceiling((shoppingCartItem.ListPrice * 0.05m) / 0.05m) * 0.05m * shoppingCartItem.Quantity;

            var controller = new PurchaseController();

            var result = controller.Index(viewModel) as ViewResult;
            var model = result.ViewData.Model as PurchaseViewModel;

            // Assert
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.AreEqual(totalsalesTaxes, model.TotalTaxes);
        }

        [Test]
        public void TestItemNotImportedButTaxExempted()
        {
            var shoppingCartItem = new ShoppingCartItemDto()
            {
                ItemName = "testObj4",
                ListPrice = 15.5m,
                Quantity = 1,
                IsImported = false,
                IsTaxExempted = true
            };
            var viewModel = new PurchaseViewModel()
            {
                Items = new List<ShoppingCartItemDto>() { shoppingCartItem }
            };

            var totalsalesTaxes = 0.0m;

            var controller = new PurchaseController();

            var result = controller.Index(viewModel) as ViewResult;
            var model = result.ViewData.Model as PurchaseViewModel;

            // Assert
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.AreEqual(totalsalesTaxes, model.TotalTaxes);
        }

        [Test]
        public void TestItemsWithMixedCases()
        {
            // Not imported and not tax exempted
            var shoppingCartItem1 = new ShoppingCartItemDto()
            {
                ItemName = "testObj5",
                ListPrice = 15.5m,
                Quantity = 1,
                IsImported = false,
                IsTaxExempted = false
            };
            // is imported and tax exempted
            var shoppingCartItem2 = new ShoppingCartItemDto()
            {
                ItemName = "testObj6",
                ListPrice = 15.5m,
                Quantity = 1,
                IsImported = true,
                IsTaxExempted = true
            };

            var viewModel = new PurchaseViewModel()
            {
                Items = new List<ShoppingCartItemDto>() { shoppingCartItem1, shoppingCartItem2 }
            };

            var totalsalesTaxes = Math.Ceiling((viewModel.Items[0].ListPrice * 0.1m) / 0.05m) * 0.05m * viewModel.Items[0].Quantity + Math.Ceiling((viewModel.Items[1].ListPrice * 0.05m) / 0.05m) * 0.05m * viewModel.Items[1].Quantity;

            var controller = new PurchaseController();

            var result = controller.Index(viewModel) as ViewResult;
            var model = result.ViewData.Model as PurchaseViewModel;

            // Assert
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.AreEqual(totalsalesTaxes, model.TotalTaxes);
        }

    }
}
