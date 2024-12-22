using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nora.Data;
using Nora.Models;

namespace Nora.Controllers
{

    [Authorize(Roles ="User, Admin")]
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

            var channels = db.Channels
                     .Include(c => c.CategoryChannels)
                     .ThenInclude(cc => cc.Category) 
                     .Include(c => c.User)           
                     .OrderByDescending(c => c.Date);




            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            // MOTOR DE CAUTARE

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); 

                channels = db.Channels
                    .Where(c => c.Title.Contains(search))
                    .Include(c => c.CategoryChannels)
                    .ThenInclude(cc => cc.Category)
                    .Include(c => c.User)
                    .OrderByDescending(c => c.Date);

            }

            ViewBag.SearchString = search;
            ViewBag.channels = channels.ToList();

            return View();
        }


        [HttpGet]
        public IActionResult New()
        {
            ViewBag.Categories = db.Categories.ToList(); // Assuming there's a `Categories` table in the database.
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> New(Channel channel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = db.Categories.ToList();
                return View(channel);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            channel.Date = DateTime.Now;
            channel.UserId = currentUser.Id;

            db.Channels.Add(channel);

            // Save the channel first to get its ID
            await db.SaveChangesAsync();

            // Add selected categories
            foreach (var categoryId in channel.SelectedCategoryIds)
            {
                var categoryChannel = new CategoryChannel
                {
                    ChannelId = channel.Id,
                    CategoryId = categoryId
                };
                db.CategoryChannels.Add(categoryChannel);
            }

            await db.SaveChangesAsync();

            var userChannel = new UserChannel();

            userChannel.ChannelId = channel.Id;
            userChannel.UserId = currentUser.Id;
            userChannel.IsModerator = true;
            userChannel.IsAccepted = true;

            db.UserChannels.Add(userChannel);

            // Save 
            await db.SaveChangesAsync();

            TempData["message"] = "Channel created successfully!";
            TempData["messageType"] = "success";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Show(int id)
        {
            var channel = db.Channels
                            .Include(c => c.CategoryChannels)
                            .ThenInclude(cc => cc.Category)
                            .FirstOrDefault(c => c.Id == id);

            if (channel == null)
            {
                return NotFound();
            }

            return View(channel);
        }


        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            var channel = db.Channels
                            .Include(c => c.CategoryChannels)
                            .ThenInclude(cc => cc.Category)
                            .FirstOrDefault(c => c.Id == id);

            if (channel == null)
            {
                return NotFound();
            }

            var categories = db.Categories.ToList();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryIds = channel.CategoryChannels?.Select(cc => cc.CategoryId).ToList();

            return View(channel);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Channel updatedChannel, List<int> SelectedCategoryIds)
        {
            var channel = db.Channels
                            .Include(c => c.CategoryChannels)
                            .FirstOrDefault(c => c.Id == id);

            if (channel == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Update basic properties
                channel.Title = updatedChannel.Title;
                channel.Description = updatedChannel.Description;

                // Update categories
                channel.CategoryChannels.Clear();
                if (SelectedCategoryIds != null && SelectedCategoryIds.Any())
                {
                    foreach (var categoryId in SelectedCategoryIds)
                    {
                        channel.CategoryChannels.Add(new CategoryChannel
                        {
                            ChannelId = channel.Id,
                            CategoryId = categoryId
                        });
                    }
                }

                db.SaveChanges();
                TempData["message"] = "Channel modified successfully!";
                TempData["messageType"] = "success";

                return RedirectToAction("Index");
            }

            // If invalid, reload categories and return view
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.SelectedCategoryIds = SelectedCategoryIds;

            return View(updatedChannel);
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var channel = db.Channels.Include(c => c.CategoryChannels).FirstOrDefault(c => c.Id == id);

            if (channel == null)
            {
                return NotFound();
            }

            db.CategoryChannels.RemoveRange(channel.CategoryChannels);
            db.Channels.Remove(channel);
            await db.SaveChangesAsync();

            TempData["message"] = "Channel deleted successfully!";
            TempData["messageType"] = "success";

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult UserChannels()
        {
            var userId = _userManager.GetUserId(User);

            var channels = db.Channels
                     .Include(c => c.CategoryChannels)
                     .ThenInclude(cc => cc.Category)
                     .Include(c => c.User)
                     .Include(c => c.UserChannels)
                     .Where(c => c.UserChannels != null && c.UserChannels.Any(uc => uc.UserId == userId))
                     .OrderByDescending(c => c.Date);



            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                channels = db.Channels
                     .Include(c => c.CategoryChannels)
                     .ThenInclude(cc => cc.Category)
                     .Include(c => c.User)
                     .Include(c => c.UserChannels)
                     .Where(c => c.UserChannels != null && c.UserChannels.Any(uc => uc.UserId == userId) && c.Title.Contains(search))
                     .OrderByDescending(c => c.Date);


            }

            ViewBag.SearchString = search;
            ViewBag.Channels = channels;


            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult MembersList(int? id)
        {
            var members = db.UserChannels.Include("User")
               .Where(uc => uc.ChannelId == id)
               .OrderBy(uc => uc.JoinDate);

            ViewBag.Members = members;

            return View();
        }
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> JoinChannel(int? id)
        {

            var currentUser = await _userManager.GetUserAsync(User);

            var userChannel = new UserChannel();

            var userChannelExists = db.UserChannels.Any(uc => uc.UserId == currentUser.Id && uc.ChannelId == id);
            if (userChannelExists) {
                return RedirectToAction("Show", new { id });
            }

            userChannel.ChannelId = id;
            userChannel.UserId = currentUser.Id;
            userChannel.JoinDate = DateTime.Now;

            db.UserChannels.Add(userChannel);

            // Save 
            await db.SaveChangesAsync();


            return RedirectToAction("Show", new { id });
        }


    }
}