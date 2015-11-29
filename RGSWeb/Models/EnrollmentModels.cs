using System.ComponentModel.DataAnnotations;

namespace RGSWeb.Models
{
    /// <summary>
    /// Represents the state of a student's enrollment
    /// </summary>
    public enum EnrollmentState
    {
        /// <summary>
        /// Return all enrollments pending and non-pending
        /// </summary>
        All,
        /// <summary>
        /// Return only pending enrollments
        /// </summary>
        Pending,
        /// <summary>
        /// Return only non-pending enrollments
        /// </summary>
        Accepted
    }

    /// <summary>
    /// Model used for requesting and accepting an enrollment by a student
    /// </summary>
    public class EnrollmentBindingModel
    {
        /// <summary>
        /// The student's username
        /// </summary>
        [Required]
        public string StudentUserName { get; set; }
        /// <summary>
        /// Id of the class for enrollment
        /// </summary>
        [Required]
        public int ClassId { get; set; }
        /// <summary>
        /// True if student should be accepted, false otherwise.
        /// </summary>
        public bool Accept { get; set; }
    }
}
