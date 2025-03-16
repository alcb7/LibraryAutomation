using LibraryAutomation.MVC.Models.ViewModels;
using LibraryAutomation.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace LibraryAutomation.MVC.Controllers
{
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
                return RedirectToAction("Login", "Auth");
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
        [HttpPost]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");

            // ✅ **Session’dan Token’ı Al**
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("❌ Token BULUNAMADI! Kullanıcı giriş yapmalı.");
                return RedirectToAction("Login", "Account");
            }

            // ✅ **Token’i API'ye gönderirken Authorization Header'a ekle**
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine("📌 API'ye Gönderilen Authorization Header: Bearer " + token);

            // ✅ **API’ye istek gönder**
            var response = await client.PostAsync($"admin/approve-user/{userId}", null);

            Console.WriteLine("📌 API Yanıt Kodu: " + response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine("❌ API Hatası: " + response.StatusCode + " - " + errorMessage);
                TempData["Error"] = "Kullanıcı onaylama başarısız.";
            }
            else
            {
                TempData["Success"] = "Kullanıcı başarıyla onaylandı.";
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult CreateUser()
        {
            var model = new CreateUserViewModel
            {
                AvailableRoles = new List<string> { "Admin", "Kütüphane Görevlisi", "Kullanıcı" } // 🔹 Rol listesi
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = new List<string> { "Admin", "Kütüphane Görevlisi", "Kullanıcı" }; // 🔹 Rolleri tekrar dolduralım
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("admin/create-user", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Kullanıcı eklenemedi.";
                return View(model);
            }

            TempData["Success"] = "Kullanıcı başarıyla eklendi.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"admin/delete-user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Kullanıcı silinemedi!";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Kullanıcı başarıyla silindi.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(string userId, string newRole)
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var updateRoleDto = new { UserId = userId, NewRole = newRole };
            var jsonContent = JsonContent.Create(updateRoleDto); // 🚀 Daha temiz JSON veri gönderimi

            var response = await client.PutAsync("admin/update-role", jsonContent); // 🚨 POST yerine PUT kullan

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Rol güncellenemedi!";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Kullanıcı rolü başarıyla güncellendi.";
            return RedirectToAction("Index");
        }







    }

}
