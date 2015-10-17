using Microsoft.AspNet.Identity.EntityFramework;
using RGSWeb.Models;
using RGSWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace RGSWeb.Managers
{
    /// <summary>
    /// Manages all class related actions - creation, update, deletion...
    /// </summary>
    public class ClassManager
    {
        private ApplicationDbContext _db;
        private ApplicationUserManager _userManager;

        private const string teacherRole = "Teacher";
        private const string studentRole = "Student";

        /// <summary>
        /// Creates a new ClassManager with the given Database context
        /// </summary>
        /// <param name="db">The Database context to use</param>
        public ClassManager(ApplicationDbContext db)
        {
            _db = db;
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_db));
        }

        /// <summary>
        /// Creates a new ClassManager with the given Database context and UserManager
        /// </summary>
        /// <param name="db">The Database context to use</param>
        /// <param name="userManager">The UserManager to use</param>
        /// <remarks>The UserManager must have been created from db</remarks>
        public ClassManager(ApplicationDbContext db, ApplicationUserManager userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns a list of classes for the user
        /// </summary>
        /// <param name="user"></param>
        /// <remarks>If the user is a teacher, returns classes taught by the teacher.
        /// If the user is a student, returns classes enrolled in by the student
        /// (both pending and accepted).</remarks>
        public async Task<IEnumerable<Class>> GetUserClasses(ApplicationUser user)
        {
            IQueryable<Class> classes = null;
            if(await _userManager.IsInRoleAsync(user.Id, studentRole))
            {
                classes = _db.Enrollments.Where(e => e.Student.Email == user.Email).Select(e => e.Class).Include(c => c.Teacher);
            }

            else if(await _userManager.IsInRoleAsync(user.Id, teacherRole))
            {
                classes = _db.Classes.Where(@class => @class.Teacher.Email == user.Email).Include(c => c.Teacher);
            }

            return classes;
        }

        /// <summary>
        /// Returns the class with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Null if class is not found</returns>
        public async Task<Class> GetClassById(int id)
        {
            return await _db.Classes.Where(c => c.Id == id).Include(c => c.Teacher).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new class from the given binding model
        /// </summary>
        /// <param name="classBindingModel">Model containing the details of the class
        /// to create</param>
        /// <returns>Null if teacher could not be found. Otherwise, returns the 
        /// created class</returns>
        public async Task<Class> CreateClass(CreateClassBindingModel classBindingModel)
        {
            var teacher = await _userManager.FindByNameAsync(classBindingModel.TeacherUserName);

            if(teacher == null)
            {
                return null;
            }

            Class @class = new Class
            {
                Title = classBindingModel.Title,
                Prefix = classBindingModel.Prefix,
                CourseNumber = classBindingModel.CourseNumber,
                Section = classBindingModel.Section,
                Teacher = teacher,
                GradeDistribution = classBindingModel.GradeDistribution
            };

            _db.Classes.Add(@class);
            await _db.SaveChangesAsync();
            return @class;
        }

        /// <summary>
        /// Updates a class using details in the given model
        /// </summary>
        /// <param name="ucbm">Model containing details of the class to update</param>
        public async Task UpdateClass(UpdateClassBindingModel ucbm)
        {
            // Find the class and update its properties
            var @class = await _db.Classes.FindAsync(ucbm.Id);
            if(@class == null)
            {
                throw new Exception("No class with id: " + ucbm.Id);
            }

            // Update attributes
            @class.Title = ucbm.Title;
            @class.Prefix = ucbm.Prefix;
            @class.CourseNumber = ucbm.CourseNumber;
            @class.Section = ucbm.Section;
            @class.GradeDistribution = ucbm.GradeDistribution;

            _db.Entry(@class).State = EntityState.Modified;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                // Deleted before update maybe?
                if(!ClassExists(@class.Id))
                {
                    throw new Exception("No class with id: " + ucbm.Id);
                }
            }
        }

        /// <summary>
        /// Deletes the class with the specified id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The class that was deleted</returns>
        public async Task<Class> DeleteClass(int id)
        {
            Class @class = await _db.Classes.FindAsync(id);
            if(@class == null)
            {
                throw new Exception("No class with id: " + id);
            }

            _db.Classes.Remove(@class);
            _db.SaveChanges();

            return @class;
        }

        /// <summary>
        /// Returns all students who have been accepted into a class
        /// </summary>
        /// <param name="class">The class to return students for</param>
        /// <returns></returns>
        public async Task<IEnumerable<ApplicationUser>> GetAcceptedStudents(Class @class)
        {
            return await _db.Enrollments.Where(e => e.Class.Id == @class.Id && e.Pending == false).Select(e => e.Student).ToListAsync();
        }
        /// <summary>
        /// Returns all students who have not been accepted into class (their status is
        ///  pending)
        /// </summary>
        /// <param name="class">The class to return students for</param>
        /// <returns></returns>
        public async Task<IEnumerable<ApplicationUser>> GetPendingStudents(Class @class)
        {
            return await _db.Enrollments.Where(e => e.Class.Id == @class.Id && e.Pending == true).Select(e => e.Student).ToListAsync();
        }

        /// <summary>
        /// Returns all students in a class (both pending and accepted)
        /// </summary>
        /// <param name="class">The class to return students for</param>
        /// <returns></returns>
        public async Task<IEnumerable<ApplicationUser>> GetAllStudents(Class @class)
        {
            return await _db.Enrollments
                .Where(e => e.Class.Id == @class.Id)
                .Select(e => e.Student).ToListAsync();
        }

        /// <summary>
        /// Returns true if a class with the given id exists. Otherwise false
        /// </summary>
        /// <param name="id">Id of the class to search for</param>
        /// <returns></returns>
        public bool ClassExists(int id)
        {
            return _db.Classes.Count(e => e.Id == id) > 0;
        }
    }
}