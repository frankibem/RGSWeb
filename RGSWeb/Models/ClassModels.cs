using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGSWeb.Models
{
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