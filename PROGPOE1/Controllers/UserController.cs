using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PROGPOE1.DAL;
using PROGPOE1.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace PROGPOE1.Controllers
{
    public class UserController : Controller
    {
        private readonly EmployeeDbContext _context;

        public UserController(EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Redirect to Employee Index if already authenticated
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Employee");

            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists in the database
                if (IsUsernameExists(user.Username))
                {
                    TempData["errorMessage"] = "Username already exists. Please choose a different one.";
                    return View(user);
                }

                AddUserToDatabase(user);
                TempData["successMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }

            TempData["errorMessage"] = "Invalid data. Please try again.";
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Redirect to Employee Index if already authenticated
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Employee");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["errorMessage"] = "Username and password cannot be empty.";
                return View();
            }

            var user = GetUserFromDatabase(username, password);
            if (user != null)
            {
                await SignInUser(user);
                TempData["successMessage"] = $"Welcome, {user.Username}!";
                return RedirectToAction("Index", "Employee");
            }

            TempData["errorMessage"] = "Invalid credentials. Please try again.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Helper Methods
        private bool IsUsernameExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        private void AddUserToDatabase(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        private User GetUserFromDatabase(string username, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                });
        }
    }
}
