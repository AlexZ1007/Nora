using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nora.Data;
using Nora.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Channels;

namespace Nora.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index([FromRoute(Name = "id")] int channelId)
        {
            var channel = await _context.Channels
                .Include(c => c.Messages)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(c => c.Id == channelId);

            var currentUser = await _userManager.GetUserAsync(User);

            if (channel == null)
            {
                return NotFound();
            }
            ViewBag.CurrentUserId = currentUser?.Id;

            return View(channel);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int channelId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction("Index", new { channelId });
            }

            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); 
            }

            var message = new Message
            {
                Content = content,
                Date = DateTime.Now,
                UserId = userId, 
                ChannelId = channelId
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Redirect("/Messages/Index/"+channelId);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            // Check if the current user is the creator
            if (message.UserId != currentUserId &&  !User.IsInRole("Admin"))
            {
                return Forbid(); // 403 Forbidden
            }

            message.IsDeleted = true; // Mark the message as deleted
            await _context.SaveChangesAsync();

            return Redirect("/Messages/Index/" + message.ChannelId);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (message.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();  // Only the message owner or Admin can edit the message
            }
            
            return View(message);  // Return the message object to the view
        }

        // POST: Edit Message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                ModelState.AddModelError("Content", "Message content cannot be empty.");
                return View();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (message.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();  // Only the message owner or Admin can edit the message
            }

            // Update the message content
            message.Content = content;
            message.Date = DateTime.Now;  // Optional: Update the date to the current time
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();

            return Redirect("/Messages/Index/" + message.ChannelId); // Redirect to the channel's message list
        }

    }
}
