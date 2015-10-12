using System.ComponentModel.DataAnnotations;

namespace RGSWeb.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Prefix { get; set; }
        [Display(Name = "Course Number")]
        public short CourseNumber { get; set; }
        public short Section { get; set; }
        public GradeDistribution GradeDistribution { get; set; }
        public ApplicationUser Teacher { get; set; }
    }
}