using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace RGSWeb.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Class> Classes { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<ScoreUnit> ScoreUnits { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
    }
}
