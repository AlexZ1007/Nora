using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class AtLeastOneCategorySelectedAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is List<int> selectedCategories && selectedCategories.Any())
        {
            return ValidationResult.Success;
        }
        return new ValidationResult("Trebuie să selectați cel puțin o categorie.");
    }
}




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


        public virtual ICollection<CategoryChannel>? CategoryChannels { get; set; }

        [NotMapped]
        [AtLeastOneCategorySelected(ErrorMessage = "Trebuie să selectați cel puțin o categorie.")]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<UserChannel>? UserChannels { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [NotMapped]
        public bool IsUserMember { get; set; }
        [NotMapped]
        public bool IsPending { get; set; }
        [NotMapped]
        public string CreatorUserId { get; set; }


    }
}