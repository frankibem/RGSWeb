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

        /// <summary>
        /// Creates a new default ScoreUnitsController
        /// </summary>
        public ScoreUnitsController()
        {
            _db = new ApplicationDbContext();
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_db));
            _scoreUnitManager = new ScoreUnitManager(_db);
        }

        // GET: api/ScoreUnits/5
        /// <summary>
        /// Returns all score units associated with a work item
        /// </summary>
        /// <param name="workItemId">Id of the score unit to get</param>
        [ResponseType(typeof(IQueryable<ScoreUnitBindingModel>))]
        public async Task<IEnumerable<ScoreUnitBindingModel>> GetScoreUnits(int workItemId)
        {
            var workItem = await _db.WorkItems.Include(wi => wi.Class).Where(wi => wi.Id == workItemId).FirstOrDefaultAsync();
            if(workItem == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No WorkItem with id: " + workItemId));
            }

            var scoreUnits = await _scoreUnitManager.GetScoreUnits(workItem);
            return scoreUnits.Select(su => new ScoreUnitBindingModel(su)).ToList();
        }

        // PUT: api/ScoreUnits
        /// <summary>
        /// Updates grade(s) for a WorkItem
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
                await _scoreUnitManager.UpdateScoreUnits(scoreUnits);
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }

            return StatusCode(HttpStatusCode.NoContent);
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