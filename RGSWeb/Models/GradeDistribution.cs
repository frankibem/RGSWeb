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
}