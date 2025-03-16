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
    }
}
