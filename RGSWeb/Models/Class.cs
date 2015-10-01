namespace RGSWeb.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Prefix { get; set; }
        public short CourseNumber { get; set; }
        public short Section { get; set; }
        public ApplicationUser Teacher { get; set; }
    }
}
