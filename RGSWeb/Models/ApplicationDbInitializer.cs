using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace RGSWeb.Models
{
    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        private ApplicationUserManager UserManager;
        private RoleManager<IdentityRole> RoleManager;
        private string adminPwd = System.Configuration.ConfigurationManager.AppSettings["adminpwd"];
        private string password = System.Configuration.ConfigurationManager.AppSettings["testpwd"];

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
            var admin = new ApplicationUser() { UserName = "admin@rgs.com", Email = "admin@rgs.com", FirstName = "Admin" };
            var adminResult = UserManager.Create(admin, adminPwd);

            if(adminResult.Succeeded)
            {
                var result = UserManager.AddToRole(admin.Id, roles[0]);
            }

            // Create some teachers
            ApplicationUser[] teachers = new ApplicationUser[]
            {
                new ApplicationUser() { UserName = "susanmengel@rgs.com", FirstName = "Susan", LastName = "Mengel" , Email = "susanmengel@rgs.com"},
                new ApplicationUser() { UserName = "teacher@rgs.com", FirstName = "Michael", LastName = "Gelfond", Email = "teacher@rgs.com" }
            };
            CreateUsersAndAddToRole(teachers, roles[1]);

            // Create some students
            ApplicationUser[] students = new ApplicationUser[]
            {
                new ApplicationUser() { UserName = "student@rgs.com", FirstName = "John", LastName = "Doe", Email = "student@rgs.com" },
                new ApplicationUser() { UserName = "joshuahernandez@rgs.com", FirstName = "Joshua", LastName = "Hernandez", Email = "joshuahernandez@rgs.com" },
                new ApplicationUser() { UserName = "laurenjoness@rgs.com", FirstName = "Lauren", LastName = "Joness", Email = "laurenjoness@rgs.com" },
                new ApplicationUser() { UserName = "clairegray@rgs.com", FirstName = "Claire", LastName = "Gray", Email = "clairegray@rgs.com" },
                new ApplicationUser() { UserName = "norisrogers@rgs.com", FirstName = "Noris", LastName = "Rogers", Email = "norisrogers@rgs.com" },
                new ApplicationUser() { UserName = "michaelarroyo@rgs.com", FirstName = "Michael", LastName = "Arroyo", Email = "michaelarroyo@rgs.com" }
            };
            CreateUsersAndAddToRole(students, roles[2]);

            // Create some classes
            Class[] classes = new Class[]
            {
                new Class() { Title = "Software Engineering I", Prefix = "CS", CourseNumber = 3375, Section = 1, Teacher = teachers[0],
                GradeDistribution = new GradeDistribution() { Exam = 40, Homework = 20, Quiz = 15, Project = 15, Other = 10 } },
                new Class() { Title = "Theory of Automata", Prefix = "CS", CourseNumber = 3350, Section = 2, Teacher = teachers[1],
                GradeDistribution = new GradeDistribution() { Exam = 30, Homework = 20, Quiz = 15, Project = 20, Other = 5 }  }
            };
            context.Classes.AddRange(classes);
            context.SaveChanges();

            // Enroll the students
            List<Enrollment> enrollments = new List<Enrollment>();
            foreach(var student in students)
            {
                enrollments.Add(new Enrollment() { Student = student, Class = classes[0], Pending = false });
            }

            enrollments.Add(new Enrollment() { Student = students[1], Class = classes[1], Pending = true });
            enrollments.Add(new Enrollment() { Student = students[3], Class = classes[1], Pending = false });
            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();

            // Create some work items
            WorkItem[] workItems = new WorkItem[]
            {
                new WorkItem() { Title = "Do great stuff", DueDate = new DateTime(2015, 02, 14), MaxPoints = 100, Description = "Make humans everywhere proud. Build a robot, find sasquatch, go to space...", Class = classes[1] },
                new WorkItem() { Title = "Prepare presentation for Iteration I", DueDate = new DateTime(2015, 09, 11), MaxPoints = 100, Description = "Be ready, get some good use cases, add some pictures in there, present stuff.", Class = classes[0] }
            };
            context.WorkItems.AddRange(workItems);

            Announcement[] announcements = new Announcement[]
            {
                new Announcement()
                {
                    Title = "First day of free stuff on campus", Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", Class = classes[0], CreatedOn = DateTime.Now },
                new Announcement()
                {
                    Title = "Too good to be true. Good stuff for free", Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", Class = classes[1],
                    CreatedOn = DateTime.Now
                }
            };

            context.Announcements.AddRange(announcements);

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