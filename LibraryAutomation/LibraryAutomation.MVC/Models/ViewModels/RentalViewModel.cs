namespace LibraryAutomation.MVC.Models.ViewModels
{
    public class RentalViewModel
    {
        public int RentalId { get; set; } // Kiralama ID'si

        public string BookTitle { get; set; } // Kitap Adı
        public string BorrowerEmail { get; set; } // Kullanıcı Email
        public DateTime RentalDate { get; set; } // Kiralama Tarihi
        public DateTime? DueDate { get; set; } // Son İade Tarihi
        public DateTime? ReturnDate { get; set; } // Geri İade Tarihi (Eğer iade edilmediyse null)
    }
}
