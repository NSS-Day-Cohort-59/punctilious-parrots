using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IUserProfileRepository
    {
        UserProfile GetByEmail(string email);
        List<UserProfile> GetUsers();
        UserProfile GetUserById(int userId);
        void UpdateUser(UserProfile user);
        void Add(UserProfile newUser);

    }
}