using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nora.Models
{
	public class CategoryChannel
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
		//public int? ChategoryId { get; set; }
		public int? CategoryId { get; set; }
		public int? ChannelId { get; set; }

		public virtual Category? Category { get; set; }
		public virtual Channel? Channel { get; set; }
	}
}
