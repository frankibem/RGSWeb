using Microsoft.AspNet.Identity.EntityFramework;
using RGSWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace RGSWeb.Managers
{
    /// <summary>
    /// Manages all ScoreUnit related actions - creation, update, deletion...
    /// </summary>
    public class ScoreUnitManager
    {
        private ApplicationDbContext _db;
        private ApplicationUserManager _userManager;

        private const string teacherRole = "Teacher";
        private const string studentRole = "Student";

        /// <summary>
        /// Creates a new ScoreUnitManager with the given Database context
        /// </summary>
        /// <param name="db">The Database context to use</param>
        public ScoreUnitManager(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates a new ScoreUnitManager with the given Database context and UserManager
        /// </summary>
        /// <param name="db">The Database context to use</param>
        /// <param name="userManager">The UserManager to use</param>
        /// <remarks>The UserManager must have been created from db</remarks>
        public ScoreUnitManager(ApplicationDbContext db, ApplicationUserManager userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns all the ScoreUnits associated with the given WorkItem
        /// </summary>
        /// <param name="workItem">WorkItem for which to return ScoreUnits</param>
        /// <returns>Creates new ScoreUnits for all students who have none</returns>
        public async Task<IEnumerable<ScoreUnit>> GetScoreUnits(WorkItem workItem)
        {
            ClassManager classManager = new ClassManager(_db, _userManager);
            var students = await classManager.GetAcceptedStudents(workItem.Class);

            var newScoreUnits = new List<ScoreUnit>();
            var scoreUnits = await _db.ScoreUnits
                .Include(su => su.Student)
                .Where(su => su.WorkItem.Id == workItem.Id).ToDictionaryAsync(su => su.Student.Id);

            // Create new ScoreUnits for those who don't have one
            foreach(var student in students)
            {
                if(!scoreUnits.ContainsKey(student.Id))
                {
                    var newScoreUnit = new ScoreUnit();
                    newScoreUnit.WorkItem = workItem;
                    newScoreUnit.Student = student;
                    newScoreUnit.Grade = null;

                    newScoreUnits.Add(newScoreUnit);
                }
            }

            var addedScoreUnits = _db.ScoreUnits.AddRange(newScoreUnits);
            await _db.SaveChangesAsync();

            // Convert dictionary to list and add new ScoreUnits
            var result = scoreUnits.Select(kvp => kvp.Value).ToList();
            result.AddRange(addedScoreUnits);
            return result;
        }

        /// <summary>
        /// Returns a students ScoreUnit for a WorkItem
        /// </summary>
        /// <param name="workItem">WorkItem to return the ScoreUnit for</param>
        /// <param name="student">Student to return the ScorreUnit for</param>
        /// <returns></returns>
        public async Task<ScoreUnit> GetStudentScoreUnit(WorkItem workItem, ApplicationUser student)
        {
            return await _db.ScoreUnits.Where(su => su.WorkItem.Id == workItem.Id && su.Student.Id == student.Id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Updates the ScoreUnits in the database using details from the modesl in the given list
        /// </summary>
        /// <param name="subm">List of models used for the update</param>
        /// <exception cref="Exception">Throws an exception if one of the models could not be found</exception>
        public async Task UpdateScoreUnits(List<ScoreUnitBindingModel> subm)
        {
            if(subm.Count != 0)
            {
                foreach(var model in subm)
                {
                    var scoreUnit = await _db.ScoreUnits.FindAsync(model.Id);
                    if(scoreUnit == null)
                    {
                        throw new Exception("Could not find ScoreUnit with id: " + model.Id);
                    }

                    scoreUnit.Grade = model.Grade;
                    _db.Entry(scoreUnit).State = EntityState.Modified;
                }
                await _db.SaveChangesAsync();
            }
        }
    }
}