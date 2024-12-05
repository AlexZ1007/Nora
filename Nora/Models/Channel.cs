using System.ComponentModel.DataAnnotations;

namespace Nora.Models
{
	public class Channel
	{
		[Key]
		public string Id { get; set; }


		[Required(ErrorMessage = "Titlul este obligatoriu")]
		[StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
		[MinLength(5, ErrorMessage = "Titlul trebuie sa aiba mai mult de 5 caractere")]
		public string Title { get; set; }


		[Required(ErrorMessage = "Descrierea canalului este obligatoriu")]
		public string Description { get; set; }


		public DateTime Date { get; set; }


		[Required(ErrorMessage = "Categoria este obligatorie")]
		public virtual ICollection<CategoryChannel>? CategoryChannels { get; set; }


		//vom avea mai multe comentarii
		public virtual ICollection<Message> Messages { get; set; }
	}
}
