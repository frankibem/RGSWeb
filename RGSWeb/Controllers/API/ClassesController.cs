using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using RGSWeb.Models;
using RGSWeb.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RGSWeb.Controllers
{
    [Authorize]
    public class ClassesController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        private const string teacherRole = "Teacher";
        private const string studentRole = "Student";

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? new ApplicationUserManager(new UserStore<ApplicationUser>(_db)); }
            set { _userManager = value; }
        }

        [NonAction]
        public static ClassViewModel ConvertToClassViewModel(Class klass)
        {
            return new ClassViewModel
            {
                Id = klass.Id,
                Title = klass.Title,
                Prefix = klass.Prefix,
                CourseNumber = klass.CourseNumber,
                Section = klass.Section,
                TeacherName = klass.Teacher.LastName + ", " + klass.Teacher.FirstName
            };
        }

        [ResponseType(typeof(IEnumerable<ClassViewModel>))]
        /// <summary>
        /// If userId is the id of a student, returns a list of all classes that student is enrolled in.
        /// If it is that of a teacher, returns a list of all classes taught by the teacher
        /// </summary>
        /// <param name="userName">Id of a teacher/student</param>
        public async Task<IHttpActionResult> GetClasses(string userName)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await UserManager.FindByNameAsync(userName);
            if(user == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No user with id: " + userName));
            }

            IQueryable<Class> classes = null;
            if(await UserManager.IsInRoleAsync(user.Id, studentRole))
            {
                classes = _db.Enrollments.Where(e => e.Student.UserName == userName).Select(e => e.Class).Include(c => c.Teacher);
            }

            else if(await UserManager.IsInRoleAsync(user.Id, teacherRole))
            {
                classes = _db.Classes.Where(@class => @class.Teacher.UserName == userName).Include(c => c.Teacher);
            }

            List<ClassViewModel> result = new List<ClassViewModel>();
            foreach(Class cl in classes)
            {
                result.Add(ConvertToClassViewModel(cl));
            }

            return Ok(result);
        }

        /// <summary>
        /// Returns the class with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ClassViewModel> GetClass(int id)
        {
            var result = await _db.Classes.Where(c => c.Id == id).Include(c => c.Teacher).FirstOrDefaultAsync();
            if(result == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + id));
            }
            else
            {
                return ConvertToClassViewModel(result);
            }
        }

        // POST: api/Classes
        /// <summary>
        /// Creates a new class
        /// </summary>
        [ResponseType(typeof(Class))]
        public async Task<IHttpActionResult> PostClass(CreateClassBindingModel classvm)
        {
            var teacher = await UserManager.FindByNameAsync(classvm.TeacherUserName);
            if(!ModelState.IsValid || teacher == null)
            {
                return BadRequest(ModelState);
            }

            Class @class = new Class
            {
                Title = classvm.Title,
                Prefix = classvm.Prefix,
                CourseNumber = classvm.CourseNumber,
                Section = classvm.Section,
                Teacher = teacher
            };

            _db.Classes.Add(@class);
            await _db.SaveChangesAsync();
            return CreatedAtRoute("DefaultApi", new { id = @class.Id }, @class);
        }

        // PUT: api/Classes
        /// <summary>
        /// Updates the details of a class
        /// </summary>
        [ResponseType(typeof(HttpStatusCode))]
        public async Task<IHttpActionResult> PutClass(UpdateClassBindingModel uclassvm)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the class and update its properties
            var @class = _db.Classes.Find(uclassvm.Id);
            if(@class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + uclassvm.Id));
            }

            // Update attributes
            @class.Title = uclassvm.Title;
            @class.Prefix = uclassvm.Prefix;
            @class.CourseNumber = uclassvm.CourseNumber;
            @class.Section = uclassvm.Section;

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
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + uclassvm.Id));
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Classes/5
        /// <summary>
        /// Deletes a class
        /// </summary>
        /// <param name="id">Id of the class to delete</param>
        [ResponseType(typeof(Class))]
        public IHttpActionResult DeleteClass(int id)
        {
            Class @class = _db.Classes.Find(id);
            if(@class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + id));
            }

            _db.Classes.Remove(@class);
            _db.SaveChanges();

            return Ok(@class);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClassExists(int id)
        {
            return _db.Classes.Count(e => e.Id == id) > 0;
        }
    }
}