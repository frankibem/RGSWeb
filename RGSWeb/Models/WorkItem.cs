using System;
using System.ComponentModel.DataAnnotations;

namespace RGSWeb.Models
{
    /// <summary>
    /// Model to represent a class work-item
    /// </summary>
    public class WorkItem
    {
        /// <summary>
        /// Id of the WorkItem
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The title of the WorkItem
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Description for the WorkItem
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Date and time at which the WorkItem is due
        /// </summary>
        [Display(Name = "Due on")]
        public DateTime DueDate { get; set; }
        /// <summary>
        /// Maximum points attainable for this WorkItem
        /// </summary>
        [Display(Name = "Max points")]
        public float MaxPoints { get; set; }
        /// <summary>
        /// The type of the WorkItem
        /// </summary>
        /// <example>Exam</example>
        public WorkItemType Type { get; set; }
        /// <summary>
        /// The Class to which the WorkItem belongs
        /// </summary>
        public Class Class { get; set; }
    }
}
