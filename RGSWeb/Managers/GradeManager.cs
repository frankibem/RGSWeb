using RGSWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RGSWeb.Managers
{
    /// <summary>
    /// Used to determine a students grade
    /// </summary>
    public class GradeManager
    {
        private ApplicationDbContext _db;

        /// <summary>
        /// Creates a new GradeManager with the given Database context
        /// </summary>
        /// <param name="db">The Database context to use</param>
        public GradeManager(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Calculates and returns a students grade in a class
        /// </summary>
        /// <param name="student">To student to calculate a grade for</param>
        /// <param name="class">The class that the student is enrolled in</param>
        /// <returns></returns>
        public async Task<float> GetStudentGradeAsync(ApplicationUser student, Class @class)
        {
            WorkItemManager workItemManager = new WorkItemManager(_db);

            var workItems = await workItemManager.GetClassWorkItems(@class);
            var total = 0.0f;

            total += await GetAverageForType(student, workItems, WorkItemType.Exam, @class.GradeDistribution);
            total += await GetAverageForType(student, workItems, WorkItemType.Homework, @class.GradeDistribution);
            total += await GetAverageForType(student, workItems, WorkItemType.Other, @class.GradeDistribution);
            total += await GetAverageForType(student, workItems, WorkItemType.Project, @class.GradeDistribution);
            total += await GetAverageForType(student, workItems, WorkItemType.Quiz, @class.GradeDistribution);

            return total;
        }

        private async Task<float> GetAverageForType(ApplicationUser student, IEnumerable<WorkItem> workItems, WorkItemType type, GradeDistribution distribution)
        {
            var scoreUnitManager = new ScoreUnitManager(_db);
            var workItemsForType = workItems.Where(wi => wi.Type == type);
            var PointsForType = MaxPointsForType(type, distribution);

            // If no WorkItem for the type, return total for that type
            if(workItemsForType.Count() == 0)
            {
                return PointsForType;
            }

            bool hasGradedScoreUnit = false;
            float studentsScores = 0.0f;
            float total = 0.0f;
            foreach(var workItem in workItemsForType)
            {
                var scoreUnit = await scoreUnitManager.GetStudentScoreUnitForWorkItem(workItem, student);
                if(scoreUnit != null && scoreUnit.Grade.HasValue)
                {
                    hasGradedScoreUnit = true;
                    studentsScores += scoreUnit.Grade.Value;
                    total += workItem.MaxPoints;
                }
            }

            // If no ScoreUnit has been graded, return the total for that type
            if(!hasGradedScoreUnit)
            {
                return PointsForType;
            }

            return (studentsScores / total) * PointsForType;
        }

        private float MaxPointsForType(WorkItemType type, GradeDistribution distribution)
        {
            switch(type)
            {
                case WorkItemType.Exam:
                    return distribution.Exam;
                case WorkItemType.Project:
                    return distribution.Project;
                case WorkItemType.Homework:
                    return distribution.Homework;
                case WorkItemType.Quiz:
                    return distribution.Quiz;
                case WorkItemType.Other:
                    return distribution.Other;
                default:
                    return 0;
            }
        }
    }
}