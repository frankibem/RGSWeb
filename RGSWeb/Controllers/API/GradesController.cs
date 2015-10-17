using Microsoft.AspNet.Identity.EntityFramework;
using RGSWeb.Managers;
using RGSWeb.Models;
using RGSWeb.ViewModels;
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
    /// API controller for grade related actions
    /// </summary>
    [Authorize]
    public class GradesController : ApiController
    {
        private ApplicationDbContext _db;
        private GradeManager _gradeManager;
        private ClassManager _classManager;

        /// <summary>
        /// Creates a new default GradesController
        /// </summary>
        public GradesController()
        {
            _db = new ApplicationDbContext();
            _gradeManager = new GradeManager(_db);
            _classManager = new ClassManager(_db);
        }


        /// <summary>
        /// Returns the grades of all students in a class
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<StudentViewModel>> GetGradesForClass(int classId)
        {
            var @class = await _db.Classes.FindAsync(classId);
            if(@class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + classId));
            }

            var students = await _classManager.GetAcceptedStudents(@class);

            var result = new List<StudentViewModel>();
            foreach(var student in students)
            {
                var viewModel = new StudentViewModel(student);
                viewModel.Grade = await _gradeManager.GetStudentGradeAsync(student, @class);
                result.Add(viewModel);
            }

            return result;
        }

        /// <summary>
        /// Returns a students grade in a class
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [ResponseType(typeof(StudentViewModel))]
        public async Task<IHttpActionResult> GetStudentsGrade(string userName, int classId)
        {
            if(userName == null)
            {
                return BadRequest("student parameter must not be empty");
            }

            var @class = await _db.Classes.FindAsync(classId);
            ApplicationUserManager manager = new ApplicationUserManager(new UserStore<ApplicationUser>(_db));
            var student = await manager.FindByEmailAsync(userName);

            if(@class == null || student == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Could not match parameters to records"));
            }

            var result = new StudentViewModel(student);
            result.Grade = await _gradeManager.GetStudentGradeAsync(student, @class);

            return Ok(result);
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