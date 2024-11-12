using NUnit.Framework;
using Moq;
using System.Web;
using System.Web.Mvc;
using WebsiteNoiThat.Areas.Admin.Controllers;
using WebsiteNoiThat.Areas.Admin.Models;
using Models.DAO;
using Models.EF;
using WebsiteNoiThat.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace WebsiteNoiThat.Tests.Controllers
{
    [TestFixture]
    public class ProductControllerTests
    {
        // Khai báo các đối tượng mock
        private Mock<HttpSessionStateBase> mockSession;
        private Mock<HttpContextBase> mockContext;
        private ProductController controller;
        private Mock<UserDao> mockUserDao;

        [SetUp]
        public void Setup()
        {
            // Khởi tạo các mock objects
            mockSession = new Mock<HttpSessionStateBase>();
            mockContext = new Mock<HttpContextBase>();
            mockUserDao = new Mock<UserDao>();

            // Đảm bảo rằng session sẽ được trả về đúng trong context
            mockContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);

            // Tạo controller mới và set ControllerContext
            controller = new ProductController
            {
                ControllerContext = new ControllerContext(mockContext.Object, new System.Web.Routing.RouteData(), controller)
            };
        }

        [Test]
        public void AddProduct_ShouldRedirect_WhenUserIsNotLoggedIn()
        {
            // Không thiết lập session đăng nhập (giả sử người dùng chưa đăng nhập)
            mockSession.Setup(sess => sess["user_sesion_admin"]).Returns(null);

            // Gọi phương thức Add trên controller
            var result = controller.Add(new Product(), null) as RedirectToRouteResult;

            // Kiểm tra kết quả trả về có phải là RedirectToRouteResult và chuyển hướng đến trang Login không
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Product"));
        }

       
    }
}
