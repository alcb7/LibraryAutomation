using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using LibraryAutomation.MVC.Models.ViewModels;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace LibraryAutomation.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7009/api/"); // API URL
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("account/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Geçersiz giriş bilgileri.");
                return View(model);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

            if (result == null || string.IsNullOrWhiteSpace(result.Token))
            {
                ModelState.AddModelError("", "Sunucudan geçersiz bir yanıt alındı.");
                return View(model);
            }

            // ✅ **Token'i doğrudan kaydet (Etrafında hiçbir şey olmadan!)**
            string pureToken = result.Token.Trim();
            HttpContext.Session.SetString("Token", pureToken);

            // ✅ **Konsola sadece token'i yazdır**
            Console.WriteLine(pureToken);

            // ✅ **Rollere göre yönlendirme yap**
            if (result.Roles.Contains("Admin"))
                return RedirectToAction("Index", "Admin");
            if (result.Roles.Contains("Kütüphane Görevlisi"))
                return RedirectToAction("Index", "Librarian");
            if (result.Roles.Contains("Kullanıcı"))
                return RedirectToAction("Index", "User");

            return RedirectToAction("Login");
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Account/register", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu.");
                return View(model);
            }

            return RedirectToAction("Login"); // ✅ Kayıt başarılıysa login sayfasına yönlendir
        }
    }








}

