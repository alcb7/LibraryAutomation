using LibraryAutomation.MVC.Models.ViewModels;
using LibraryAutomation.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace LibraryAutomation.MVC.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin kullanıcılar erişebilir
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");

            // Session’dan token'ı al
            var token = HttpContext.Session.GetString("Token");
            Console.WriteLine("📌 Session’dan Okunan Token: " + (token ?? "YOK!"));

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("❌ HATA: Session’dan Token okunamadı!");
                return RedirectToAction("Login", "Auth");
            }

            // **Token'in başına 'Bearer ' eklediğimizden emin olalım!**
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            Console.WriteLine("📌 API'ye Gönderilen Authorization Header: " + client.DefaultRequestHeaders.Authorization);

            var response = await client.GetAsync("admin/users");

            Console.WriteLine("📌 API Yanıt Kodu: " + response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ API yetkilendirme hatası! StatusCode: " + response.StatusCode);
                return View("Error", new ErrorViewModel { Message = "Yetkilendirme hatası: API'den kullanıcı listesi alınamadı." });
            }

            var users = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
            return View(users);
        }

    }
}
