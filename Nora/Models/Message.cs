using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nora.Models
{
	public class Message
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Mesajul nu poate fi gol")]
		public string Content {  get; set; }


		public DateTime Date { get; set; }

        public string? UserId { get; set; }

		public int ChannelId { get; set; }


		public virtual Channel? Channel { get; set; }
		public virtual ApplicationUser? User { get; set; }
        public bool IsDeleted { get; set; }

		[NotMapped]
		public string? EmbeddedUrl { get; set; }
    }
}
