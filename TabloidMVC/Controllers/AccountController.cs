using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace TabloidMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserTypeRepository _userTypeRepository;

        public AccountController(IUserProfileRepository userProfileRepository,
            IUserTypeRepository userTypeRepository)
        {
            _userProfileRepository = userProfileRepository;
            _userTypeRepository = userTypeRepository;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Credentials credentials)
        {
            var userProfile = _userProfileRepository.GetByEmail(credentials.Email);

            if (userProfile == null || userProfile.Activated == false)
            {
                ModelState.AddModelError("Email", "Invalid email");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userProfile.Id.ToString()),
                new Claim(ClaimTypes.Email, userProfile.Email),
                new Claim(ClaimTypes.Role, userProfile.UserType.Name),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        // GET: New User Form
        public IActionResult Register()
        {
            return View();
        }

        // POST: New User Registration Info
        [HttpPost]
        public IActionResult Register(UserProfile newUser)
        {
            try
            {
                newUser.CreateDateTime = DateAndTime.Now;
                newUser.UserTypeId = 2;

                _userProfileRepository.Add(newUser);

                return RedirectToAction("Login");
            }
            catch
            {
                return View(newUser);
            }
        }

        // GET: list of all users (admin only)
        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                List<UserProfile> users = _userProfileRepository.GetUsers();
                return View(users);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: User details
        [Authorize]
        public IActionResult Details(int id)
        {
            int userId = GetCurrentUserProfileId();
            if (User.IsInRole("Admin") || userId == id)
            {
                UserProfile userProfile = _userProfileRepository.GetUserById(id);

                return View(userProfile);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Edit User Form
        [Authorize]
        public IActionResult Edit(int id)
        {
            UserProfileEditViewModel vm = new UserProfileEditViewModel
            {
                UserProfile = _userProfileRepository.GetUserById(id),
                UserTypes = _userTypeRepository.GetUserTypes(),
                IsSafeToEditUserType = true
            };
            if (vm.UserProfile != null)
            {
                if (vm.UserProfile.UserTypeId == 1)
                {
                    List<UserProfile> admins = _userProfileRepository.GetAdmins();
                    if (admins.Count < 2)
                    {
                        vm.IsSafeToEditUserType = false;
                    }
                }
                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: Edit User Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfileEditViewModel vm)
        {
            if (vm.IsSafeToEditUserType == false && vm.UserProfile.UserTypeId != 1)
            {
                vm.IsSafeToEditUserType = true;
                return RedirectToAction("LastAdminError");
            }
            try
            {
                if (vm.Image.Length > 0)
                {
                    var fileName = Guid.NewGuid();
                    var filePath = Path.Combine("wwwroot", "images", $"{fileName}.jpg");
                       
                    using (var stream = System.IO.File.Create(filePath))
                       {
                           await vm.Image.CopyToAsync(stream);
                       }
                       vm.UserProfile.ImageLocation = $"/images/{fileName}.jpg";
                }
                _userProfileRepository.UpdateUser(vm.UserProfile);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View(vm);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Deactivate(int id)
        {
            UserProfile user = _userProfileRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deactivate(UserProfile user)
        {
            if (user.UserTypeId == 1)
            {
                List<UserProfile> admins = _userProfileRepository.GetAdmins();
                if (admins.Count < 2)
                {
                    return RedirectToAction("LastAdminError");
                }
            }
            user.Activated = false;
            _userProfileRepository.UpdateUser(user);
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult ViewDeactivated()
        {
            List<UserProfile> users = _userProfileRepository.GetDeactivatedUsers();
            return View(users);
        }

        [Authorize]
        public IActionResult Reactivate(int id)
        {
            UserProfile user = _userProfileRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivate(UserProfile user)
        {
            user.Activated = true;
            _userProfileRepository.UpdateUser(user);
            return RedirectToAction("ViewDeactivated");
        }

        [Authorize]
        public IActionResult LastAdminError()
        {
            return View();
        }

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}