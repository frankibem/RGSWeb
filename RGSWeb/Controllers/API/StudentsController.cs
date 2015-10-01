using Microsoft.AspNet.Identity.EntityFramework;
using RGSWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace RGSWeb.Controllers
{
    public class StudentsController : ApiController
    {
        private ApplicationDbContext db;
        private ApplicationUserManager userManager;

        public StudentsController()
        {
            db = new ApplicationDbContext();
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
        }

        /// <summary>
        /// Returns a list of all students in a class
        /// </summary>
        /// <param name="classId">Id of the class</param>
        public async Task<IHttpActionResult> GetStudents(int classId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @class = await db.Classes.FindAsync(classId);
            if(@class == null)
            {
                return NotFound();
            }

            var result = db.Enrollments.Where(e => e.Class.Id == @class.Id).Select(e => new UserResultView()
            {
                Id = e.Student.Id,
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
        public async Task<IHttpActionResult> EnrollStudent(EnrollmentBindingModel enroll)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure that the student and the class exist
            var student = await userManager.FindByIdAsync(enroll.StudentId);
            var @class = await db.Classes.FindAsync(enroll.ClassId);
            if(student == null || @class == null)
            {
                return BadRequest("Incorrect student id or class id");
            }

            // Check that the student is not already enrolled
            var status = (from enrollment in db.Enrollments
                          where enrollment.Student.Id == student.Id && enrollment.Class.Id == @class.Id
                          select enrollment).FirstOrDefault();
            if(status != null)
            {
                return BadRequest("Student is already in class");
            }

            // Enroll the student
            Enrollment newEnroll = new Enrollment { Class = @class, Student = student };
            db.Enrollments.Add(newEnroll);
            await db.SaveChangesAsync();

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

            var student = await userManager.FindByIdAsync(enroll.StudentId);
            var @class = await db.Classes.FindAsync(enroll.ClassId);
            if(student == null || @class == null)
            {
                return BadRequest("Incorrect student id or class id");
            }

            // Ensure that the student is enrolled before removing
            var status = db.Enrollments.Where(e => e.Class.Id == enroll.ClassId && e.Student.Id == enroll.StudentId).FirstOrDefault();
            if(status == null)
            {
                return NotFound();
            }

            // Delete all student related data
            var scoreUnits = db.ScoreUnits.Where(sc => sc.Student.Id == enroll.StudentId);
            db.ScoreUnits.RemoveRange(scoreUnits);
            db.Enrollments.Remove(status);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}