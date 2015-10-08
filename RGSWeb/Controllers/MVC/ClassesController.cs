using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RGSWeb.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RGSWeb.Controllers.MVC
{
    [Authorize(Roles = "Admin")]
    public class ClassesController : Controller
    {
        private ApplicationDbContext db;
        private ApplicationUserManager userManager;

        public ClassesController()
        {
            db = new ApplicationDbContext();
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
        }

        // GET: Classes
        public async Task<ActionResult> Index()
        {
            return View(await db.Classes.ToListAsync());
        }

        public async Task<ActionResult> List(string name)
        {
            var user = await userManager.FindByNameAsync(name);
            if(user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var result = await db.Enrollments.Where(e => e.Student.UserName == name).Select(e => e.Class).ToListAsync();
            ViewBag.UserName = String.Format("{0}, {1}", user.LastName, user.FirstName);
            return View(result);
        }

        // GET: Classes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = await db.Classes.FindAsync(id);
            if(@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // GET: Classes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Title,Prefix,CourseNumber,Section,TeacherUserName")] CreateClassBindingModel @cvm)
        {
            if(!ModelState.IsValid)
            {
                return View(cvm);
            }

            var teacher = await userManager.FindByNameAsync(cvm.TeacherUserName);
            if(teacher == null)
            {
                // TODO: Add error for teacher user name
                return View(cvm);
            }

            Class @class = new Class
            {
                Title = cvm.Title,
                Prefix = cvm.Prefix,
                CourseNumber = cvm.CourseNumber,
                Section = cvm.Section,
                Teacher = teacher
            };

            db.Classes.Add(@class);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Classes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = await db.Classes.FindAsync(id);
            if(@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Prefix,CourseNumber,Section")] Class @class)
        {
            if(ModelState.IsValid)
            {
                db.Entry(@class).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(@class);
        }

        // GET: Classes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = await db.Classes.FindAsync(id);
            if(@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Class @class = await db.Classes.FindAsync(id);
            db.Classes.Remove(@class);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
