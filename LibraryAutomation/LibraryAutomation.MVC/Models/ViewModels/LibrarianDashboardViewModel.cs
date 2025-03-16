namespace LibraryAutomation.MVC.Models.ViewModels
{
    public class LibrarianDashboardViewModel
    {
        public List<BookViewModel> Books { get; set; } = new List<BookViewModel>();
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }
}
