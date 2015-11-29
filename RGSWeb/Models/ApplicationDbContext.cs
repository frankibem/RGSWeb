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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure tables to cascade on delete
            modelBuilder.Entity<Announcement>()
                .HasRequired(a => a.Class)
                .WithMany(c => c.Announcements)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ScoreUnit>()
                .HasRequired(su => su.WorkItem)
                .WithMany(wi => wi.ScoreUnits)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ScoreUnit>()
                .HasRequired(su => su.Student)
                .WithMany()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<WorkItem>()
                .HasRequired(wi => wi.Class)
                .WithMany(c => c.WorkItems)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Enrollment>()
                .HasRequired(e => e.Class)
                .WithMany(c => c.Enrollments)
                .WillCascadeOnDelete(true);

            //-----------------------------------
            //modelBuilder.Entity<Enrollment>()
            //    .HasRequired(e => e.Student)
            //    .WithOptional()
            //    .WillCascadeOnDelete();

            //modelBuilder.Entity<Class>()
            //    .HasRequired(c => c.Teacher)
            //    .WithOptional()
            //    .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Class> Classes { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<ScoreUnit> ScoreUnits { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
    }
}