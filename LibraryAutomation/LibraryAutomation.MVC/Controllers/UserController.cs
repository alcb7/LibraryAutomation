using LibraryAutomation.MVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LibraryAutomation.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // 📌 1️⃣ Kullanılabilir Kitapları Listele
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("books");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Kitaplar yüklenemedi.";
                return View(new UserDashboardViewModel());
            }

            var books = await response.Content.ReadFromJsonAsync<List<BookViewModel>>();
            var viewModel = new UserDashboardViewModel { AvailableBooks = books };

            return View(viewModel);
        }

        // 📌 2️⃣ Kitap Kiralama İşlemi
        [HttpPost]
        public async Task<IActionResult> RentBook(int bookId)
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync($"user/rent-book/{bookId}", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Kitap kiralanamadı!";
            }
            else
            {
                TempData["Success"] = "Kitap başarıyla kiralandı!";
            }

            return RedirectToAction("Index");
        }

        // 📌 3️⃣ Kullanıcının Kiraladığı Kitapları Listele
        [HttpGet]
        public async Task<IActionResult> MyRentals()
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("user/my-rentals");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Kiraladığınız kitaplar yüklenemedi.";
                return View(new List<RentalViewModel>());
            }

            var rentals = await response.Content.ReadFromJsonAsync<List<RentalViewModel>>();
            return View(rentals);
        }

        // 📌 4️⃣ Kiralanan Kitabı İade Etme
        [HttpPost]
        public async Task<IActionResult> ReturnBook(int rentalId)
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync($"user/return-book/{rentalId}", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Kitap iade edilemedi!";
            }
            else
            {
                TempData["Success"] = "Kitap başarıyla iade edildi!";
            }

            return RedirectToAction("MyRentals");
        }
    }
}

