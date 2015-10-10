using Microsoft.AspNet.Identity.Owin;
using RGSWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RGSWeb.Controllers
{
    [Authorize]
    public class StudentsController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        /// <summary>
        /// Returns a list of all students in a class
        /// </summary>
        /// <param name="classId">Id of the class</param>
        [ResponseType(typeof(IEnumerable<UserResultView>))]
        public async Task<IHttpActionResult> GetStudents(int classId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @class = await _db.Classes.FindAsync(classId);
            if(@class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + classId));
            }

            var result = _db.Enrollments.Where(e => e.Class.Id == @class.Id).Select(e => new UserResultView()
            {
                Email = e.Student.Email,
                FirstName = e.Student.FirstName,
                LastName = e.Student.LastName
            });
            return Ok(result);
        }

        /// <summary>
        /// Enrolls a student in a class
        /// </summary>
        /// <param name="enroll">Contains the student Id and class Id</param>
        [HttpPost]
        [ResponseType(typeof(Enrollment))]
        public async Task<IHttpActionResult> EnrollStudent(EnrollmentBindingModel enroll)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure that the student and the class exist
            var student = await UserManager.FindByNameAsync(enroll.StudentUserName);
            var @class = await _db.Classes.FindAsync(enroll.ClassId);
            if(student == null || @class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    string.Format("Could not match student:{0} or class:{1} to existing records", enroll.StudentUserName, enroll.ClassId)));
            }

            // Check that the student is not already enrolled
            var status = (from enrollment in _db.Enrollments
                          where enrollment.Student.Id == student.Id && enrollment.Class.Id == @class.Id
                          select enrollment).FirstOrDefault();

            if(status != null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Conflict, "Student is already enrolled in class"));
            }

            // Enroll the student
            Enrollment newEnroll = new Enrollment { Class = @class, Student = student };
            _db.Enrollments.Add(newEnroll);
            await _db.SaveChangesAsync();

            return Ok(newEnroll);
        }

        /// <summary>
        /// Removes a student from a class.
        /// </summary>
        /// <param name="enroll">Contains the student Id and class Id</param>
        [HttpDelete]
        public async Task<IHttpActionResult> UnEnrollStudent(EnrollmentBindingModel enroll)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var student = await UserManager.FindByNameAsync(enroll.StudentUserName);
            var @class = await _db.Classes.FindAsync(enroll.ClassId);

            if(student == null || @class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    string.Format("Could not match student:{0} or class:{1} to existing records", enroll.StudentUserName, enroll.ClassId)));
            }

            // Ensure that the student is enrolled before removing
            var status = _db.Enrollments.Where(e => e.Class.Id == enroll.ClassId && e.Student.UserName == enroll.StudentUserName).FirstOrDefault();
            if(status == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    string.Format("{0} is not enrolled in class:{0}", enroll.StudentUserName, enroll.ClassId)));
            }

            // Delete all student related data
            var scoreUnits = _db.ScoreUnits.Where(sc => sc.Student.UserName == enroll.StudentUserName);
            _db.ScoreUnits.RemoveRange(scoreUnits);
            _db.Enrollments.Remove(status);

            await _db.SaveChangesAsync();
            return Ok(status);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}