using LibraryAutomation.MVC.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace LibraryAutomation.MVC.Controllers
{
    public class LibrarianController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LibrarianController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("❌ Token bulunamadı! Kullanıcı giriş yapmamış.");
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Kullanıcıları API'den çek
            var usersResponse = await client.GetAsync("librarian/users");
            List<UserViewModel> users = new List<UserViewModel>();

            if (usersResponse.IsSuccessStatusCode)
            {
                users = await usersResponse.Content.ReadFromJsonAsync<List<UserViewModel>>();
            }

            // Kitapları API'den çek
            var booksResponse = await client.GetAsync("books");
            List<BookViewModel> books = new List<BookViewModel>();

            if (booksResponse.IsSuccessStatusCode)
            {
                books = await booksResponse.Content.ReadFromJsonAsync<List<BookViewModel>>();
            }

            // 📌 **Yeni ViewModel'i Kullan**
            var model = new LibrarianDashboardViewModel
            {
                Users = users,
                Books = books
            };

            return View(model);
        }

        // Kitap ekleme sayfası
        public IActionResult CreateBook()
        {
            return View(new CreateBookViewModel());
        }

        // Kitap ekleme işlemi
        [HttpPost]
        public async Task<IActionResult> CreateBook(CreateBookViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("books/add-book", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Kitap eklenirken hata oluştu!";
                return View(model);
            }

            return RedirectToAction("Index");
        }

        // Kitap silme işlemi
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"books/{bookId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Kitap silinemedi!";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Kitap başarıyla silindi.";
            return RedirectToAction("Index");
        }

        // Kiralama geçmişini görüntüleme
        public async Task<IActionResult> RentalHistory(int bookId)
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"rentals/rental-history/{bookId}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Kiralama geçmişi alınamadı!";
                return View(new List<RentalHistoryViewModel>());
            }

            var rentals = await response.Content.ReadFromJsonAsync<List<RentalHistoryViewModel>>();
            return View(rentals);
        }
        [HttpGet("users")]
        public async Task<IActionResult> Users()
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("librarian/users");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Kullanıcı listesi alınamadı!";
                return RedirectToAction("Index");
            }

            var users = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
            return View(users);
        }

        // 📌 Kullanıcının Kiraladığı Kitapları Listele
        [HttpGet("user-rentals/{userId}")]
        public async Task<IActionResult> UserRentals(string userId)
        {
            var client = _httpClientFactory.CreateClient("LibraryApi");
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"rentals/user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Kullanıcının kiraladığı kitapları getirme başarısız!";
                return View(new List<RentalHistoryViewModel>());
            }

            var rentals = await response.Content.ReadFromJsonAsync<List<RentalHistoryViewModel>>();
            return View(rentals);
        }
    }
}
