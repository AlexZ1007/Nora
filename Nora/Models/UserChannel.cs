using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nora.Models
{
    public class UserChannel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //public int? ChategoryId { get; set; }
        public string? UserId { get; set; }
        public int? ChannelId { get; set; }
        public bool IsModerator { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime JoinDate { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Channel? Channel { get; set; }
    }
}
