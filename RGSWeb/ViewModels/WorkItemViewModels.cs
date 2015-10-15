using System;

namespace RGSWeb.Models
{
    /// <summary>
    /// A model whose details are used to create a WorkItem
    /// </summary>
    public class CreateWorkItemViewModel
    {
        /// <summary>
        /// The title of the WorkItem
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// A short description of the WorkItem
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Date by which the WorkItem is due
        /// </summary>
        public DateTime DueDate { get; set; }
        /// <summary>
        /// Id of the instructor for the class
        /// </summary>
        public string TeacherUserName { get; set; }
        /// <summary>
        /// Maximum points assignable for this WorkItem
        /// </summary>
        public float MaxPoints { get; set; }
        /// <summary>
        /// The id of the class to which the WorkItem should belong
        /// </summary>
        public int ClassId { get; set; }
        /// <summary>
        /// The type of this WorkItem (e.g. Project, Exam...)
        /// </summary>
        public WorkItemType Type { get; set; }
    }

    /// <summary>
    /// A model whose detail are used to update a WorkItem
    /// </summary>
    public class UpdateWorkItemViewModel
    {
        /// <summary>
        /// Id of the WorkItem that is being updated
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The title of the WorkItem
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// A short description of the WorkItem
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Date by which the WorkItem is due
        /// </summary>
        public DateTime DueDate { get; set; }
        /// <summary>
        /// Maximum points assignable for this WorkItem
        /// </summary>
        public float MaxPoints { get; set; }
        /// <summary>
        /// The type of this WorkItem (e.g. Project, Exam...)
        /// </summary>
        public WorkItemType Type { get; set; }
    }
}