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
using RGSWeb.Managers;

namespace RGSWeb.Controllers.MVC
{
    public class ClassesController : Controller
    {
        private ApplicationDbContext db;
        private ApplicationUserManager userManager;
        private ClassManager classManager;

        public ClassesController()
        {
            db = new ApplicationDbContext();
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            classManager = new ClassManager(db);
        }

        // GET: Classes
        public async Task<ActionResult> Index()
        {
            return View(await db.Classes.Include(@class => @class.Teacher).ToListAsync());
        }

        // GET: Classes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            const int maxItems = 10;

            Class @class = await db.Classes.FindAsync(id);
            if(@class == null)
            {
                return HttpNotFound();
            }

            var workItemManager = new WorkItemManager(db);
            var workItems = (await workItemManager.GetClassWorkItems(@class)).Take(5);
            ViewBag.WorkItems = workItems;

            var announcementManager = new AnnouncementManager(db);
            var announcements = (await announcementManager.GetAnnouncementsForClass(@class)).Take(5);
            ViewBag.Announcements = announcements;

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
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Prefix,CourseNumber,Section,TeacherUserName,GradeDistribution")] CreateClassBindingModel classModel)
        {
            if(!ModelState.IsValid)
            {
                return View(@classModel);
            }

            var result = await classManager.CreateClass(classModel);
            if(result == null)
            {
                ModelState.AddModelError("TeacherUserName", "Could not match username to existing record");
                return View(@classModel);
            }

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
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Prefix,CourseNumber,Section,GradeDistribution")] Class @class)
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
