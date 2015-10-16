using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using RGSWeb.Managers;
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
        private ClassManager _classManager;

        public ClassManager ClassManager
        {
            get
            {
                if(_classManager == null)
                {
                    _classManager = new ClassManager(_db, UserManager);
                }
                return _classManager;
            }
            set { _classManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? new ApplicationUserManager(new UserStore<ApplicationUser>(_db)); }
            set { _userManager = value; }
        }

        /// <summary>
        /// Returns a list of all students in a class
        /// </summary>
        /// <param name="classId">Id of the class</param>
        /// <param name="state">The state of the students to obtain e.g. All, PendingOnly, AcceptedOnly</param>
        [ResponseType(typeof(IEnumerable<UserResultView>))]
        public async Task<IHttpActionResult> GetStudents(int classId, EnrollmentState? state)
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

            IEnumerable<ApplicationUser> users = null;
            if(!state.HasValue || state == EnrollmentState.All)
            {
                users = await ClassManager.GetAllStudents(@class);
            }
            else if(state == EnrollmentState.Accepted)
            {
                users = await ClassManager.GetAcceptedStudents(@class);
            }
            else if(state == EnrollmentState.Pending)
            {
                users = await ClassManager.GetPendingStudents(@class);
            }

            var result = users.Select(u => new UserResultView(u));
            return Ok(result);
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

            var enrollment = await ClassManager.RemoveStudentEnrollment(@class, student);
            if(enrollment == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    string.Format("{0} is not enrolled in class:{0}", enroll.StudentUserName, enroll.ClassId)));
            }

            return Ok(enrollment);
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