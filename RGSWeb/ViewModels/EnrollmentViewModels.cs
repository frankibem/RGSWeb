using RGSWeb.Models;

namespace RGSWeb.ViewModels
{
    /// <summary>
    /// View-model to represent an enrollment in a class
    /// </summary>
    public class EnrollmentViewModel
    {
        /// <summary>
        /// The Id of the enrollment
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The Class for which the enrollment applies
        /// </summary>
        public ClassViewModel Class { get; set; }
        /// <summary>
        /// Pending is true if the student has not been accepted to a class
        /// and false otherwise
        /// </summary>
        public bool Pending { get; set; }
        /// <summary>
        /// The student for which the enrollment applies
        /// </summary>
        public UserViewModel Student { get; set; }

        /// <summary>
        /// Students grade in the class
        /// </summary>
        /// <remarks>Default grade is -1 (pending). User's of this class must set it explicitly</remarks>
        public float Grade { get; set; }

        /// <summary>
        /// Creates a view-model for an enrollment with default values
        /// </summary>
        public EnrollmentViewModel() { }

        /// <summary>
        /// Creates a view-model for the given enrollment
        /// </summary>
        /// <param name="enrollment">Model containing the details for the view-model</param>
        public EnrollmentViewModel(Enrollment enrollment)
        {
            if(enrollment != null)
            {
                Id = enrollment.Id;
                Pending = enrollment.Pending;
                Class = new ClassViewModel(enrollment.Class);
                Student = new UserViewModel(enrollment.Student);
                Grade = -1;
            }
        }
    }
}