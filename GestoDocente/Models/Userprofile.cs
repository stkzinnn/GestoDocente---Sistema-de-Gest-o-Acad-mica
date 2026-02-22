using GestoDocente.Models;

namespace GestoDocente.Models
{
    public class UserProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhotoPath { get; set; }
    }
}
public static class SessionData
{
    public static UserProfile CurrentUserProfile { get; set; } = new UserProfile();
}
