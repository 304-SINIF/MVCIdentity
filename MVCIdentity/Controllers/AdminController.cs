using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCIdentity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCIdentity.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private UserManager<AppUser> userManager;

        public AdminController()
        {
            userManager = new UserManager<AppUser>(new UserStore<AppUser>(new IdentityDataContext()));
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View(userManager.Users);
        }
    }
}