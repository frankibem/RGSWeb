using System;

namespace RGSWeb.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public ApplicationUser AssignedBy { get; set; }
        public float MaxPoints { get; set; }
        public float Weight { get; set; }
        public Class Class { get; set; }
    }
}
