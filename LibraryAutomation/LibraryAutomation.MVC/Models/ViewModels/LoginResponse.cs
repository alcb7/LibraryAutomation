namespace LibraryAutomation.MVC.Models.ViewModels
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public List<string> Roles { get; set; }
    }
}
