using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IUserProfileRepository
    {
        UserProfile GetByEmail(string email);
        List<UserProfile> GetUsers();
        List<UserProfile> GetAdmins();
        UserProfile GetUserById(int userId);
        void UpdateUser(UserProfile user);
        void Add(UserProfile newUser);
        List<UserProfile> GetDeactivatedUsers();

    }
}