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

namespace RGSWeb.Controllers
{
    // TODO: Add [Authorize] when login is implemented
    public class ScoreUnitsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ScoreUnits
        // TODO: Add [Authorize(Roles = "Admin")] when login is implemented
        /// <summary>
        /// Returns all score-units
        /// </summary>
        public IQueryable<ScoreUnit> GetScoreUnits()
        {
            return db.ScoreUnits;
        }

        // GET: api/ScoreUnits/5
        /// <summary>
        /// Returns all score units associated with a work item
        /// </summary>
        /// <param name="workItemId">Id of the score unit to get</param>
        [ResponseType(typeof(IQueryable<ScoreUnitBindingModel>))]
        public IHttpActionResult GetScoreUnits(int workItemId)
        {
            return Ok(db.ScoreUnits.Where(su => su.WorkItem.Id == workItemId).Select(su => new ScoreUnitBindingModel
            {
                Id = su.Id,
                StudentId = su.Student.Id,
                WorkItemId = su.WorkItem.Id,
                Grade = su.Grade
            }));
        }

        // PUT: api/ScoreUnits
        /// <summary>
        /// Add or update grade(s)
        /// </summary>
        public async Task<IHttpActionResult> PutScoreUnits(List<ScoreUnitBindingModel> scoreUnits)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(scoreUnits.Count() == 0)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                foreach(var workItem in scoreUnits)
                {
                    try
                    {
                        await ProcessScoreUnit(workItem);
                    }
                    catch(Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
                await db.SaveChangesAsync();
                return StatusCode(HttpStatusCode.NoContent);
            }
        }

        private async Task ProcessScoreUnit(ScoreUnitBindingModel model)
        {
            // Check that no score unit already exists for this work item for the student
            var workItem = await db.WorkItems.FindAsync(model.WorkItemId);
            var student = db.Users.Find(model.StudentId);
            if(workItem == null || student == null)
            {
                throw new Exception("Could not match Id to existing entry");
            }

            var scoreUnit = await db.ScoreUnits.Where(sc => sc.Student.Id == student.Id && sc.WorkItem.Id == workItem.Id).FirstOrDefaultAsync();
            // If it exists already, update it
            if(scoreUnit != null)
            {
                scoreUnit.Grade = model.Grade;
                db.Entry(scoreUnit).State = EntityState.Modified;
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
                db.ScoreUnits.Add(scoreUnit);
            }
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