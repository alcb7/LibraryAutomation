using System.ComponentModel.DataAnnotations;

namespace LibraryAutomation.MVC.Models.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrar alanı zorunludur.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Rol seçmek zorunludur.")]
        public string Role { get; set; }

        public List<string> AvailableRoles { get; set; } = new List<string> { "Admin", "Kütüphane Görevlisi", "Kullanıcı" };
    }
}
