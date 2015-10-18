using RGSWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace RGSWeb.Managers
{
    /// <summary>
    /// Manages all announcement related actions - creation, update, deletion ...
    /// </summary>
    public class AnnouncementManager
    {
        private ApplicationDbContext _db;

        /// <summary>
        /// Creates a new AnnouncementManager with the given database context
        /// </summary>
        /// <param name="db"></param>
        public AnnouncementManager(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates a new announcement from the given model
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public async Task<Announcement> CreateAnnouncement(CreateAnnouncementModel cam)
        {
            var @class = await _db.Classes.FindAsync(cam.ClassId);
            if(@class == null)
            {
                return null;
            }

            Announcement announcement = new Announcement
            {
                Title = cam.Title,
                CreatedOn = DateTime.UtcNow,
                Description = cam.Description,
                Class = @class
            };

            _db.Announcements.Add(announcement);
            await _db.SaveChangesAsync();
            return announcement;
        }

        /// <summary>
        /// Updates an announcement using details in the given model
        /// </summary>
        /// <param name="uam">Model containing details of the announcement to update</param>
        public async Task UpdateAnnouncement(UpdateAnnouncementModel uam)
        {
            var announcement = await _db.Announcements.FindAsync(uam.Id);
            if(announcement == null)
            {
                throw new Exception("No announcement with id: " + uam.Id);
            }

            announcement.Title = uam.Title;
            announcement.Description = uam.Description;
            _db.Entry(announcement).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                // Deleted before update maybe?
                if(!AnnouncementExists(announcement.Id))
                {
                    throw new Exception("No announcement with id: " + uam.Id);
                }
            }
        }

        /// <summary>
        /// Deletes the announcement with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Announcement> DeleteAnnouncement(int id)
        {
            Announcement announcement = await _db.Announcements.FindAsync(id);
            if(announcement == null)
            {
                throw new Exception("No announcement with id: " + id);
            }

            _db.Announcements.Remove(announcement);
            _db.SaveChanges();

            return announcement;
        }

        /// <summary>
        /// Returns a list of announcements for the given class
        /// </summary>
        /// <param name="class"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Announcement>> GetAnnouncementsForClass(Class @class)
        {
            return await _db.Announcements.Where(a => a.Class.Id == @class.Id).ToListAsync();
        }
        /// <summary>
        /// Returns true if an announcement with the given id exists. Otherwise false
        /// </summary>
        /// <param name="id">Id of the announcement to search for</param>
        /// <returns></returns>
        public bool AnnouncementExists(int id)
        {
            return _db.Announcements.Count(e => e.Id == id) > 0;
        }
    }
}