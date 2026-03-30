using Microsoft.AspNetCore.Identity;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow
{
    public static class SeedData
    {
        public static void Seed(AppDbContext db, IConfiguration configuration)
        {
            if (!db.Roles.Any())
            {
                db.Roles.AddRange(
                    new Role { Id = 1, Name = "Admin" },
                    new Role { Id = 2, Name = "User" }
                );
                db.SaveChanges();
            }

            if (!db.Users.Any(u => u.UserName == "admin"))
            {
                db.Users.Add(CreateAdminUser(configuration));
                db.SaveChanges();
            }
        }

        private static User CreateAdminUser(IConfiguration configuration)
        {
            var passwordHasher = new PasswordHasher<User>();
            var user = new User
            {
                UserName = "admin",
                FullName = "Admin User",
                Email = "admin@example.com",
                RoleId = 1,
                PasswordHash = "temp"
            };

            user.PasswordHash = passwordHasher.HashPassword(user, configuration["SeedAdminPassword"]);

            return user;
        }
    }
}
