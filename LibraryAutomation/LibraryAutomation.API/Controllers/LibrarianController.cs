using LibraryAutomation.Business.Interfaces;
using LibraryAutomation.Core.DTO_s;
using LibraryAutomation.Core.Entity;
using LibraryAutomation.Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAutomation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Librarian)] // Sadece kütüphane görevlisi erişebilir
    public class LibrarianController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IRentalService _rentalService;
        private readonly UserManager<AppUser> _userManager;


       

        public LibrarianController(IBookService bookService, IRentalService rentalService, UserManager<AppUser> userManager)
        {
            _bookService = bookService;
            _rentalService = rentalService;
            _userManager = userManager;
        }

        // 📚 Tüm Kitapları Listeleme
        [HttpGet("books")]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        // 📕 Yeni Kitap Ekleme
        [HttpPost("add-book")]
        public async Task<IActionResult> AddBook([FromBody] CreateBookDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newBook = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                PublicationYear = dto.PublicationYear,
                Publisher = dto.Publisher,
                Description = dto.Description,
                Status = BookStatus.Available // 📌 Varsayılan olarak "Available" olacak
            };

            await _bookService.AddBookAsync(newBook);
            return Ok(new { message = "Kitap başarıyla eklendi." });
        }

        // ✏ Kitap Güncelleme
        [HttpPut("update-book/{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] CreateBookDto dto)
        {
            var existingBook = await _bookService.GetBookByIdAsync(id);
            if (existingBook == null)
                return NotFound(new { message = "Kitap bulunamadı." });

            existingBook.Title = dto.Title;
            existingBook.Author = dto.Author;
            existingBook.PublicationYear = dto.PublicationYear;
            existingBook.Publisher = dto.Publisher;
            existingBook.Description = dto.Description;

            await _bookService.UpdateBookAsync(existingBook);
            return Ok(new { message = "Kitap başarıyla güncellendi." });
        }

        // ❌ Kitap Silme
        [HttpDelete("delete-book/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteBookAsync(id);
            return Ok(new { message = "Kitap başarıyla silindi." });
        }

        // 📜 Kiralanan Kitapları Listeleme
        [HttpGet("rental-list")]
        public async Task<IActionResult> GetRentalList()
        {
            var rentals = await _rentalService.GetAllRentalsAsync();
            return Ok(rentals);
        }
        [HttpGet("rental-history/{bookId}")]
        public async Task<IActionResult> GetRentalHistoryByBook(int bookId)
        {
            var rentals = await _rentalService.GetRentalHistoryByBookIdAsync(bookId);

            if (!rentals.Any())
            {
                return Ok(new { Message = "Bu kitap daha önce kiralanmamış." });
            }

            return Ok(rentals);
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var userList = users.Select(user => new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName
            }).ToList();

            return Ok(userList);
        }
    }
}
