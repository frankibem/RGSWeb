using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGSWeb.Models
{
    /// <summary>
    ///  Represents a class in the raider grader system
    /// </summary>
    public class Class
    {
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
        /// <summary>
        /// The teacher who is going to teach this class
        /// </summary>
        public ApplicationUser Teacher { get; set; }
    }

    public class CreateClassBindingModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Prefix { get; set; }
        [Required]
        [Display(Name = "Course number")]
        public short CourseNumber { get; set; }
        [Required]
        public short Section { get; set; }
        [Required]
        [Display(Name = "Teacher username")]
        [DataType(DataType.EmailAddress)]
        public string TeacherUserName { get; set; }
        [Required]
        [Display(Name = "Grade Distribution")]
        public GradeDistribution GradeDistribution { get; set; }
    }

    public class UpdateClassBindingModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Prefix { get; set; }
        [Required]
        public short CourseNumber { get; set; }
        [Required]
        public short Section { get; set; }
        [Required]
        public GradeDistribution GradeDistribution { get; set; }
    }
}