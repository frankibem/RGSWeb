using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using RGSWeb.Managers;
using RGSWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RGSWeb.Controllers.MVC
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        ApplicationDbContext db;
        ApplicationUserManager userManager;
        RoleManager<IdentityRole> roleManager;

        public UsersController()
        {
            db = new ApplicationDbContext();
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
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
                    if(await userManager.IsInRoleAsync(user.Id, role))
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

        /// <summary>
        /// Returns a list of all teachers in the application
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Teachers(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if(searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var teachers = (await db.Users.ToListAsync()).Where(user => userManager.IsInRole(user.Id, "teacher"));

            if(!string.IsNullOrEmpty(searchString))
            {
                teachers = teachers.Where(s => s.LastName.Contains(searchString)
                || s.FirstName.Contains(searchString)
                || s.Email.Contains(searchString));
            }

            switch(sortOrder)
            {
                case "name_desc":
                    teachers = teachers.OrderByDescending(s => s.LastName);
                    break;
                default:
                    teachers = teachers.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 20;
            int pageNumber = page ?? 1;

            return View(teachers.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Returns a list of all students in the application
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Students(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if(searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var students = (await db.Users.ToListAsync()).Where(user => userManager.IsInRole(user.Id, "student"));

            if(!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                || s.FirstName.Contains(searchString)
                || s.Email.Contains(searchString));
            }

            switch(sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 20;
            int pageNumber = page ?? 1;

            return View(students.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Shows the details of the user with the given user name
        /// </summary>
        /// <param name="userName"></param>
        public async Task<ActionResult> Details(string userName)
        {
            var teacher = await userManager.FindByEmailAsync(userName);

            ClassManager manager = new ClassManager(db);
            var classes = await manager.GetUserClasses(teacher);
            ViewBag.Classes = classes;

            return View(teacher);
        }
    }
}