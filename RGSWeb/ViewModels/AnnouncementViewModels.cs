using RGSWeb.Models;
using System;

namespace RGSWeb.ViewModels
{
    /// <summary>
    /// View-model to represent an announcement
    /// </summary>
    public class AnnouncementViewModel
    {
        /// <summary>
        /// Id of the announcment
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Title of the announcement
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Description of the announcement
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Creation time of the assignment
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Class for which the announcement was created
        /// </summary>
        public ClassViewModel Class { get; set; }

        /// <summary>
        /// Creates a view-model for the announcement
        /// </summary>
        /// <param name="announcement"></param>
        public AnnouncementViewModel(Announcement announcement)
        {
            Id = announcement.Id;
            Title = announcement.Title;
            Description = announcement.Description;
            CreatedOn = announcement.CreatedOn;
            Class = new ClassViewModel(announcement.Class);
        }
    }
}