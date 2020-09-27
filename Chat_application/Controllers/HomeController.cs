using Chat_application.Hubs;
using Chat_application.Models;
using Chat_application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Chat_application.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View(new LoginData());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginData loginData)
        {
            if (ModelState.IsValid)
            {
                int userID;
                if (new AppService().Login(loginData, out userID))
                {
                    FormsAuthentication.RedirectFromLoginPage(userID.ToString(), loginData.RememberMe);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.LoginError = "Username or password is incorrect";
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            int userId = int.Parse(User.Identity.Name);
            new AppService().RemoveAllUserConnections(userId);
            ChatHub.OfflineUser(userId);
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


        [HttpPost]
        public ActionResult GetChatbox(int toUserId)
        {
            ChatBoxModel chatBoxModel = new AppService().GetChatbox(toUserId);
            return PartialView("~/Views/Partials/ChatBox.cshtml", chatBoxModel);
        }

        [HttpPost]
        public ActionResult SendMessage(int toUserId, string message)
        {
            return Json(new AppService().SendMessage(toUserId, message));
        }

    }
}