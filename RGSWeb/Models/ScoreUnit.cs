namespace RGSWeb.Models
{
    public class ScoreUnit
    {
        public int Id { get; set; }
        public WorkItem WorkItem { get; set; }
        public ApplicationUser Student { get; set; }
        public float? Grade { get; set; }
    }
}
