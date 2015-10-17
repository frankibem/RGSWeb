namespace RGSWeb.Models
{
    /// <summary>
    /// Alternative model to represent a user
    /// </summary>
    public class UserResultView
    {
        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Creates a default UserResultView
        /// </summary>
        public UserResultView() { }

        /// <summary>
        /// Create a UserResultView for the given user
        /// </summary>
        /// <param name="user">Model to create the UserResultView from</param>
        public UserResultView(ApplicationUser user)
        {
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
        }
    }
}