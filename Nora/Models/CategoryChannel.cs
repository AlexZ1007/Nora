namespace Nora.Models
{
	public class CategoryChannel
	{
		public int Id { get; set; }
		public int? ChategoryId { get; set; }
		public int? ChannelId { get; set; }

		public virtual Category? Category { get; set; }
		public virtual Channel? Channel { get; set; }
	}
}
