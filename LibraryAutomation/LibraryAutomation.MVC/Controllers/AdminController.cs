using LibraryAutomation.MVC.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAutomation.MVC.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = "Admin")] // Sadece Admin erişebilir
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Kullanıcıları listele
        [HttpGet("users")]
        public async Task<IActionResult> Users()
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var response = await client.GetAsync("admin/users");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error", new { Message = "Kullanıcıları getirme başarısız." });
            }

            var users = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
            return View(users);
        }

        // Kullanıcıyı onayla
        [HttpPost("approve/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var response = await client.PostAsync($"admin/approve-user/{userId}", null);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Onay işlemi başarısız.";
            }

            return RedirectToAction("Users");
        }
    }
}
