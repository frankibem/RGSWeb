using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGSWeb.Models
{
    /// <summary>
    /// A class to represent student enrollment in a "class"
    /// </summary>
    public class Enrollment
    {
        /// <summary>
        /// The Id of the enrollment
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The Class for which the enrollment applies
        /// </summary>
        public Class Class { get; set; }
        /// <summary>
        /// Pending is true if the student has not been accepted to a class
        /// and false otherwise
        /// </summary>
        public bool Pending { get; set; }
        /// <summary>
        /// The student for which the enrollment applies
        /// </summary>
        public ApplicationUser Student { get; set; }
    }

    /// <summary>
    /// Represents the 
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
}
