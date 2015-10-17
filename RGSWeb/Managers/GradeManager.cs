using RGSWeb.Models;
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
        private ApplicationUserManager _userManager;

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
        public async Task<float> GetStudentGrade(ApplicationUser student, Class @class)
        {
            WorkItemManager workItemManager = new WorkItemManager(_db);
            ScoreUnitManager scoreUnitManager = new ScoreUnitManager(_db);

            var workItems = await workItemManager.GetClassWorkItems(@class);

            // Group the work items by type
            var groups = workItems.GroupBy(wi => wi.Type);

            float grade = 0.0f;

            // For each work item type, sum up the students grades and average
            foreach(var group in groups)
            {
                float percentage = 0;
                switch(group.Key)
                {
                    case WorkItemType.Exam:
                        percentage = @class.GradeDistribution.Exam;
                        break;
                    case WorkItemType.Project:
                        percentage = @class.GradeDistribution.Project;
                        break;
                    case WorkItemType.Homework:
                        percentage = @class.GradeDistribution.Homework;
                        break;
                    case WorkItemType.Quiz:
                        percentage = @class.GradeDistribution.Quiz;
                        break;
                    case WorkItemType.Other:
                        percentage = @class.GradeDistribution.Other;
                        break;
                }

                float studentsScores = 0.0f;
                float total = 0.0f;
                bool hasGradedScoreUnit = false;
                foreach(var workItem in group)
                {
                    var scoreUnit = await scoreUnitManager.GetStudentScoreUnit(workItem, student);
                    if(scoreUnit != null && scoreUnit.Grade.HasValue)
                    {
                        hasGradedScoreUnit = true;
                        studentsScores += scoreUnit.Grade.Value;
                        total += workItem.MaxPoints;
                    }
                }

                // If no grade assigned for this group, assign maximum points
                if(!hasGradedScoreUnit)
                {
                    grade += percentage;
                }
                else
                {
                    grade += (studentsScores / total) * percentage;
                }
            }
            return grade;
        }
    }
}