using System;

namespace RGSWeb.Models
{
    /// <summary>
    /// Class to represent a class announcement
    /// </summary>
    public class Announcement
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
        public Class Class { get; set; }
    }

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