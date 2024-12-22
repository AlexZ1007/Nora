using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nora.Data;
using System.Linq;

namespace Nora.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class UserChannelController : Controller
    {
        private readonly ApplicationDbContext db;

        public UserChannelController(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpPost]
        public JsonResult Accept(int id)
        {
            var userChannel = db.UserChannels
                .Include(uc => uc.User) // Eagerly load User property
                .SingleOrDefault(uc => uc.Id == id);

            if (userChannel == null)
                return Json(new { success = false, message = "User not found." });

            userChannel.IsAccepted = true;
            db.SaveChanges();

            // Project the data to avoid circular references
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
                .SingleOrDefault(uc => uc.Id == id);

            if (userChannel == null)
                return Json(new { success = false, message = "User not found." });

            db.UserChannels.Remove(userChannel);
            db.SaveChanges();

            return Json(new { success = true, userId = id });
        }

        [HttpPost]
        public JsonResult MakeModerator(int id)
        {
            var userChannel = db.UserChannels
                .Include(uc => uc.User)
                .SingleOrDefault(uc => uc.Id == id);

            if (userChannel == null)
                return Json(new { success = false, message = "User not found." });

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
                .SingleOrDefault(uc => uc.Id == id);

            if (userChannel == null)
                return Json(new { success = false, message = "User not found." });

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
                .SingleOrDefault(uc => uc.Id == id);

            if (userChannel == null)
                return Json(new { success = false, message = "User not found." });

            db.UserChannels.Remove(userChannel);
            db.SaveChanges();

            return Json(new { success = true, userId = id });
        }
    }
}
