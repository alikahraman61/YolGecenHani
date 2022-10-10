using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YolGecenHani.Models;
using YolGecenHani.Models.EntityModel;
using YolGecenHani.Models.HomeModel;

namespace YolGecenHani.Controllers
{
    public class CartController : Controller
    {
        DataContext db = new DataContext();
        // GET: Cart
        public ActionResult Index()
        {
            return PartialView(GetCart());
        }
        [HttpPost]
        public ActionResult AddToCart(int id)
        {
            var product = db.Product.FirstOrDefault(x => x.Id == id);
            if (product != null)
            {
                GetCart().AddProduct(product, 1);
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            var product = db.Product.FirstOrDefault(x => x.Id == id);
            if (product!=null)
            {
                GetCart().DeleteProduct(product);
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult ClearCart()
        {
            if (GetCart().CartLines.Count > 0)
            {
                GetCart().Clear();
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        public Cart GetCart()
        {
            var cart = (Cart)Session["Cart"];
            if (cart == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }

        public ActionResult ViewCart()
        {
            return View(GetCart());
        }
        public ActionResult ProductMinus(int id)
        {
            var product = db.Product.FirstOrDefault(x => x.Id == id);
            if (product != null)
            {
                GetCart().AddProduct(product, -1);
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult ProductPlus(int id)
        {
            var product = db.Product.FirstOrDefault(x => x.Id == id);
            if (product != null)
            {
                GetCart().AddProduct(product, 1);
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult Checkout()
        {
            return View();
        }
        public PartialViewResult CheckoutPartial()
        {
            return PartialView(GetCart());
        }
        public PartialViewResult CheckoutShippingForm()
        {

            var user = db.User.FirstOrDefault(x => x.UserName == User.Identity.Name);
            ShippingAddressModel model = new ShippingAddressModel();
            if (user != null)
            {
                model.User = user;
                model.City = db.City.Where(x => x.IsDelete == false).ToList();
                model.UserAddress = db.UserAddress.FirstOrDefault(x => x.UserId == user.Id && x.Selected==true);
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult CheckoutShippingForm(string Name,string Surname, string AddressTitle,string Email, string Phone,string Address, string City, string Notes)
        {
            OrderShippingAddress osa = new OrderShippingAddress();
            osa.OrderNumber = "YGH" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + Guid.NewGuid().ToString().Substring(0, 10);
            osa.Name = Name;
            osa.Surname = Surname;
            osa.Email = Email;
            osa.Phone = Phone;
            osa.AdressTitle = AddressTitle;
            osa.Address = Address;
            osa.City = City;
            osa.Notes = Notes;

            var shippingaddres = (OrderShippingAddress)Session["ShippingAddress"];
            shippingaddres = osa;

            return RedirectToAction("Payment");
        }
        public ActionResult Payment()
        {
            return View();
        }
    }
}