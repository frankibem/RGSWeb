using Microsoft.AspNet.Identity.EntityFramework;
using RGSWeb.Managers;
using RGSWeb.Models;
using RGSWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RGSWeb.Controllers
{
    /// <summary>
    /// API controller for WorkItem related actions
    /// </summary>
    [Authorize]
    public class WorkItemsController : ApiController
    {
        private ApplicationDbContext _db;
        private ApplicationUserManager _userManager;
        private WorkItemManager _workItemManager;

        /// <summary>
        /// Create a new default WorkItemsController
        /// </summary>
        public WorkItemsController()
        {
            _db = new ApplicationDbContext();
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_db));
            _workItemManager = new WorkItemManager(_db);
        }

        /// <summary>
        /// Returns all work items associated with a class
        /// </summary>
        /// <param name="classId">Id of the class</param>
        [ResponseType(typeof(IEnumerable<WorkItemViewModel>))]
        public async Task<IHttpActionResult> GetClassWorkItems(int classId)
        {
            var @class = await _db.Classes.FindAsync(classId);
            if(@class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + classId));
            }

            var result = (await _workItemManager.GetClassWorkItems(@class)).Select(wi => new WorkItemViewModel(wi));
            return Ok(result);
        }

        // PUT: api/WorkItems/5
        /// <summary>
        /// Update a WorkItem
        /// </summary>
        [ResponseType(typeof(HttpStatusCode))]
        public async Task<IHttpActionResult> PutWorkItem(UpdateWorkItemViewModel workItemvm)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _workItemManager.UpdateWorkItem(workItemvm);
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/WorkItems
        /// <summary>
        /// Create a new WorkItem
        /// </summary>
        [ResponseType(typeof(WorkItemViewModel))]
        public async Task<IHttpActionResult> PostWorkItem(CreateWorkItemViewModel workItemvm)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workItem = await _workItemManager.CreateWorkItem(workItemvm);
            if(workItem == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Could not match class to existing records"));
            }

            return CreatedAtRoute("DefaultApi", new { id = workItem.Id }, new WorkItemViewModel(workItem));
        }

        // DELETE: api/WorkItems/5
        /// <summary>
        /// Delete a WorkItem and all associated data
        /// </summary>
        /// <param name="id">Id of the WorkItem to delete</param>
        [ResponseType(typeof(WorkItemViewModel))]
        public async Task<WorkItemViewModel> DeleteWorkItem(int id)
        {
            var workItem = await _workItemManager.DeleteWorkItemById(id);
            if(workItem == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No workitem with id: " + id));
            }
            return new WorkItemViewModel(workItem);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}