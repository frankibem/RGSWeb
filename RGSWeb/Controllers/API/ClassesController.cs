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

namespace RGSWeb.Controllers
{
    [Authorize]
    public class ClassesController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private ClassManager _classManager;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? new ApplicationUserManager(new UserStore<ApplicationUser>(_db)); }
            set { _userManager = value; }
        }

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

        /// <summary>
        /// If userId is the id of a student, returns a list of all classes that student is enrolled in.
        /// If it is that of a teacher, returns a list of all classes taught by the teacher
        /// </summary>
        /// <param name="userName">Id of teacher/student</param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<ClassViewModel>))]
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

            var classes = await ClassManager.GetUserClasses(user);
            List<ClassViewModel> result = new List<ClassViewModel>();
            foreach(Class cl in classes)
            {
                result.Add(ClassManager.ConvertToClassViewModel(cl));
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
            var result = await ClassManager.GetClassById(id);
            if(result == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + id));
            }
            else
            {
                return ClassManager.ConvertToClassViewModel(result);
            }
        }

        // POST: api/Classes
        /// <summary>
        /// Creates a new class
        /// </summary>
        [ResponseType(typeof(Class))]
        public async Task<IHttpActionResult> PostClass(CreateClassBindingModel classvm)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @class = await ClassManager.CreateClass(classvm);
            if(@class == null)
            {
                return BadRequest(ModelState);
            }
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

            try
            {
                await ClassManager.UpdateClass(uclassvm);
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
        [ResponseType(typeof(Class))]
        public async Task<Class> DeleteClass(int id)
        {
            Class @class = await ClassManager.DeleteClass(id);
            if(@class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + id));
            }
            return @class;
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