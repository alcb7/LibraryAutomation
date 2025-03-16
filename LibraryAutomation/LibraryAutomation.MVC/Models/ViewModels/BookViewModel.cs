namespace LibraryAutomation.MVC.Models.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }              // Birincil anahtar
        public string Title { get; set; }            // Kitap adı
        public string Author { get; set; }           // Yazar adı
       
        public int? PublicationYear { get; set; }    // Yayın yılı (opsiyonel)
        public string Publisher { get; set; }        // Yayıncı (opsiyonel)
        public string Description { get; set; }      // Kitap açıklaması (opsiyonel)
        public BookStatus Status { get; set; }       // Kitabın durumu
    }
}
