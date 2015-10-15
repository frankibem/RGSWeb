namespace RGSWeb.Models
{
    public class UserResultView
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserResultView() { }

        public UserResultView(ApplicationUser user)
        {
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
        }
    }
}
