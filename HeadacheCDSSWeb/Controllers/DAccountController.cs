using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HeadacheCDSSWeb.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace HeadacheCDSSWeb.Controllers
{
    public class DAccountController : Controller
    {
        //
        // GET: /DAccount/

        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Index(DoctorAccount dacount)
        {
           
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
           
            UserOperation UOpertation = new UserOperation();
            if (UOpertation.ValidateUser(dacount.UserName,dacount.PassWord))
            {
                HttpCookie cookie = new HttpCookie("username", dacount.UserName);
                Response.Cookies.Add(cookie);
                return RedirectToAction("Index", "EnterPatInfor");

            }
            else
            {
                ViewBag.message = "用户名或密码错误";
                return View();
            }



        }
    }
}
