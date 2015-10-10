using Microsoft.AspNet.Identity.Owin;
using RGSWeb.Models;
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

namespace RGSWeb.Controllers
{
    [Authorize]
    public class WorkItemsController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        // TODO: Add [Authorize(Roles = "Admin")] when login is implemented
        /// <summary>
        /// Returns all work items
        /// </summary>
        /// <remarks>Admin only</remarks>
        public IQueryable<WorkItem> GetWorkItems()
        {
            return _db.WorkItems;
        }

        /// <summary>
        /// Returns all work items associated with a class
        /// </summary>
        /// <param name="classId">Id of the class</param>
        [ResponseType(typeof(IEnumerable<WorkItem>))]
        public async Task<IHttpActionResult> GetWorkItems(int classId)
        {
            var @class = await _db.Classes.FindAsync(classId);
            if(@class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + classId));
            }

            return Ok(_db.WorkItems.Where(wi => wi.Class.Id == classId));
        }

        // PUT: api/WorkItems/5
        /// <summary>
        /// Update a work item
        /// </summary>
        [ResponseType(typeof(HttpStatusCode))]
        public async Task<IHttpActionResult> PutWorkItem(UpdateWorkItemViewModel workItemvm)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WorkItem workItem = await _db.WorkItems.FindAsync(workItemvm.Id);
            if(workItem == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No workitem with id: " + workItemvm.Id));
            }

            workItem.Title = workItemvm.Title;
            workItem.Description = workItemvm.Description;
            workItem.DueDate = workItemvm.DueDate;
            workItem.MaxPoints = workItemvm.MaxPoints;
            _db.Entry(workItem).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!WorkItemExists(workItemvm.Id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No workitem with id: " + workItemvm.Id));
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

            ApplicationUser teacher = await UserManager.FindByNameAsync(workItemvm.TeacherUserName);
            if(teacher == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No teacher with id: " + workItemvm.TeacherUserName));
            }

            WorkItem workItem = new WorkItem
            {
                Title = workItemvm.Title,
                Description = workItemvm.Description,
                DueDate = workItemvm.DueDate,
                AssignedBy = teacher,
                MaxPoints = workItemvm.MaxPoints
            };

            _db.WorkItems.Add(workItem);
            await _db.SaveChangesAsync();

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
            WorkItem workItem = await _db.WorkItems.FindAsync(id);
            if(workItem == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No workitem with id: " + id));
            }

            // Remove all associated data
            var scoreUnits = _db.ScoreUnits.Where(sc => sc.WorkItem.Id == workItem.Id);
            _db.ScoreUnits.RemoveRange(scoreUnits);
            _db.WorkItems.Remove(workItem);

            await _db.SaveChangesAsync();
            return Ok(workItem);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WorkItemExists(int id)
        {
            return _db.WorkItems.Count(e => e.Id == id) > 0;
        }
    }
}