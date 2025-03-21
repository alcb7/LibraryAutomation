﻿namespace LibraryAutomation.MVC.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsApproved { get; set; }
        public List<string> Roles { get; set; } // Kullanıcı rollerini liste olarak tutalım
    }
}
