using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MVCIdentity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MVCIdentity.Models.UserModel;

namespace MVCIdentity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        
        private UserManager<AppUser> userManager;

        public AccountController()
        {
            userManager = new UserManager<AppUser>(new UserStore<AppUser>(new IdentityDataContext()));

            userManager.PasswordValidator=new CustomPasswordValidator() { RequireDigit = true, //sayısal ifade olmalı
                RequiredLength=8, // En az 8 karakter olmalı
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonLetterOrDigit = true, //alfanumerik değer olmalı
                
            
            
            
            };
            userManager.UserValidator = new UserValidator<AppUser>(userManager)
            {
                RequireUniqueEmail = true,
                AllowOnlyAlphanumericUserNames = false,
            };
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("Error", new string[] { "Erişim hakkınız yoktur" });
            }
            ViewBag.returnUrl=returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user= userManager.Find(model.UserName,model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Yanlış kullanıcı adı veya parola");
                }
                else
                {
                    var authManager=HttpContext.GetOwinContext().Authentication; // Login ve Logout işlemlerini yerine getirecek değişken authManager
                    var identity = userManager.CreateIdentity(user, "ApplicationCookie");
                    var autprop = new AuthenticationProperties()
                    {

                        IsPersistent = true
                    };
                    authManager.SignOut();
                    authManager.SignIn(autprop,identity);
                    return Redirect(string.IsNullOrEmpty(returnUrl)?"/":returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
       
        public ActionResult Register(Register model)
        {
            if (ModelState.IsValid) { 
            var user = new AppUser();
                user.UserName = model.UserName;
                user.Email = model.Email;
                var result=userManager.Create(user,model.Password);
                if (result.Succeeded)
                {
                  //  userManager.AddToRole(user.Id, "User");//İlk kayıtta rol atama
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

            }
            return View(model);


        }

        public ActionResult Logout()
        {
            var authManager=HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("Login");
        }
    }
}