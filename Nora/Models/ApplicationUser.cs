using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nora.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<UserChannel>? UserChannels { get; set; }
        public virtual ICollection<Message> Messages { get; set; }


        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}
