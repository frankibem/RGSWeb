using Microsoft.AspNet.Identity.EntityFramework;
using RGSWeb.Managers;
using RGSWeb.Models;
using RGSWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Linq;

namespace RGSWeb.Controllers
{
    /// <summary>
    /// API controller for class related actions
    /// </summary>
    [Authorize]
    public class ClassesController : ApiController
    {
        private ApplicationDbContext _db;
        private ApplicationUserManager _userManager;
        private ClassManager _classManager;

        /// <summary>
        /// Creates a new default ClassesController
        /// </summary>
        public ClassesController()
        {
            _db = new ApplicationDbContext();
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_db));
            _classManager = new ClassManager(_db);
        }

        /// <summary>
        /// If userId is the id of a student, returns a list of all classes that student is enrolled in.
        /// If it is that of a teacher, returns a list of all classes taught by the teacher
        /// </summary>
        /// <param name="userName">Id of teacher/student</param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<ClassViewModel>))]
        public async Task<IHttpActionResult> GetClasses(string userName)
        {
            if(!ModelState.IsValid || userName == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(userName);
            if(user == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No user with id: " + userName));
            }

            var classes = await _classManager.GetUserClasses(user);

            var result = classes.Select(@class => new ClassViewModel(@class));
            return Ok(result);
        }

        /// <summary>
        /// Returns the class with the specified id
        /// </summary>
        /// <param name="id">Id of the class to return</param>
        /// <returns></returns>
        public async Task<ClassViewModel> GetClass(int id)
        {
            var result = await _classManager.GetClassById(id);
            if(result == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + id));
            }
            else
            {
                return new ClassViewModel(result);
            }
        }

        // POST: api/Classes
        /// <summary>
        /// Creates a new class
        /// </summary>
        [ResponseType(typeof(ClassViewModel))]
        public async Task<IHttpActionResult> PostClass(CreateClassBindingModel classvm)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @class = await _classManager.CreateClass(classvm);
            if(@class == null)
            {
                return BadRequest(ModelState);
            }
            return CreatedAtRoute("DefaultApi", new { id = @class.Id }, new ClassViewModel(@class));
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

            try
            {
                await _classManager.UpdateClass(uclassvm);
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Classes/5
        /// <summary>
        /// Deletes a class
        /// </summary>
        /// <param name="id">Id of the class to delete</param>
        [ResponseType(typeof(ClassViewModel))]
        public async Task<ClassViewModel> DeleteClass(int id)
        {
            Class @class = await _classManager.DeleteClass(id);
            if(@class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + id));
            }
            return new ClassViewModel(@class);
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