using Models.DAO;
using Models.EF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteNoiThat.Common;
using WebsiteNoiThat.Models;
using System.Diagnostics;

namespace WebsiteNoiThat.Areas.Admin.Controllers
{
    public class ProductController : HomeController
    {
        DBNoiThat db = new DBNoiThat();

        [HasCredential(RoleId = "VIEW_PRODUCT")]
        public ActionResult Show()
        {
            var session = (UserLogin)Session[WebsiteNoiThat.Common.Commoncontent.user_sesion_admin];
            ViewBag.username = session.Username;

            var productViewModels = (from a in db.Products
                                     join b in db.Providers on a.ProviderId equals b.ProviderId
                                     join c in db.Categories on a.CateId equals c.CategoryId
                                     select new ProductViewModel
                                     {
                                         ProductId = a.ProductId,
                                         Name = a.Name,
                                         Description = a.Description,
                                         Discount = a.Discount,
                                         ProviderName = b.Name,
                                         CateName = c.Name,
                                         Price = a.Price,
                                         Quantity = a.Quantity,
                                         StartDate = a.StartDate,
                                         EndDate = a.EndDate,
                                         Photo = a.Photo,
                                     }).ToList();

            return View(productViewModels);
        }

        [HttpGet]
        [HasCredential(RoleId = "ADD_PRODUCT")]
        public ActionResult Add()
        {
            var session = (UserLogin)Session[WebsiteNoiThat.Common.Commoncontent.user_sesion_admin];
            ViewBag.username = session.Username;
            ViewBag.ListCate = new SelectList(db.Categories.ToList(), "CategoryId", "Name");
            ViewBag.ListProvider = new SelectList(db.Providers.ToList(), "ProviderId", "Name");
            var model = new Product()
            {
                Discount = 0,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        [HasCredential(RoleId = "ADD_PRODUCT")]
        public ActionResult Add(Product n, HttpPostedFileBase UploadImage)
        {
            var session = (UserLogin)Session[WebsiteNoiThat.Common.Commoncontent.user_sesion_admin];
            ViewBag.username = session.Username;
            ViewBag.ListCate = new SelectList(db.Categories.ToList(), "CategoryId", "Name");
            ViewBag.ListProvider = new SelectList(db.Providers.ToList(), "ProviderId", "Name");

            //var models = db.Products.SingleOrDefault(a => a.ProductId == n.ProductId);
            //if (models != null)
            //{
            //    ModelState.AddModelError("ProductError", "Mã sản phẩm đã tồn tại!");
            //    return View();
            //}
            //else
            //{
            ModelState.Remove("Photo");
            if (ModelState.IsValid)
            {

                if (UploadImage != null && UploadImage.ContentLength > 0)
                {
                    var root = Server.MapPath("~/image/");
                    var path = Path.Combine(root, UploadImage.FileName);
                    UploadImage.SaveAs(path);
                    n.Photo = UploadImage.FileName;
                }
                else
                {
                    ModelState.AddModelError("Photo", "Vui lòng chọn ảnh cho sản phẩm.");
                    return View();
                }
                var model = new Product();
                model.Name = n.Name;
                model.Photo = n.Photo;
                model.Price = n.Price;
                model.Quantity = n.Quantity;
                model.CateId = n.CateId;
                model.ProviderId = n.ProviderId;
                if (n.Description == null || n.Description == string.Empty)
                {
                    model.Description = "Không có mô tả gì";
                }
                else
                { model.Description = n.Description; }

                if (n.Discount == null || n.Discount == 0)
                {
                    model.Discount = 0;
                    model.StartDate = DateTime.MinValue;
                    model.EndDate = DateTime.MinValue;
                }
                else
                {
                    model.Discount = n.Discount;
                    model.StartDate = n.StartDate;
                    model.EndDate = n.EndDate;
                }
              
                db.Products.Add(model);
                db.SaveChanges();
            }
            else
            {

                return View();
            }
            return RedirectToAction("Show");
        }

        [HttpGet]
        [HasCredential(RoleId = "EDIT_PRODUCT")]
        public ActionResult Edit(int ProductId)
        {
            var session = (UserLogin)Session[WebsiteNoiThat.Common.Commoncontent.user_sesion_admin];
            ViewBag.username = session.Username;

            var model = (from a in db.Products
                         join b in db.Providers on a.ProviderId equals b.ProviderId
                         join c in db.Categories on a.CateId equals c.CategoryId
                         where a.ProductId == ProductId
                         select new ProductViewModel
                         {
                             ProductId = a.ProductId,
                             Name = a.Name,
                             Description = a.Description,
                             Discount = a.Discount,
                             ProviderName = b.Name,
                             CateName = c.Name,
                             Price = a.Price,
                             Quantity = a.Quantity,
                             StartDate = a.StartDate,
                             EndDate = a.EndDate,
                             Photo = a.Photo,
                             CateId = a.CateId
                         }).ToList();

            ViewBag.ListCate = new SelectList(db.Categories.ToList(), "CategoryId", "Name");
            ViewBag.ListProvider = new SelectList(db.Providers.ToList(), "ProviderId", "Name");
            var models = model.Where(n => n.ProductId == ProductId).First();
            return View(models);
        }

        [HttpPost]
        [HasCredential(RoleId = "EDIT_PRODUCT")]
        public ActionResult Edit(ProductViewModel n, HttpPostedFileBase UploadImage)

        {
            var session = (UserLogin)Session[WebsiteNoiThat.Common.Commoncontent.user_sesion_admin];
            ViewBag.username = session.Username;

            ViewBag.ListCate = new SelectList(db.Categories.ToList(), "CategoryId", "Name");
            ViewBag.ListProvider = new SelectList(db.Providers.ToList(), "ProviderId", "Name");
            if (ModelState.IsValid)
            {
                ProductDao a = new ProductDao();
                if (UploadImage != null)
                {
                    // Delete exiting file
                    //System.IO.File.Delete(Path.Combine(Server.MapPath("~/image"), n.Photo));
                    // Save new file
                    string fileName = Path.GetFileName(UploadImage.FileName);
                    string path = Path.Combine(Server.MapPath("~/image"), fileName);
                    UploadImage.SaveAs(path);
                    n.Photo = fileName;

                }
                var model = db.Products.FirstOrDefault(m => m.ProductId == n.ProductId);
                model.ProductId = n.ProductId;
                model.Name = n.Name;
                model.Photo = n.Photo;
                model.Price = n.Price;
                model.Quantity = n.Quantity;
                model.StartDate = n.StartDate;
                model.EndDate = n.EndDate;
                model.CateId = model.CateId;
                model.ProductId = n.ProductId;
                model.Description = n.Description;
                model.Discount = n.Discount;
                model.ProviderId = n.ProviderId;
                db.SaveChanges();
                return RedirectToAction("Show", new { CateId = n.CateId });
            }
            else
            {
                ModelState.AddModelError("", "Ngày kết thúc phải muộn hơn ngày bắt đầu");
                return View();
            }
        }

        //[HttpGet]
        //[HasCredential(RoleId = "DELETE_PRODUCT")]
        //public ActionResult Delete()
        //{
        //    var session = (UserLogin)Session[WebsiteNoiThat.Common.Commoncontent.user_sesion_admin];
        //    ViewBag.username = session.Username;
        //    return View();
        //}
        [HttpGet]
        [HasCredential(RoleId = "DELETE_PRODUCT")]
        public ActionResult Delete(int id)
        {
            var model = db.Products.Find(Convert.ToInt32(id));
            db.Products.Remove(model);
            db.SaveChanges();
            return View();
        }

        public ActionResult Menu()
        {
            var session = (UserLogin)Session[WebsiteNoiThat.Common.Commoncontent.user_sesion_admin];
            ViewBag.username = session.Username;

            var model = new CategoryDao().ListCategory();
            return PartialView(model);
        }
    }
}