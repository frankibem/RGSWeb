using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using RGSWeb.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RGSWeb.Controllers
{
    // TODO: Add [Authorize] when login is implemented
    public class WorkItemsController : ApiController
    {
        private ApplicationDbContext db;
        private ApplicationUserManager userManager;

        public WorkItemsController()
        {
            db = new ApplicationDbContext();
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
        }

        // TODO: Add [Authorize(Roles = "Admin")] when login is implemented
        /// <summary>
        /// Returns all work items
        /// </summary>
        /// <remarks>Admin only</remarks>
        public IQueryable<WorkItem> GetWorkItems()
        {
            return db.WorkItems;
        }

        /// <summary>
        /// Returns all work items associated with a class
        /// </summary>
        /// <param name="classId">Id of the class</param>
        public async Task<IHttpActionResult> GetWorkItems(int classId)
        {
            var @class = await db.Classes.FindAsync(classId);
            if(@class == null)
            {
                return BadRequest(classId.ToString() + " is not a valid class Id");
            }

            return Ok(db.WorkItems.Where(wi => wi.Class.Id == classId));
        }

        // PUT: api/WorkItems/5
        /// <summary>
        /// Update a work item
        /// </summary>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutWorkItem(UpdateWorkItemViewModel workItemvm)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WorkItem workItem = await db.WorkItems.FindAsync(workItemvm.Id);
            if(workItem == null)
            {
                return NotFound();
            }

            workItem.Title = workItemvm.Title;
            workItem.Description = workItemvm.Description;
            workItem.DueDate = workItemvm.DueDate;
            workItem.MaxPoints = workItemvm.MaxPoints;
            db.Entry(workItem).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!WorkItemExists(workItemvm.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/WorkItems
        /// <summary>
        /// Create a new work item
        /// </summary>
        [ResponseType(typeof(WorkItem))]
        public async Task<IHttpActionResult> PostWorkItem(CreateWorkItemViewModel workItemvm)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser teacher = await userManager.FindByIdAsync(workItemvm.TeacherId);
            if(teacher == null)
            {
                return BadRequest(workItemvm.TeacherId.ToString() + " is not a valid teacher id");
            }

            WorkItem workItem = new WorkItem
            {
                Title = workItemvm.Title,
                Description = workItemvm.Description,
                DueDate = workItemvm.DueDate,
                AssignedBy = teacher,
                MaxPoints = workItemvm.MaxPoints
            };

            db.WorkItems.Add(workItem);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = workItem.Id }, workItem);
        }

        // DELETE: api/WorkItems/5
        /// <summary>
        /// Delete a work item and all associated data
        /// </summary>
        /// <param name="id">Id of the work item to delete</param>
        [ResponseType(typeof(WorkItem))]
        public async Task<IHttpActionResult> DeleteWorkItem(int id)
        {
            WorkItem workItem = await db.WorkItems.FindAsync(id);
            if(workItem == null)
            {
                return NotFound();
            }

            var scoreUnits = db.ScoreUnits.Where(sc => sc.WorkItem.Id == workItem.Id);
            db.ScoreUnits.RemoveRange(scoreUnits);
            db.WorkItems.Remove(workItem);
            await db.SaveChangesAsync();

            return Ok(workItem);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WorkItemExists(int id)
        {
            return db.WorkItems.Count(e => e.Id == id) > 0;
        }
    }
}