using RGSWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace RGSWeb.ViewModels
{
    /// <summary>
    /// View-model to represent a class
    /// </summary>
    public class ClassViewModel
    {
        /// <summary>
        /// The id of the class
        /// </summary>
        /// <example>1s</example>
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
        /// <summary>
        /// The teacher who is going to teach this class
        /// </summary>
        public UserViewModel Teacher { get; set; }

        /// <summary>
        /// Creates a ClassViewModel for the given class
        /// </summary>
        /// <param name="class">Class to create view-model for</param>
        public ClassViewModel(Class @class)
        {
            if(@class != null)
            {
                Id = @class.Id;
                Title = @class.Title;
                Prefix = @class.Prefix;
                CourseNumber = @class.CourseNumber;
                Section = @class.Section;
                GradeDistribution = @class.GradeDistribution;
                Teacher = new UserViewModel(@class.Teacher);
            }
        }
    }
}