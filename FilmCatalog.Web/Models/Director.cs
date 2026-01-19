using System.ComponentModel.DataAnnotations;

namespace FilmCatalog.Web.Models
{
    public class Director
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я режисера є обов'язковим")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Ім'я має бути від 2 до 150 символів")]
        [Display(Name = "Ім'я режисера")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Назва країни не може перевищувати 100 символів")]
        [Display(Name = "Країна")]
        public string? Country { get; set; }

        public ICollection<Film>? Films { get; set; }
    }
}
