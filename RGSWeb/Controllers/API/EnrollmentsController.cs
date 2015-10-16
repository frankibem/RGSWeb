﻿using Microsoft.AspNet.Identity.EntityFramework;
using RGSWeb.Managers;
using RGSWeb.Models;
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
        /// <param name="state">The state of the students to obtain e.g. All, PendingOnly, AcceptedOnly</param>
        [ResponseType(typeof(IEnumerable<Enrollment>))]
        public async Task<IHttpActionResult> GetEnrollments(int classId, EnrollmentState? state)
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

            IEnumerable<Enrollment> enrollments = null;
            if(!state.HasValue || state == EnrollmentState.All)
            {
                enrollments = await _enrollmentManager.GetAllEnrollmentsForClass(@class);
            }
            else if(state == EnrollmentState.Accepted)
            {
                enrollments = await _enrollmentManager.GetAcceptedEnrollmentsForClass(@class);
            }
            else if(state == EnrollmentState.Pending)
            {
                enrollments = await _enrollmentManager.GetPendingEnrollmentsForClass(@class);
            }

            return Ok(enrollments);
        }

        /// <summary>
        /// Request student enrollment into a class
        /// </summary>
        /// <param name="enroll">Contains the student Id and class Id</param>
        [HttpPost]
        [ResponseType(typeof(Enrollment))]
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
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Conflict, "Student is already enrolled in class"));
            }

            return Ok(newEnroll);
        }

        /// <summary>
        /// Accept/Reject a student's enrollment into a class
        /// </summary>
        /// <param name="enroll">Model containing the details of the enrollment</param>
        [HttpPut]
        public async Task<IHttpActionResult> AcceptStudentEnrollment(EnrollmentBindingModel enroll)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure that the enrollment exists
            var enrollment = _db.Enrollments.Where(e => e.Student.UserName == enroll.StudentUserName && e.Class.Id == enroll.ClassId).FirstOrDefault();
            if(enrollment == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Could not match model with records"));
            }

            await _enrollmentManager.AcceptEnrollment(enrollment, enroll.Accept);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Drops a student from a class
        /// </summary>
        [HttpDelete]
        [ResponseType(typeof(Enrollment))]
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

            return Ok(result);
        }
    }
}