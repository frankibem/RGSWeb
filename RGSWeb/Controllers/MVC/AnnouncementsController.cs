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
using RGSWeb.Managers;

namespace RGSWeb.Controllers.MVC
{
    [Authorize(Roles = "Admin")]
    public class AnnouncementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Announcements
        public async Task<ActionResult> Index(int? classId)
        {
            if(!classId.HasValue)
            {
                ViewBag.Title = "All announcements";
                return View(await db.Announcements.ToListAsync());
            }

            Class @class = await db.Classes.FindAsync(classId);
            if(@class == null)
            {
                ViewBag.Title = "Error";
                ModelState.AddModelError("error", string.Format("No class with id \"{0}\"", classId));
                return View();
            }

            ViewBag.Title = string.Format("Announcements for \"{0}\"", @class.Title);
            AnnouncementManager manager = new AnnouncementManager(db);
            return View(await manager.GetClassAnnouncements(@class));
        }

        // GET: Announcements/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Announcement announcement = await db.Announcements.Include(ann => ann.Class.Teacher).SingleOrDefaultAsync(ann => ann.Id == id);
            if(announcement == null)
            {
                return HttpNotFound();
            }
            return View(announcement);
        }

        // GET: Announcements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Announcements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Description,CreatedOn")] Announcement announcement)
        {
            if(ModelState.IsValid)
            {
                db.Announcements.Add(announcement);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(announcement);
        }

        // GET: Announcements/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcement announcement = await db.Announcements.FindAsync(id);
            if(announcement == null)
            {
                return HttpNotFound();
            }
            return View(announcement);
        }

        // POST: Announcements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,CreatedOn")] Announcement announcement)
        {
            if(ModelState.IsValid)
            {
                db.Entry(announcement).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(announcement);
        }

        // GET: Announcements/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcement announcement = await db.Announcements.Include(ann => ann.Class.Teacher).SingleOrDefaultAsync(ann => ann.Id == id);
            if(announcement == null)
            {
                return HttpNotFound();
            }
            return View(announcement);
        }

        // POST: Announcements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Announcement announcement = await db.Announcements.FindAsync(id);
            db.Announcements.Remove(announcement);
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
