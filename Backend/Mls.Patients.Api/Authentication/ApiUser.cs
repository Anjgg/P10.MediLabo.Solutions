namespace Mls.Patients.Api.Authentication
{
    public class ApiUser
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
