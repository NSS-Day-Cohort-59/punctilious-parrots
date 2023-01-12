using Microsoft.Data.SqlClient.Server;
using System;
using System.ComponentModel;
using System.Runtime.Intrinsics.X86;
using System.Security.Policy;

namespace TabloidMVC.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Display Name")]
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public DateTime CreateDateTime { get; set; }
        [DisplayName("Profile Picture")]
        public string ImageLocation { get; set; }
        public int UserTypeId { get; set; }
        public UserType UserType { get; set; }
        public bool Activated { get; set; } = true;
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}