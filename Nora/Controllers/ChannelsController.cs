using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nora.Data;
using Nora.Models;

namespace Nora.Controllers
{
    [Authorize]
    public class ChannelsController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ChannelsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {

            var channels = db.Channels.Include("User")
                                      .OrderByDescending(c => c.Date);

            ViewBag.Channels = channels;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult UserChannels()
        {
            var userId = _userManager.GetUserId(User);

            var channels = db.Channels
                .Include(c => c.User)
                .Include(c => c.UserChannels)
                .Where(c => c.UserChannels != null && c.UserChannels.Any(uc => uc.UserId == userId))
                .OrderByDescending(c => c.Date);



            ViewBag.Channels = channels;


            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult MembersList(int? id)
        {
            var users = db.Users
               .Include(u => u.UserChannels)
               .Where(u => u.UserChannels != null && u.UserChannels.Any(uc => uc.ChannelId == id))
               .OrderBy(u => u.UserName);

            ViewBag.Users = users;

            return View();
        }

       
    }
}