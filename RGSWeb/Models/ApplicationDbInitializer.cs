using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace RGSWeb.Models
{
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        private ApplicationUserManager UserManager;
        private RoleManager<IdentityRole> RoleManager;
        private string password = "SuperSecurePassword100";

        protected override void Seed(ApplicationDbContext context)
        {
            UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Create roles if they don't exist
            string[] roles = { "Admin", "Teacher", "Student" };
            foreach(var role in roles)
            {
                if(!RoleManager.RoleExists(role))
                {
                    RoleManager.Create(new IdentityRole(role));
                }
            }

            // Create admin account if it doesn't exist
            string userName = "admin@raidergrader.com";

            var user = new ApplicationUser();
            user.UserName = userName;
            var adminResult = UserManager.Create(user, password);

            if(adminResult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, roles[0]);
            }

            // Create some teachers
            ApplicationUser[] teachers = new ApplicationUser[]
            {
                new ApplicationUser() { UserName = "susanmengel", FirstName = "Mengel", LastName = "Susan" },
                new ApplicationUser() { UserName = "michaelgelfond", FirstName = "Michael", LastName = "Gelfond" }
            };
            CreateUsersAndAddToRole(teachers, roles[1]);

            // Create some students
            ApplicationUser[] students = new ApplicationUser[]
            {
                new ApplicationUser() { UserName = "frankibem", FirstName = "Frank", LastName = "Ibem" },
                new ApplicationUser() { UserName = "joshuahernandez", FirstName = "Joshua", LastName = "Hernandez" },
                new ApplicationUser() { UserName = "laurenjoness", FirstName = "Lauren", LastName = "Joness" },
                new ApplicationUser() { UserName = "clairegray", FirstName = "Claire", LastName = "Gray" },
                new ApplicationUser() { UserName = "norisrogers", FirstName = "Noris", LastName = "Rogers" },
                new ApplicationUser() { UserName = "michaelarroyo", FirstName = "Michael", LastName = "Arroyo" }
            };
            CreateUsersAndAddToRole(students, roles[2]);

            // Create some classes
            Class[] classes = new Class[]
            {
                new Class() { Title = "Software Engineering I", Prefix = "CS", CourseNumber = 3375, Section = 1, Teacher = teachers[0] },
                new Class() { Title = "Theory of Automata", Prefix = "CS", CourseNumber = 3350, Section = 2, Teacher = teachers[1] }
            };
            context.Classes.AddRange(classes);
            context.SaveChanges();

            // Enroll the students
            List<Enrollment> enrollments = new List<Enrollment>();
            foreach(var student in students)
            {
                enrollments.Add(new Enrollment() { Student = student, Class = classes[0] });
            }
            enrollments.Add(new Enrollment() { Student = students[1], Class = classes[1] });
            enrollments.Add(new Enrollment() { Student = students[3], Class = classes[1] });
            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();

            // Create some work items
            WorkItem[] workItems = new WorkItem[]
            {
                new WorkItem() { Title = "Do great stuff", AssignedBy = teachers[1], DueDate = new DateTime(2015, 02, 14), MaxPoints = 100, Description = "Make humans everywhere proud. Build a robot, find sasquatch, go to space..", Class = classes[1] },
                new WorkItem() { Title = "Prepare presentation for Iteration I", AssignedBy = teachers[0], DueDate = new DateTime(2015, 09, 11), MaxPoints = 100, Description = "Be ready, get some good use cases, add some pictures in there, present stuff.", Class = classes[0] }
            };
            context.WorkItems.AddRange(workItems);
            context.SaveChanges();


            base.Seed(context);
        }

        private void CreateUsersAndAddToRole(ApplicationUser[] users, string role)
        {
            for(int i = 0; i < users.Length; i++)
            {
                UserManager.Create(users[i], password);
                UserManager.AddToRole(users[i].Id, role);
            }
        }
    }
}