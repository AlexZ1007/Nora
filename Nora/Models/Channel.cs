using System.ComponentModel.DataAnnotations;

namespace Nora.Models
{
	public class Channel
	{
		[Key]
		public int Id { get; set; }


		[Required(ErrorMessage = "Titlul este obligatoriu")]
		[StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
		[MinLength(5, ErrorMessage = "Titlul trebuie sa aiba mai mult de 5 caractere")]
		public string Title { get; set; }


		[Required(ErrorMessage = "Descrierea canalului este obligatorie")]
		public string Description { get; set; }

        public string? UserId { get; set; }

        public DateTime Date { get; set; }


		//[Required(ErrorMessage = "Categoria este obligatorie")]

		public virtual ICollection<CategoryChannel>? CategoryChannels { get; set; }
		public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<UserChannel>? UserChannels { get; set; }
        public virtual ApplicationUser? User { get; set; }

    }
}
