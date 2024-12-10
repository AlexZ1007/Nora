using Microsoft.AspNetCore.Identity;

namespace Nora.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<UserChannel>? UserChannels { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

    }
}
