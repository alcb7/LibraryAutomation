using LibraryAutomation.Core.Entity;
using LibraryAutomation.DAL.Contexts;
using LibraryAutomation.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.DAL.Repositories
{
    public class RentalRepository : Repository<Rental>, IRentalRepository
    {
        public RentalRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Rental>> GetRentalsByUserIdAsync(string userId)
        {
            return await _context.Rentals
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)  // 🔹 Kitap bilgilerini de çek
                .Include(r => r.User)  // 🔹 Kullanıcı bilgilerini de çek
                .ToListAsync();
        }


        public async Task<List<Rental>> GetRentalHistoryByBookIdAsync(int bookId)
        {
            return await _context.Rentals
                .Where(r => r.BookId == bookId)
                .Include(r => r.Book)  // 🔹 Kitap bilgilerini de çek
                .Include(r => r.User)  // 🔹 Kullanıcı bilgilerini de çek
                .ToListAsync();
        }
        public async Task<List<Rental>> GetRentalsWithBooksAsync(string userId)
        {
            return await _context.Rentals
                .Include(r => r.Book)  // Kitap bilgilerini getir
                .Include(r => r.User)  // Kullanıcı bilgilerini getir
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
        public async Task<List<Rental>> GetActiveRentalsByUserEmailAsync(string userEmail)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return new List<Rental>(); // Kullanıcı yoksa boş liste döndür

            var rentals = await _context.Rentals
                .Where(r => r.UserId == user.Id && r.ReturnDate == null)
                .Include(r => r.Book) // Kitap bilgilerini tam yükle
                .Include(r => r.User) // Kullanıcı bilgilerini de al
                .ToListAsync();

            // 📌 Log ile kontrol edelim
            foreach (var rental in rentals)
            {
                Console.WriteLine($"📌 (Repository) Rental ID: {rental.Id}, Book ID: {rental.BookId}, User ID: {rental.UserId}");
            }

            return rentals;
        }

        public async Task<List<Rental>> GetPastRentalsByUserEmailAsync(string userEmail)
        {
            var user = await _context.Set<AppUser>().FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return new List<Rental>(); // Kullanıcı yoksa boş liste döndür

            return await _context.Set<Rental>()
                .Where(r => r.UserId == user.Id && r.ReturnDate != null)
                .Include(r => r.Book)
                .ToListAsync();
        }
    }
}
