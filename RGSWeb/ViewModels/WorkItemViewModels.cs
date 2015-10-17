using RGSWeb.Models;
using System;

namespace RGSWeb.ViewModels
{
    /// <summary>
    /// View-model to represent a WorkItem
    /// </summary>
    public class WorkItemViewModel
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
        public DateTime DueDate { get; set; }
        /// <summary>
        /// Maximum points attainable for this WorkItem
        /// </summary>
        public float MaxPoints { get; set; }
        /// <summary>
        /// The type of the WorkItem
        /// </summary>
        /// <example>Exam</example>
        public WorkItemType Type { get; set; }
        /// <summary>
        /// The Class to which the WorkItem belongs
        /// </summary>
        public ClassViewModel Class { get; set; }

        /// <summary>
        /// Creates a view-model for the given WorkItem
        /// </summary>
        /// <param name="workItem">Model after which the view-model is created</param>
        public WorkItemViewModel(WorkItem workItem)
        {
            Id = workItem.Id;
            Title = workItem.Title;
            Description = workItem.Description;
            DueDate = workItem.DueDate;
            MaxPoints = workItem.MaxPoints;
            Type = workItem.Type;
            Class = new ClassViewModel(workItem.Class);
        }
    }

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