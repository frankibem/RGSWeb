using Microsoft.AspNet.Identity.EntityFramework;
using RGSWeb.Managers;
using RGSWeb.Models;
using RGSWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RGSWeb.Controllers.API
{
    /// <summary>
    /// API controller for enrollment related actions
    /// </summary>
    [Authorize]
    public class EnrollmentsController : ApiController
    {
        private ApplicationDbContext _db;
        private EnrollmentManager _enrollmentManager;
        private ApplicationUserManager _userManager;

        /// <summary>
        /// Create a new default EnrollmentsController
        /// </summary>
        public EnrollmentsController()
        {
            _db = new ApplicationDbContext();
            _enrollmentManager = new EnrollmentManager(_db);
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_db));
        }

        /// <summary>
        /// Return a list of enrollments in a class
        /// </summary>
        /// <param name="classId">Id of the class</param>
        [ResponseType(typeof(IEnumerable<EnrollmentViewModel>))]
        public async Task<IHttpActionResult> GetEnrollments(int classId)
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

            var enrollments = await _enrollmentManager.GetAllEnrollmentsForClass(@class);
            var result = await ConvertToViewModel(enrollments);

            return Ok(result);
        }

        /// <summary>
        /// Returns a list of all enrollments by a student (pending and not)
        /// </summary>
        /// <param name="studentUserName">Username of student</param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<EnrollmentViewModel>))]
        public async Task<IHttpActionResult> GetEnrollments(string studentUserName)
        {
            if(studentUserName == null)
            {
                return BadRequest("Must supply a value for student username");
            }

            var student = await _userManager.FindByEmailAsync(studentUserName);
            var enrollments = await _enrollmentManager.GetStudentEnrollments(student);

            var result = await ConvertToViewModel(enrollments);
            return Ok(result);
        }

        /// <summary>
        /// Converts a list of enrollments to a list of corresponding view-models
        /// </summary>
        /// <param name="enrollments"></param>
        /// <returns></returns>
        private async Task<List<EnrollmentViewModel>> ConvertToViewModel(IEnumerable<Enrollment> enrollments)
        {
            List<EnrollmentViewModel> result = new List<EnrollmentViewModel>();
            var gradeManager = new GradeManager(_db);
            foreach(var enroll in enrollments)
            {
                var enrollvm = new EnrollmentViewModel(enroll);
                if(!enroll.Pending)
                {
                    enrollvm.Grade = await gradeManager.GetStudentGradeAsync(enroll.Student, enroll.Class);
                }
                result.Add(enrollvm);
            }

            return result;
        }

        /// <summary>
        /// Request student enrollment into a class
        /// </summary>
        /// <param name="enroll">Contains the student Id and class Id</param>
        [HttpPost]
        [ResponseType(typeof(EnrollmentViewModel))]
        public async Task<IHttpActionResult> RequestEnrollment(EnrollmentBindingModel enroll)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure that the student and the class exist
            var student = await _userManager.FindByNameAsync(enroll.StudentUserName);
            var @class = await _db.Classes.FindAsync(enroll.ClassId);
            if(student == null || @class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    string.Format("Could not match student:{0} or class:{1} to existing records", enroll.StudentUserName, enroll.ClassId)));
            }

            var newEnroll = await _enrollmentManager.RequestEnrollment(@class, student);
            if(newEnroll == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Conflict, "Enrollment already requested, or student already enrolled in class"));
            }

            return Ok(new EnrollmentViewModel(newEnroll));
        }

        /// <summary>
        /// Accept/Reject a student's enrollment into a class
        /// </summary>
        /// <param name="models">List of model containing the details of the enrollment</param>
        [HttpPut]
        public async Task<IHttpActionResult> AcceptStudentEnrollment(List<EnrollmentBindingModel> models)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _enrollmentManager.AcceptEnrollment(models);
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Drops a student from a class
        /// </summary>
        [HttpDelete]
        [ResponseType(typeof(EnrollmentViewModel))]
        public async Task<IHttpActionResult> DropStudent(EnrollmentBindingModel enroll)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure that the student and the class exist
            var student = await _userManager.FindByNameAsync(enroll.StudentUserName);
            var @class = await _db.Classes.FindAsync(enroll.ClassId);
            if(student == null || @class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    string.Format("Could not match student:{0} or class:{1} to existing records", enroll.StudentUserName, enroll.ClassId)));
            }

            var result = await _enrollmentManager.DropStudent(@class, student);
            if(result == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    string.Format("Student: {0} is not enrolled in class: {1}", enroll.StudentUserName, enroll.ClassId)));
            }

            return Ok(new EnrollmentViewModel(result));
        }
    }
}