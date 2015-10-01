﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGSWeb.Models;
using System;
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
    // TODO: Add [Authorize] when login is implemented
    public class ClassesController : ApiController
    {
        private ApplicationDbContext db;
        private ApplicationUserManager userManager;
        private string teacherRole = "Teacher";
        private string studentRole = "Student";

        public ClassesController()
        {
            db = new ApplicationDbContext();
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
        }

        // TODO: Add [Authorize(Roles = "Admin")] when login is implemented
        /// <summary>
        /// Returns all classes
        /// </summary>
        public IHttpActionResult Get()
        {
            return Ok(db.Classes);
        }

        /// <summary>
        /// If userId is the id of a student, returns a list of all classes that student is enrolled in.
        /// Otherwise if it is that of a teacher, returns a list of all classes taught by the teacher
        /// </summary>
        /// <param name="userId">Id of a teacher/student</param>
        // TODO: Filter returned result to contain only pertinent information
        public async Task<IHttpActionResult> GetClasses(string userId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                throw new HttpRequestException("No user with id: " + userId);
            }

            else if(await userManager.IsInRoleAsync(userId, studentRole))
            {
                var result = from enrollment in db.Enrollments
                             where enrollment.Student.Id == userId
                             select enrollment.Class;
                return Ok(result);
            }

            else if(await userManager.IsInRoleAsync(userId, teacherRole))
            {
                return Ok(db.Classes.Where(@class => @class.Teacher.Id == userId));
            }

            return BadRequest("User Id not a valid student or teacher");
        }

        // POST: api/Classes
        /// <summary>
        /// Creates a new class
        /// </summary>
        [ResponseType(typeof(Class))]
        public async Task<IHttpActionResult> PostClass(CreateClassBindingModel classvm)
        {
            var teacher = await userManager.FindByIdAsync(classvm.TeacherId);
            if(!ModelState.IsValid || teacher == null)
            {
                return BadRequest(ModelState);
            }

            Class @class = new Class
            {
                Title = classvm.Title,
                CourseNumber = classvm.CourseNumber,
                Section = classvm.Section,
                Teacher = teacher
            };

            db.Classes.Add(@class);
            await db.SaveChangesAsync();
            return CreatedAtRoute("DefaultApi", new { id = @class.Id }, @class);
        }

        // PUT: api/Classes
        /// <summary>
        /// Updates the details of a class
        /// </summary>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutClass(UpdateClassBindingModel uclassvm)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the class and update its properties
            var @class = db.Classes.Find(uclassvm.Id);
            if(@class == null)
            {
                return NotFound();
            }

            // Update attributes
            @class.Title = uclassvm.Title;
            @class.Prefix = uclassvm.Prefix;
            @class.CourseNumber = uclassvm.CourseNumber;
            @class.Section = uclassvm.Section;

            db.Entry(@class).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                // Deleted before update maybe?
                if(!ClassExists(@class.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
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
            Class @class = db.Classes.Find(id);
            if(@class == null)
            {
                return NotFound();
            }

            db.Classes.Remove(@class);
            db.SaveChanges();

            return Ok(@class);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClassExists(int id)
        {
            return db.Classes.Count(e => e.Id == id) > 0;
        }
    }
}