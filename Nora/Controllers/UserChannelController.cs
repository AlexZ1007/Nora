using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nora.Data;
using Nora.Models;
using System.Linq;

namespace Nora.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class UserChannelController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserChannelController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;

        }

        [HttpPost]
        public JsonResult Accept(int id)
        {
            var userChannel = db.UserChannels
                .Include(uc => uc.User)
                .Include(uc => uc.Channel) // Load Channel to access creator
                .SingleOrDefault(uc => uc.Id == id);

            if (userChannel == null)
                return Json(new { success = false, message = "User not found." });

            if (userChannel.UserId == userChannel.Channel.UserId)
            {
                return Json(new { success = false, message = "The creator's status cannot be modified." });
            }

            userChannel.IsAccepted = true;
            db.SaveChanges();

            var result = new
            {
                userChannel.Id,
                User = new
                {
                    userChannel.User.UserName,
                    userChannel.User.Email
                },
                userChannel.IsModerator,
                userChannel.IsAccepted,
                JoinDate = userChannel.JoinDate.ToString("yyyy-MM-dd")
            };

            return Json(new { success = true, userChannel = result });
        }


        [HttpPost]
        public JsonResult Deny(int id)
        {
            var userChannel = db.UserChannels
                .Include(uc => uc.User)
                .Include(uc => uc.Channel)
                .SingleOrDefault(uc => uc.Id == id);

            if (userChannel == null)
                return Json(new { success = false, message = "User not found." });

            if (userChannel.UserId == userChannel.Channel.UserId)
            {
                return Json(new { success = false, message = "The creator cannot be removed." });
            }

            db.UserChannels.Remove(userChannel);
            db.SaveChanges();

            return Json(new { success = true, userId = id });
        }


        [HttpPost]
        public JsonResult MakeModerator(int id)
        {
            var userChannel = db.UserChannels
                .Include(uc => uc.User)
                .Include(uc => uc.Channel)
                .SingleOrDefault(uc => uc.Id == id);

            if (userChannel == null)
                return Json(new { success = false, message = "User not found." });

            if (userChannel.UserId == userChannel.Channel.UserId)
            {
                return Json(new { success = false, message = "The creator is already a moderator by default." });
            }

            userChannel.IsModerator = true;
            db.SaveChanges();

            var result = new
            {
                userChannel.Id,
                User = new
                {
                    userChannel.User.UserName,
                    userChannel.User.Email
                },
                userChannel.IsModerator,
                userChannel.IsAccepted,
                JoinDate = userChannel.JoinDate.ToString("yyyy-MM-dd")
            };

            return Json(new { success = true, userChannel = result });
        }


        [HttpPost]
        public JsonResult RemoveModerator(int id)
        {
            var userChannel = db.UserChannels
                .Include(uc => uc.User)
                .Include(uc => uc.Channel)
                .SingleOrDefault(uc => uc.Id == id);

            if (userChannel == null)
                return Json(new { success = false, message = "User not found." });

            if (userChannel.UserId == userChannel.Channel.UserId)
            {
                return Json(new { success = false, message = "The creator cannot be demoted as a moderator." });
            }

            userChannel.IsModerator = false;
            db.SaveChanges();

            var result = new
            {
                userChannel.Id,
                User = new
                {
                    userChannel.User.UserName,
                    userChannel.User.Email
                },
                userChannel.IsModerator,
                userChannel.IsAccepted,
                JoinDate = userChannel.JoinDate.ToString("yyyy-MM-dd")
            };

            return Json(new { success = true, userChannel = result });
        }

        [HttpPost]
        public JsonResult Remove(int id)
        {
            var userChannel = db.UserChannels
                .Include(uc => uc.User)
                .Include(uc => uc.Channel)
                .SingleOrDefault(uc => uc.Id == id);

            if (userChannel == null)
                return Json(new { success = false, message = "User not found." });

            if (userChannel.UserId == userChannel.Channel.UserId)
            {
                return Json(new { success = false, message = "The creator cannot be removed from the channel." });
            }

            db.UserChannels.Remove(userChannel);
            db.SaveChanges();

            return Json(new { success = true, userId = id });
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Leave(int id)
        {
            var currentUserId = _userManager.GetUserId(User);

            // Check if the user is a member of the channel
            var userChannel = await db.UserChannels
                .FirstOrDefaultAsync(uc => uc.ChannelId == id && uc.UserId == currentUserId);

            if (userChannel == null)
            {
                return NotFound();
            }

                
            if (userChannel.UserId == userChannel.Channel.UserId)
            {
                return Forbid();
            }

            db.UserChannels.Remove(userChannel);
            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Channels"); // Redirect to the list of channels or wherever you'd like
        }


    }
}
