using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
    public class UserProfileEditViewModel
    {
        public UserProfile UserProfile { get; set; }
        public List<UserType> UserTypes { get; set; }
        public IFormFile Image { get; set; }
        public bool IsSafeToEditUserType { get; set; }
    }
}