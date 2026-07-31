using Microsoft.AspNetCore.Identity;

namespace Mls.Patients.Api.Authentication
{
    public static class InMemoryUserStore
    {
        public static IReadOnlyList<ApiUser> Users { get; }

        static InMemoryUserStore()
        {
            var hasher = new PasswordHasher<ApiUser>();

            var frontend = new ApiUser { Username = "frontend" };
            frontend.PasswordHash = hasher.HashPassword(frontend, "P@ssw0rd2026!");

            Users = new List<ApiUser> { frontend };
        }

        public static ApiUser? FindByUsername(string username)
        {
            return Users.FirstOrDefault(u => u.Username == username);
        }
    }
}
