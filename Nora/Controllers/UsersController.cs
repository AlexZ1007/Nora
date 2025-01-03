using Nora.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nora.Models;

namespace Nora.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;

            ViewBag.UsersList = users;

            return View();
        }

        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;

            ViewBag.UserCurent = await _userManager.GetUserAsync(User);

            return View(user);
        }


        

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = db.Users
                         .Include(u => u.UserChannels)
                         .Include(u => u.Messages)
                         .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Delete user messages
            if (user.Messages != null && user.Messages.Any())
            {
                db.Messages.RemoveRange(user.Messages);
            }

            // Delete user channels
            var userChannels = db.Channels.Where(c => c.UserId == user.Id).ToList();
            if (userChannels.Any())
            {
                db.Channels.RemoveRange(userChannels);
            }

            // Delete user channels associations
            if (user.UserChannels != null && user.UserChannels.Any())
            {
                db.UserChannels.RemoveRange(user.UserChannels);
            }

            // Delete the user
            db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRole = userRoles.FirstOrDefault(); // Assuming a single role per user
            ViewBag.AllRoles = GetAllRoles();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string newRole, ApplicationUser updatedUser)
        {
            if (id == null || updatedUser == null)
            {
                return BadRequest("Invalid data.");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update phone number
            user.PhoneNumber = updatedUser.PhoneNumber;

            // Save user updates
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user details.");
                return View(user);
            }

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!string.IsNullOrWhiteSpace(newRole) && !currentRoles.Contains(newRole))
            {
                // Remove existing roles
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // Add the new role by name (not ID)
                var roleResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to update user role.");
                    return View(user);
                }
            }

            return RedirectToAction("Index");
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            return db.Roles.Select(role => new SelectListItem
            {
                Value = role.Name,  // Use Name instead of Id
                Text = role.Name     // Display the role name
            }).ToList();
        }


    }
}
