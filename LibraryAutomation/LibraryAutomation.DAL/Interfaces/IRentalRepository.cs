using LibraryAutomation.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.DAL.Interfaces
{
    public interface IRentalRepository : IRepository<Rental>
    {
        // Kiralama entity'sine özel metodlar
        Task<List<Rental>> GetRentalsByUserIdAsync(string userId);
        Task<List<Rental>> GetRentalHistoryByBookIdAsync(int bookId);
        Task<List<Rental>> GetRentalsWithBooksAsync(string userId);
        Task<List<Rental>> GetActiveRentalsByUserEmailAsync(string userEmail);
        Task<List<Rental>> GetPastRentalsByUserEmailAsync(string userEmail);
    }
}
