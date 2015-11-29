using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGSWeb.Models
{
    /// <summary>
    /// Class to represent the weight distribution of the different types of workitems
    /// </summary>
    public class GradeDistribution
    {
        public float Exam { get; set; }
        public float Quiz { get; set; }
        public float Project { get; set; }
        public float Homework { get; set; }
        public float Other { get; set; }
    }

    /// <summary>
    ///  Represents a class in the raider grader system
    /// </summary>
    public class Class
    {
        public Class()
        {
            this.Announcements = new HashSet<Announcement>();
            this.WorkItems = new HashSet<WorkItem>();
            this.Enrollments = new HashSet<Enrollment>();
        }

        /// <summary>
        /// The id of the class
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The title of the Class
        /// </summary>
        /// <example>Advanced Biology</example>
        public string Title { get; set; }
        /// <summary>
        /// The short prefix for the class
        /// </summary>
        /// <example>BIO</example>
        public string Prefix { get; set; }
        /// <summary>
        /// The course number
        /// </summary>
        /// <example>3375</example>
        [Display(Name = "Course Number")]
        public short CourseNumber { get; set; }
        /// <summary>
        /// The section of the class
        /// </summary>
        /// <example>1</example>
        public short Section { get; set; }
        /// <summary>
        /// The distribution of grades for this class for the different types
        /// of work items
        /// </summary>
        public GradeDistribution GradeDistribution { get; set; }

        // Navigation Properties
        /// <summary>
        /// The teacher who is going to teach this class
        /// </summary>
        public virtual ApplicationUser Teacher { get; set; }
        public virtual ICollection<Announcement> Announcements { get; private set; }
        public virtual ICollection<WorkItem> WorkItems { get; private set; }
        public virtual ICollection<Enrollment> Enrollments { get; private set; }
    }

    /// <summary>
    /// A class to represent student enrollment in a "class"
    /// </summary>
    public class Enrollment
    {
        /// <summary>
        /// The Id of the enrollment
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Pending is true if the student has not been accepted to a class
        /// and false otherwise
        /// </summary>
        public bool Pending { get; set; }

        // Navigation properties
        /// <summary>
        /// The Class for which the enrollment applies
        /// </summary>
        public virtual Class Class { get; set; }
        /// <summary>
        /// The student for which the enrollment applies
        /// </summary>
        public virtual ApplicationUser Student { get; set; }
    }

    /// <summary>
    /// Class to represent the different types of workitems
    /// </summary>
    public enum WorkItemType
    {
        Exam,
        Quiz,
        Homework,
        Project,
        Other
    }

    /// <summary>
    /// Model to represent a class work-item
    /// </summary>
    public class WorkItem
    {
        public WorkItem()
        {
            this.ScoreUnits = new HashSet<ScoreUnit>();
        }

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

        // Navigation properties
        /// <summary>
        /// The Class to which the WorkItem belongs
        /// </summary>
        public virtual Class Class { get; set; }
        public virtual ICollection<ScoreUnit> ScoreUnits { get; private set; }
    }

    /// <summary>
    /// Class to represent a class announcement
    /// </summary>
    public class Announcement
    {
        /// <summary>
        /// Id of the announcment
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Title of the announcement
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Description of the announcement
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Creation time of the assignment
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Creation date")]
        public DateTime CreatedOn { get; set; }

        // Navigation property
        /// <summary>
        /// Class for which the announcement was created
        /// </summary>
        public virtual Class Class { get; set; }
    }

    public class ScoreUnit
    {
        public int Id { get; set; }
        public float? Grade { get; set; }

        // Navigation properties
        public virtual WorkItem WorkItem { get; set; }
        public virtual ApplicationUser Student { get; set; }
    }
}