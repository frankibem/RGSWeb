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
    /// API controller for announcement related actions
    /// </summary>
    public class AnnouncementsController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private AnnouncementManager _announcementManager;

        /// <summary>
        /// Create a new default AnnouncementsController
        /// </summary>
        public AnnouncementsController()
        {
            _db = new ApplicationDbContext();
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>());
            _announcementManager = new AnnouncementManager(_db);
        }

        /// <summary>
        /// Returns all announcements made in a class
        /// </summary>
        /// <param name="classId">Id of the class</param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetClassAnnouncements(int classId)
        {
            var @class = await _db.Classes.FindAsync(classId);
            if(@class == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No class with id: " + classId));
            }

            return Ok(await _announcementManager.GetAnnouncementsForClass(@class));
        }

        /// <summary>
        /// Update an announcement
        /// </summary>
        [ResponseType(typeof(HttpStatusCode))]
        public async Task<IHttpActionResult> PutAnnouncement(UpdateAnnouncementModel uam)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _announcementManager.UpdateAnnouncement(uam);
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message));
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Create a new announcement
        /// </summary>
        [ResponseType(typeof(Announcement))]
        public async Task<IHttpActionResult> PostAnnouncement(CreateAnnouncementModel cam)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var announcement = await _announcementManager.CreateAnnouncement(cam);
            if(announcement == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Could not match class to existing records"));
            }

            return CreatedAtRoute("DefaultApi", new { id = announcement.Id }, announcement);
        }

        /// <summary>
        /// Delete an announcment by id
        /// </summary>
        /// <param name="id">Id of the announcement to delete</param>
        [ResponseType(typeof(Announcement))]
        public async Task<Announcement> DeleteWorkItem(int id)
        {
            var announcement = await _announcementManager.DeleteAnnouncement(id);
            if(announcement == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No announcement with id: " + id));
            }
            return announcement;
        }
    }
}