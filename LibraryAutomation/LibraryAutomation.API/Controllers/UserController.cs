using LibraryAutomation.Core.DTO_s;
using LibraryAutomation.Core.Entity;
using LibraryAutomation.Core.Enum;
using LibraryAutomation.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryAutomation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.User)]  // 🔹 Sadece kullanıcılar erişebilir
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;


        

        public UserController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // 📌 1️⃣ Kullanıcı tüm kitapları listeleyebilir
        [HttpGet("books")]
        public async Task<IActionResult> GetAvailableBooks()
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            return Ok(books);
        }

        [HttpPost("rent-book/{bookId}")]
        public async Task<IActionResult> RentBook(int bookId)
        {
            // 📌 Kullanıcı ID veya Email'i token içinden al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Eğer email ID ise, değiştir

            // ❌ Kullanıcı giriş yapmamışsa Unauthorized döndür
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği alınamadı! Giriş yapmanız gerekiyor." });
            }

            // 📌 Kullanıcıyı email ID ile bulmayı dene
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userId); // 📌 Eğer ID değilse, email olarak ara
            }

            if (user == null)
            {
                return NotFound(new { Message = "Kullanıcı bulunamadı!" });
            }

            // 📌 Kitabı veritabanından al
            var book = await _unitOfWork.Books.GetByIdAsync(bookId);
            if (book == null)
            {
                return NotFound(new { Message = "Kitap bulunamadı!" });
            }

            // ❌ Kitap kiralanmışsa hata döndür
            if (book.Status != BookStatus.Available)
            {
                return BadRequest(new { Message = "Bu kitap şu an mevcut değil!" });
            }

            // 📌 Kiralama kaydı oluştur
            var rental = new Rental
            {
                BookId = bookId,
                UserId = user.Id, // 📌 Doğru GUID ID'yi kullan
                RentalDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                ReturnDate = null
            };

            book.Status = BookStatus.Rented;

            await _unitOfWork.Rentals.AddAsync(rental);
            await _unitOfWork.Books.UpdateAsync(book);
            await _unitOfWork.SaveAsync();

            return Ok(new { Message = "Kitap başarıyla kiralandı." });
        }

        [HttpGet("my-rentals")]
        public async Task<IActionResult> GetMyRentals()
        {
            // 📌 Kullanıcı ID'sini doğru aldığımızdan emin ol
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Giriş yapmanız gerekiyor!" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userId); // 📌 Eğer ID değilse, email olarak ara
            }

            if (user == null)
            {
                return NotFound(new { Message = "Kullanıcı bulunamadı!" });
            }

            // 📌 Kiralamaları User ve Book bilgileriyle birlikte getir
            var rentals = await _unitOfWork.Rentals.GetRentalsWithBooksAsync(user.Id);

            if (!rentals.Any())
            {
                return Ok(new { Message = "Henüz kiralanmış kitabınız yok." });
            }

            var rentalDetails = rentals.Select(r => new RentalDetailsDto
            {
                Id = r.Id,
                BookTitle = r.Book?.Title ?? "Bilinmiyor",
                BorrowerEmail = r.User?.Email ?? "Bilinmiyor",
                RentalDate = r.RentalDate,
                DueDate = r.DueDate,
                ReturnDate = r.ReturnDate
            }).ToList();

            return Ok(rentalDetails);
        }


        [HttpPost("return-book/{rentalId}")]
        public async Task<IActionResult> ReturnBook(int rentalId)
        {
            // 📌 Kullanıcı ID'sini al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Giriş yapmanız gerekiyor!" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userId);
            }

            if (user == null)
            {
                return NotFound(new { Message = "Kullanıcı bulunamadı!" });
            }

            // 📌 Kiralama kaydını al
            var rental = await _unitOfWork.Rentals.GetByIdAsync(rentalId);

            if (rental == null)
            {
                return NotFound(new { Message = "Kiralama kaydı bulunamadı!" });
            }

            // ❌ Kullanıcının bu kitabı kiralayıp kiralamadığını kontrol et
            if (rental.UserId != user.Id)
            {
                return BadRequest(new { Message = "Bu kitabı siz kiralamadınız!" });
            }

            // 📌 Kitap durumunu güncelle
            rental.ReturnDate = DateTime.UtcNow;
            var book = await _unitOfWork.Books.GetByIdAsync(rental.BookId);

            if (book != null)
            {
                book.Status = BookStatus.Available;
                await _unitOfWork.Books.UpdateAsync(book);
            }

            await _unitOfWork.Rentals.UpdateAsync(rental);
            await _unitOfWork.SaveAsync();

            return Ok(new { Message = "Kitap başarıyla iade edildi." });
        }
        [HttpGet("my-active-rentals")]
        public async Task<IActionResult> GetMyActiveRentals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Giriş yapmanız gerekiyor!" });
            }

            var activeRentals = await _unitOfWork.Rentals.GetActiveRentalsByUserEmailAsync(userId);

            var result = activeRentals.Select(r => new
            {
                RentalId = r.Id,
                BookId = r.BookId,
                BookTitle = r.Book.Title,
                RentalDate = r.RentalDate,
                DueDate = r.DueDate
            }).ToList();

            return Ok(result);
        }
        [HttpGet("my-past-rentals")]
        public async Task<IActionResult> GetMyPastRentals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Giriş yapmanız gerekiyor!" });
            }

            var pastRentals = await _unitOfWork.Rentals.GetPastRentalsByUserEmailAsync(userId);

            var result = pastRentals.Select(r => new
            {
                RentalId = r.Id,
                BookId = r.BookId,
                BookTitle = r.Book.Title,
                RentalDate = r.RentalDate,
                DueDate = r.DueDate,
                ReturnDate = r.ReturnDate
            }).ToList();

            return Ok(result);
        }
    }
}
