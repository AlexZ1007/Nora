using System.ComponentModel.DataAnnotations;

namespace Nora.Models
{
	public class Message
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Mesajul nu poate fi gol")]
		public string Content {  get; set; }


		public DateTime Date { get; set; }


		public int ChannelId { get; set; }


		public virtual Channel? Channel { get; set; }


		//public string? UserId { get; set; }
		//public virtual ApplicationUser? User { get; set; }

	}
}
