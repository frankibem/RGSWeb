namespace RGSWeb.Models
{
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
}