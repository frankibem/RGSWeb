using Microsoft.AspNet.Identity.EntityFramework;
using RGSWeb.Managers;
using RGSWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RGSWeb.Controllers
{
    /// <summary>
    /// API Controller for ScoreUnit related actions
    /// </summary>
    [Authorize]
    public class ScoreUnitsController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private ScoreUnitManager _scoreUnitManager;

        private ScoreUnitManager ScoreUnitManager
        {
            get
            {
                if(_scoreUnitManager == null)
                {
                    _scoreUnitManager = new ScoreUnitManager(_db, UserManager);
                }
                return _scoreUnitManager;
            }
            set { _scoreUnitManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? new ApplicationUserManager(new UserStore<ApplicationUser>(_db)); }
            set { _userManager = value; }
        }

        // GET: api/ScoreUnits
        // TODO: Add [Authorize(Roles = "Admin")] when login is implemented
        /// <summary>
        /// Returns all score-units
        /// </summary>
        public IQueryable<ScoreUnit> GetScoreUnits()
        {
            return _db.ScoreUnits;
        }

        // GET: api/ScoreUnits/5
        /// <summary>
        /// Returns all score units associated with a work item
        /// </summary>
        /// <param name="workItemId">Id of the score unit to get</param>
        [ResponseType(typeof(IQueryable<ScoreUnitBindingModel>))]
        public async Task<IEnumerable<ScoreUnitBindingModel>> GetScoreUnits(int workItemId)
        {
            var workItem = await _db.WorkItems.FindAsync(workItemId);
            if(workItem == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No WorkItem with id: " + workItemId));
            }

            var scoreUnits = await ScoreUnitManager.GetScoreUnits(workItem);
            return scoreUnits.Select(su => new ScoreUnitBindingModel(su)).ToList();
        }

        // PUT: api/ScoreUnits
        /// <summary>
        /// Add or update grade(s)
        /// </summary>
        [ResponseType(typeof(HttpStatusCode))]
        public async Task<IHttpActionResult> PutScoreUnits(List<ScoreUnitBindingModel> scoreUnits)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await ScoreUnitManager.UpdateScoreUnits(scoreUnits);
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Updates a scoreunit if it already exists, otherwise creates a new one for the associated student
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task ProcessScoreUnit(ScoreUnitBindingModel model)
        {
            // Check that no score unit already exists for this work item for the student
            var student = await UserManager.FindByNameAsync(model.StudentUserName);
            var workItem = await _db.WorkItems.FindAsync(model.WorkItemId);

            if(workItem == null || student == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    string.Format("Could not match workItem:{0} | student:{1} to existing records", model.WorkItemId, model.StudentUserName)));
            }

            var scoreUnit = await _db.ScoreUnits.Where(sc => sc.Student.Id == student.Id && sc.WorkItem.Id == workItem.Id).FirstOrDefaultAsync();
            // If it exists already, update it
            if(scoreUnit != null)
            {
                scoreUnit.Grade = model.Grade;
                _db.Entry(scoreUnit).State = EntityState.Modified;
            }
            // Create a new score unit
            else
            {
                scoreUnit = new ScoreUnit
                {
                    Student = student,
                    WorkItem = workItem,
                    Grade = model.Grade
                };
                _db.ScoreUnits.Add(scoreUnit);
            }
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