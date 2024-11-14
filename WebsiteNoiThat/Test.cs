using NUnit.Framework;
using Moq;
using System.Web.Mvc;
using WebsiteNoiThat.Areas.Admin.Controllers;
using Models.EF;
using System.Web;
using System.IO;
using System;
using System.Web.Routing;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace WebsiteNoiThat.Tests.Controllers
{
    [TestFixture]
    public class ProductControllerTests
    {
        private Mock<DBNoiThat> mockDb;
        private ProductController controller;
        private Mock<DbSet<Category>> mockCategories;
        private Mock<DbSet<Provider>> mockProviders;
        private Mock<DbSet<Product>> mockProducts;

        [SetUp]
        public void SetUp()
        {
            mockDb = new Mock<DBNoiThat>();
            mockCategories = new Mock<DbSet<Category>>();
            mockProviders = new Mock<DbSet<Provider>>();
            mockProducts = new Mock<DbSet<Product>>();

            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Test Category" }
            }.AsQueryable();

            mockCategories.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
            mockCategories.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);
            mockCategories.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.ElementType);
            mockCategories.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

            var providers = new List<Provider>
            {
                new Provider { ProviderId = 1, Name = "Test Provider" }
            }.AsQueryable();

            mockProviders.As<IQueryable<Provider>>().Setup(m => m.Provider).Returns(providers.Provider);
            mockProviders.As<IQueryable<Provider>>().Setup(m => m.Expression).Returns(providers.Expression);
            mockProviders.As<IQueryable<Provider>>().Setup(m => m.ElementType).Returns(providers.ElementType);
            mockProviders.As<IQueryable<Provider>>().Setup(m => m.GetEnumerator()).Returns(providers.GetEnumerator());

            var products = new List<Product>().AsQueryable();
            mockProducts.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
            mockProducts.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            mockProducts.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockProducts.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

            mockDb.Setup(m => m.Categories).Returns(mockCategories.Object);
            mockDb.Setup(m => m.Providers).Returns(mockProviders.Object);
            mockDb.Setup(m => m.Products).Returns(mockProducts.Object);

            controller = new ProductController();

            var httpContext = new Mock<HttpContextBase>();
            var session = new Mock<HttpSessionStateBase>();
            httpContext.Setup(c => c.Session).Returns(session.Object);
            controller.ControllerContext = new ControllerContext(httpContext.Object, new RouteData(), controller);
        }

        [Test]
        public void TC01_Add_Product_Invalid_Name_ModelState_Returns_View()
        {
            var product = new Product
            {
                Price = 100,
                Quantity = 10,
                CateId = 1,
                ProviderId = 1,
                Description = "Product description"
            };

            // Add model error manually to simulate validation failure
            controller.ModelState.AddModelError("Name", "Tên sản phẩm là bắt buộc");

            var result = controller.Add(product, null) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(string.Empty));
            Assert.That(controller.ModelState.IsValid, Is.False);
            Assert.That(controller.ModelState.ContainsKey("Name"), Is.True);
            Assert.That(controller.ModelState["Name"].Errors[0].ErrorMessage,
                Is.EqualTo("Tên sản phẩm là bắt buộc"));
        }

        [Test]
        public void TC02_Add_Product_Invalid_Price_Returns_Error()
        {
            var product = new Product
            {
                Name = "New Product",
                Price = -1,
                Quantity = 10,
                CateId = 1,
                ProviderId = 1,
                Description = "Product description"
            };
            controller.ModelState.AddModelError("Price", "Giá sản phẩm phải lớn hơn 0.");
            var result = controller.Add(product, null) as ViewResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(string.Empty));
            Assert.That(controller.ModelState.ContainsKey("Price"), Is.True);
            Assert.That(controller.ModelState["Price"].Errors[0].ErrorMessage,
                Is.EqualTo("Giá sản phẩm phải lớn hơn 0."));
        }

        [Test]
        public void TC03_Add_Product_Invalid_Quantity_Returns_Error()
        {
            var product = new Product
            {
                Name = "New Product",
                Price = 100,
                Quantity = -1,
                CateId = 1,
                ProviderId = 1,
                Description = "Product description"
            };

            controller.ModelState.AddModelError("Quantity", "Số lượng phải lớn hơn 0.");
            var result = controller.Add(product, null) as ViewResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(string.Empty));
            Assert.That(controller.ModelState.ContainsKey("Quantity"), Is.True);
            Assert.That(controller.ModelState["Quantity"].Errors[0].ErrorMessage,
                Is.EqualTo("Số lượng phải lớn hơn 0."));
        }

        [Test]
        public void TC04_Add_Product_No_Image_Returns_Error()
        {
            var product = new Product
            {
                Name = "New Product",
                Price = 100,
                Quantity = 10,
                CateId = 1,
                ProviderId = 1,
                Description = "Product description"
            };

            var result = controller.Add(product, null) as ViewResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(string.Empty));
            Assert.That(controller.ModelState.ContainsKey("Photo"), Is.True);
            Assert.That(controller.ModelState["Photo"].Errors[0].ErrorMessage,
                Is.EqualTo("Vui lòng chọn ảnh cho sản phẩm."));
        }

        [Test]
        public void TC05_Add_Product_Invalid_Dates_Returns_Error()
        {
            var product = new Product
            {
                Name = "Product with invalid dates",
                Price = 100,
                Quantity = 10,
                CateId = 1,
                ProviderId = 1,
                Description = "Product description",
                Discount = 10,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(5)
            };

            var mockFile = new Mock<HttpPostedFileBase>();
            mockFile.Setup(f => f.ContentLength).Returns(100);
            mockFile.Setup(f => f.FileName).Returns("product.jpg");
            var stream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 });
            mockFile.Setup(f => f.InputStream).Returns(stream);

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(c => c.Server.MapPath(It.IsAny<string>())).Returns(@"C:\Temp\image\");
            controller.ControllerContext = new ControllerContext(mockHttpContext.Object, new RouteData(), controller);
            var result = controller.Add(product, mockFile.Object) as ViewResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(controller.ModelState.ContainsKey("EndDate"), Is.True);
            Assert.That(controller.ModelState["EndDate"].Errors[0].ErrorMessage,
                Is.EqualTo("Ngày kết thúc phải sau ngày bắt đầu."));
        }

        [Test]
        public void TC06_Add_Product_Valid_ModelState_Adds_Product()
        {
            var product = new Product
            {
                Name = "New Product",
                Price = 100,
                Quantity = 10,
                CateId = 1,
                ProviderId = 1,
                Description = "Product description",
                Discount = 10,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10)
            };

            var mockFile = new Mock<HttpPostedFileBase>();
            mockFile.Setup(f => f.ContentLength).Returns(100);
            mockFile.Setup(f => f.FileName).Returns("product.jpg");

            var stream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 });
            mockFile.Setup(f => f.InputStream).Returns(stream);

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(c => c.Server.MapPath(It.IsAny<string>())).Returns(@"C:\Temp\image\");
            controller.ControllerContext = new ControllerContext(mockHttpContext.Object, new RouteData(), controller);
            mockDb.Setup(db => db.SaveChanges()).Returns(1);
            var result = controller.Add(product, mockFile.Object) as RedirectToRouteResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Show"));
        }

        [TearDown]
        public void TearDown()
        {
            controller.Dispose();
        }
    }
}