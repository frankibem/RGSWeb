using System;

namespace RGSWeb.Models
{
    public class CreateWorkItemViewModel
    {
        /// <summary>
        /// The title of the work item
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// A short description of the work item
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Date by which the work item is due
        /// </summary>
        public DateTime DueDate { get; set; }
        /// <summary>
        /// Id of the instructor for the class
        /// </summary>
        public string TeacherId { get; set; }
        // TODO: Ask and if necessary, remove. We can assume max points is always 100
        /// <summary>
        /// Maximum points assignable for this work item
        /// </summary>
        public float MaxPoints { get; set; }
        /// <summary>
        /// Numerical weight of this work item
        /// </summary>
        public float Weight { get; set; }
    }

    public class UpdateWorkItemViewModel
    {
        /// <summary>
        /// Id of the work item that is being updated
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The title of the work item
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// A short description of the work item
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Date by which the work item is due
        /// </summary>
        public DateTime DueDate { get; set; }
        // TODO: Ask and if necessary, remove. We can assume max points is always 100
        /// <summary>
        /// Maximum points assignable for this work item
        /// </summary>
        public float MaxPoints { get; set; }
        /// <summary>
        /// Numerical weight of this work item
        /// </summary>
        public float Weight { get; set; }
    }
}