using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using LibraryAutomation.MVC.Models.ViewModels;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

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
            var client = _httpClientFactory.CreateClient("LibraryApi");

            Console.WriteLine("📌 API'ye Login isteği gönderiliyor: " + model.Email);
            var response = await client.PostAsJsonAsync("account/login", model);
            Console.WriteLine("📌 API Login Yanıt Kodu: " + response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Giriş başarısız, lütfen bilgilerinizi kontrol edin.";
                Console.WriteLine("❌ API Login Başarısız! StatusCode: " + response.StatusCode);
                return View();
            }

            var result = await response.Content.ReadFromJsonAsync<JwtResponseDto>();
            Console.WriteLine("📌 Alınan Token: " + result.Token);

            // ✅ **Token'ı Cookie olarak kaydediyoruz**
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, model.Email),
            new Claim(ClaimTypes.Name, model.Email),
            new Claim(ClaimTypes.Role, "Admin") // **Burada API'den dönen gerçek rolü alabilirsin**
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            Console.WriteLine("✅ Kullanıcı Yetkilendirildi ve Cookie Kaydedildi!");

            return RedirectToAction("Index", "Admin");
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
