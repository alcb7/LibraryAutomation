using System.ComponentModel.DataAnnotations;

namespace LibraryAutomation.MVC.Models.ViewModels
{
    public class CreateBookViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public int? PublicationYear { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }
    }
}
