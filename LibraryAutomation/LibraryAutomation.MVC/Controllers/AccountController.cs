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

            var response = await _httpClient.PostAsync("Account/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Geçersiz giriş bilgileri.");
                return View(model);
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            // **Gelen yanıtın boş olup olmadığını kontrol edelim**
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                ModelState.AddModelError("", "Sunucudan geçersiz bir yanıt alındı.");
                return View(model);
            }

            var result = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

            // **Eğer result veya result.Roles null ise, güvenli şekilde hata gösterelim**
            if (result == null || result.Roles == null)
            {
                ModelState.AddModelError("", "Giriş işlemi sırasında beklenmeyen bir hata oluştu.");
                return View(model);
            }

            // **Token'ı Session'a kaydet**
            HttpContext.Session.SetString("Token", result.Token);

            // **Rolleri kontrol edip yönlendirme yap**
            if (result.Roles.Contains("Admin"))
                return RedirectToAction("Index", "Admin");
            if (result.Roles.Contains("Kütüphane Görevlisi"))
                return RedirectToAction("Index", "Librarian");
            if (result.Roles.Contains("Kullanıcı"))
                return RedirectToAction("Index", "User");

            return RedirectToAction("Login");
        }
    }

}
