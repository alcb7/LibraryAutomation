using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using LibraryAutomation.MVC.Models.ViewModels;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace LibraryAutomation.MVC.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7009/api/account/login", model);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Giriş başarısız, lütfen bilgilerinizi kontrol edin.";
                return View();
            }

            var result = await response.Content.ReadFromJsonAsync<JwtResponseDto>();

            // Token'ı sakla
            HttpContext.Session.SetString("Token", result.Token);

            // Token'ı çözümleyerek rolü al
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result.Token);
            var role = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Kullanıcıyı rolüne göre yönlendir
            if (role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (role == "Kütüphane Görevlisi")
            {
                return RedirectToAction("Dashboard", "Librarian");
            }
            else
            {
                return RedirectToAction("Home", "User");
            }
        }
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.Error = "Şifreler eşleşmiyor.";
                return View();
            }

            var client = _httpClientFactory.CreateClient("LibraryApi"); // Merkezi tanımlanan HTTP istemciyi kullan
            var response = await client.PostAsJsonAsync("account/register", model); // API'ye kayıt isteği gönder

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Kayıt başarısız, lütfen bilgilerinizi kontrol edin.";
                return View();
            }

            ViewBag.Success = "Kayıt başarılı, giriş yapabilirsiniz!";
            return RedirectToAction("Login");
        }
    }

}
