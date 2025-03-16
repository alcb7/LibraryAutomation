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

            // ✅ **Session’dan Token’ı Al**
            var token = HttpContext.Session.GetString("Token");
            Console.WriteLine("📌 Session’dan Okunan Token: " + (token ?? "YOK!"));

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("❌ HATA: Token bulunamadı, giriş sayfasına yönlendiriliyor.");
                return RedirectToAction("Login", "Account");
            }

            // ✅ **API’ye gönderilen Token’ı doğru formatta gönderelim**
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            Console.WriteLine("📌 API'ye Gönderilen Authorization Header: " + client.DefaultRequestHeaders.Authorization);

            // ✅ API’ye İstek Yap
            var response = await client.GetAsync("admin/users");

            // ❌ **API Hata Verirse Log Yazalım**
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ API Hatası: {response.StatusCode} - {errorMessage}");

                ViewBag.Error = $"API Hatası: {response.StatusCode} - {errorMessage}";
                return View(new List<UserViewModel>()); // **Boş liste gönderiyoruz**
            }

            // ✅ Kullanıcı listesini al
            var users = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
            return View(users);
        }
       



    }
}
