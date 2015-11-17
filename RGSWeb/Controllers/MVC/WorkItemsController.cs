﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RGSWeb.Models;

namespace RGSWeb.Controllers.MVC
{
    public class WorkItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WorkItems
        public async Task<ActionResult> Index()
        {
            return View(await db.WorkItems.ToListAsync());
        }

        // GET: WorkItems/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkItem workItem = await db.WorkItems.Include(model => model.Class.Teacher).SingleOrDefaultAsync(model => model.Id == id);
            if(workItem == null)
            {
                return HttpNotFound();
            }
            return View(workItem);
        }

        // GET: WorkItems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WorkItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Description,DueDate,MaxPoints,Type")] WorkItem workItem)
        {
            if(ModelState.IsValid)
            {
                db.WorkItems.Add(workItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(workItem);
        }

        // GET: WorkItems/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkItem workItem = await db.WorkItems.FindAsync(id);
            if(workItem == null)
            {
                return HttpNotFound();
            }
            return View(workItem);
        }

        // POST: WorkItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,DueDate,MaxPoints,Type")] WorkItem workItem)
        {
            if(ModelState.IsValid)
            {
                db.Entry(workItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(workItem);
        }

        // GET: WorkItems/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkItem workItem = await db.WorkItems.FindAsync(id);
            if(workItem == null)
            {
                return HttpNotFound();
            }
            return View(workItem);
        }

        // POST: WorkItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            WorkItem workItem = await db.WorkItems.FindAsync(id);
            db.WorkItems.Remove(workItem);
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