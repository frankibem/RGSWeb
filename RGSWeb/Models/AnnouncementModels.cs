using System;
using System.ComponentModel.DataAnnotations;

namespace RGSWeb.Models
{
    /// <summary>
    /// Model used to create an announcement
    /// </summary>
    public class CreateAnnouncementModel
    {
        /// <summary>
        /// Title of the announcment
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Description of the announcement (main content)
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Id of the class for which to create the announcement
        /// </summary>
        public int ClassId { get; set; }
    }

    /// <summary>
    /// Model used to update an announcement
    /// </summary>
    public class UpdateAnnouncementModel
    {
        /// <summary>
        /// Id of the announcement to update
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Updated announcement title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Updated announcement description
        /// </summary>
        public string Description { get; set; }
    }
}