using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGSWeb.Models
{
    public class ScoreUnitBindingModel
    {
        /// <summary>
        /// Id of the score unit to update
        /// </summary>
        /// <remarks>Set to 0 if adding for the first time</remarks>
        public int Id { get; set; }
        /// <summary>
        /// Id of the student
        /// </summary>
        public string StudentId { get; set; }
        /// <summary>
        /// Id of the work item
        /// </summary>
        public int WorkItemId { get; set; }
        /// <summary>
        /// Student's grade
        /// </summary>
        public float? Grade { get; set; }
    }
}