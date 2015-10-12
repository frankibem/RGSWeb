using RGSWeb.Models;

namespace RGSWeb.ViewModels
{
    public class ClassViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Prefix { get; set; }
        public short CourseNumber { get; set; }
        public short Section { get; set; }
        public string TeacherName { get; set; }
        public GradeDistribution GradeDistribution { get; set; }
    }
}
