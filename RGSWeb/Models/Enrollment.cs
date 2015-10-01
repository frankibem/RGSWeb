namespace RGSWeb.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public Class Class { get; set; }
        public ApplicationUser Student { get; set; }
    }
}
