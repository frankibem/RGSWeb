using RGSWeb.Managers;
using RGSWeb.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RGSWeb.ViewModels
{
    /// <summary>
    /// Base view-model for application users
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// The user's first name
        /// </summary>
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// The user's last name
        /// </summary>
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// Username for user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Creates a default UserViewModel
        /// </summary>
        public UserViewModel() { }

        /// <summary>
        /// Create a UserModel for an ApplicationUser
        /// </summary>
        /// <param name="user"></param>
        public UserViewModel(ApplicationUser user)
        {
            if(user != null)
            {
                this.FirstName = user.FirstName;
                this.LastName = user.LastName;
                this.UserName = user.UserName;
            }
        }
    }

    /// <summary>
    /// Model for representing a student
    /// </summary>
    public class StudentViewModel : UserViewModel
    {
        /// <summary>
        /// The student's grade for the class
        /// </summary>
        public float Grade { get; set; }

        private StudentViewModel() { }

        /// <summary>
        /// Creates a StudentViewModel for an ApplicationUser
        /// </summary>
        /// <param name="user">ApplicationUser to construct view-model from</param>
        /// <param name="class">Class to calculate the students in</param>
        /// <param name="manager">Grademanager to calculate student's grade</param>
        public StudentViewModel(ApplicationUser user)
            : base(user)
        { }
    }
}