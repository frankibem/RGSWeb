using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using RGSWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RGSWeb.Controllers.MVC
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        ApplicationDbContext db;
        ApplicationUserManager _userManager;

        public UsersController()
        {
            db = new ApplicationDbContext();
        }

        // GET: Users
        public async Task<ActionResult> All(string role)
        {
            // All users
            if(role == null)
            {
                ViewBag.Title = "All users";
                return View(db.Users.ToList());
            }

            ViewBag.Title = String.Format("Users in '{0}'", role);

            // Users in specific role
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var result = new List<ApplicationUser>();
            if(await roleManager.RoleExistsAsync(role))
            {
                foreach(var user in db.Users)
                {
                    if(await UserManager.IsInRoleAsync(user.Id, role))
                    {
                        result.Add(user);
                    }
                }
                return View(result);
            }
            else
            {
                return View();
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
    }
}