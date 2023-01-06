using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCIdentity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MVCIdentity.Models.UserModel;

namespace MVCIdentity.Controllers
{
    public class RoleAdminController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<AppUser> userManager;

        public RoleAdminController()
        {
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new IdentityDataContext()));

            userManager = new UserManager<AppUser>(new UserStore<AppUser>(new IdentityDataContext()));
        }
        // GET: RoleAdmin
        public ActionResult Index()
        {
            return View(roleManager.Roles);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string name)
        {
            if (ModelState.IsValid)
            {
                var result = roleManager.Create(new IdentityRole(name));

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item);
                    }
                }
            }


            return View();
        }

        public ActionResult Delete(string id)
        {
            var role = roleManager.FindById(id);
            if (role != null)
            {
                var result=roleManager.Delete(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error",result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "Not Found Role" });
            }
        }
        public ActionResult Edit(string id)
        {
            var role=roleManager.FindById(id);
            var members=new List<AppUser>();
            var nonmembers=new List<AppUser>();

            foreach (var user in userManager.Users.ToList())
            {
                var list = userManager.IsInRole(user.Id, role.Name) ? members : nonmembers;
                list.Add(user);
            }
            return View(new RoleEditModel()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            });

            

        }
        [HttpPost]
        public ActionResult Edit(RoleUpdateModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (var UserId in model.IdsToAdd ?? new string[] {})
                {
                    result = userManager.AddToRole(UserId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }

                }
                foreach (var UserId in model.IdsToDelete ?? new string[] { })
                {
                    result = userManager.RemoveFromRole(UserId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }

                }

                return RedirectToAction("Index");

            }
            return View("Error", new string[] { "Aranan rol yok" });

        }
    }
}