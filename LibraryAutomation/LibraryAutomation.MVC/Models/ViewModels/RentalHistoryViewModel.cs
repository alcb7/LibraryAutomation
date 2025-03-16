namespace LibraryAutomation.MVC.Models.ViewModels
{
    public class RentalHistoryViewModel
    {
        public string BookTitle { get; set; }
        public string BorrowerEmail { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
