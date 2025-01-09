using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nora.Data;
using Nora.Models;

namespace Nora.Controllers
{

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
            var userId = _userManager.GetUserId(User);

            var channels = db.Channels
                        .Include(c => c.CategoryChannels)
                        .ThenInclude(cc => cc.Category)
                        .Include(c => c.User)
                        .Include(c => c.UserChannels) // Include UserChannels
                        .OrderByDescending(c => c.Date)
                        .ToList();

            var userChannels = db.UserChannels.Where(uc => uc.UserId == userId).ToList();

            foreach (var channel in channels)
            {
                var userChannel = userChannels.FirstOrDefault(uc => uc.ChannelId == channel.Id);

                // Set temporary properties
                channel.IsUserMember = userChannel != null && userChannel.IsAccepted;
                channel.IsPending = userChannel != null && !userChannel.IsAccepted && !userChannel.IsModerator;
            }

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            // MOTOR DE CAUTARE

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim().ToLower();

                channels = channels.Where(c => c.Title.ToLower().Contains(search)).ToList();
            }

            ViewBag.SearchString = search;
            ViewBag.channels = channels.ToList();

            return View();
        }


        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            ViewBag.Categories = db.Categories.ToList(); 
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> New(Channel channel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = db.Categories.ToList();
                return View(channel);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Set basic properties for the channel
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

            // Automatically add the creator to the UserChannels table as a moderator
            var userChannel = new UserChannel
            {
                UserId = currentUser.Id,
                ChannelId = channel.Id,
                IsAccepted = true, // Automatically accepted
                IsModerator = true, // Mark as moderator
                JoinDate = DateTime.Now
            };
            db.UserChannels.Add(userChannel);

            await db.SaveChangesAsync();

            TempData["message"] = "Canalul a fost adăugat cu succes!";
            TempData["messageType"] = "success";

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Show(int id)
        {
            var channel = db.Channels
                            .Include(c => c.CategoryChannels)
                            .ThenInclude(cc => cc.Category)
                            .Include(c => c.UserChannels) // Include UserChannels to check membership
                            .FirstOrDefault(c => c.Id == id);


            if (channel == null)
            {
                return NotFound();
            }


            var creatorUserId = channel.UserId;

            var userId = _userManager.GetUserId(User);

            // Check if the current user is a member of the channel
            var userChannel = channel.UserChannels?.FirstOrDefault(uc => uc.UserId == userId);

            var currentUserId = _userManager.GetUserId(User);

            // Check if the current user is an admin, creator, or moderator
            var isAdmin = User.IsInRole("Admin");
            var isModerator = channel.UserChannels
                                      .Any(uc => uc.UserId == currentUserId && uc.IsModerator);
        
            if (!isAdmin && !isModerator )
            {
                return Forbid(); // 403 Forbidden
            }

            ViewBag.CreatorUserId = creatorUserId;
            channel.IsUserMember = userChannel != null && userChannel.IsAccepted;


            return View(channel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var channel = db.Channels
                            .Include(c => c.CategoryChannels)
                            .Include (c => c.UserChannels)
                            .FirstOrDefault(c => c.Id == id);




            if (channel == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            // Check if the current user is an admin or a moderator for this channel
            var isAdmin = User.IsInRole("Admin");
            var isModerator = channel.UserChannels.Any(uc => uc.UserId == currentUserId && uc.IsModerator);

            if (!isAdmin && !isModerator)
            {
                return Forbid(); // 403 Forbidden
            }

            var categories = db.Categories.ToList();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryIds = channel.CategoryChannels?.Select(cc => cc.CategoryId).ToList();

            return View(channel);
        }

        [HttpPost]
        public IActionResult Edit(int id, Channel updatedChannel, List<int> SelectedCategoryIds)
        {
            var channel = db.Channels
                .Include(c => c.CategoryChannels) // Include categories
                .Include(c => c.UserChannels) // Include UserChannels to check moderators
                .FirstOrDefault(c => c.Id == id);


            if (channel == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            // Check if the current user is an admin or a moderator for this channel
            var isAdmin = User.IsInRole("Admin");
            var isModerator = channel.UserChannels.Any(uc => uc.UserId == currentUserId && uc.IsModerator);

            if (!isAdmin && !isModerator)
            {
                return Forbid(); // 403 Forbidden
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
            var channel = db.Channels
                .Include(c => c.CategoryChannels)
                .Include(c => c.UserChannels) // Include UserChannels to check moderators
                .FirstOrDefault(c => c.Id == id);

            if (channel == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            var isAdmin = User.IsInRole("Admin");
            var isModerator = channel.UserChannels.Any(uc => uc.UserId == currentUserId && uc.IsModerator);

            if (!isAdmin && !isModerator)
            {
                return Forbid(); // 403 Forbidden
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
                     .OrderByDescending(c => c.Date).ToList();

            // Pass this data to the view
            var userChannels = db.UserChannels.Where(uc => uc.UserId == userId).ToList();

         

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim().ToLower();

                channels = channels.Where(c => c.Title.ToLower().Contains(search)).ToList();

            }

            foreach (var channel in channels)
            {
                var userChannel = userChannels.FirstOrDefault(uc => uc.ChannelId == channel.Id);


                channel.IsUserMember = userChannel != null && userChannel.IsAccepted;
                channel.IsPending = userChannel != null && !userChannel.IsAccepted && !userChannel.IsModerator;
            }


            ViewBag.SearchString = search;
            ViewBag.Channels = channels;


            return View();
        }



        [Authorize(Roles = "User,Admin")]
        public IActionResult MembersList(int? id)
        {
            if (id == null)
            {
                return BadRequest("Channel ID is required.");
            }

            var channel = db.Channels
                .Include(c => c.UserChannels)
                .ThenInclude(uc => uc.User)
                .FirstOrDefault(c => c.Id == id);

            if (channel == null)
            {
                return NotFound("Channel not found.");
            }


            var currentUserId = _userManager.GetUserId(User);

            var isMember = channel.UserChannels
        .Any(uc => uc.UserId == currentUserId && uc.IsAccepted);

            if (!isMember)
            {
                return Forbid(); // 403 Forbidden
            }

            // Check if the current user is a moderator for the channel
            var isCurrentUserModerator = channel.UserChannels
                .Any(uc => uc.UserId == currentUserId && uc.IsModerator);

            // Identify the creator of the channel
            var creatorUserId = channel.UserId;
            // Get members of the channel
            var members = channel.UserChannels
                .OrderBy(uc => uc.JoinDate)
                .ToList();

            // Pass data to the view
            ViewBag.Members = members;
            ViewBag.IsCurrentUserModerator = isCurrentUserModerator;
            ViewBag.CreatorUserId = creatorUserId; // Pass the creator's UserId to the view
            ViewBag.CurrentUserId=currentUserId;
            return View();
        }



        public async Task<IActionResult> JoinChannel(int? id)
        {
            if (id == null)
            {
                return BadRequest("Channel ID is required.");
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            // Check if the user is already in the channel
            var userChannelExists = db.UserChannels.Any(uc => uc.UserId == currentUser.Id && uc.ChannelId == id);
            if (userChannelExists)
            {
                return RedirectToAction("Index", "Messages", new { id });
            }

            // Create a new UserChannel entry
            var userChannel = new UserChannel
            {
                ChannelId = id.Value,
                UserId = currentUser.Id,
                JoinDate = DateTime.Now,
                IsAccepted = User.IsInRole("Admin"), 
                IsModerator = User.IsInRole("Admin") 
            };

            db.UserChannels.Add(userChannel);

            // Save the changes
            await db.SaveChangesAsync();

            return RedirectToAction("UserChannels", new { id });
        }





    }
}