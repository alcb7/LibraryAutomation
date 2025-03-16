using LibraryAutomation.DAL.Contexts;
using LibraryAutomation.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IBookRepository _bookRepository;
        private IRentalRepository _rentalRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IBookRepository Books => _bookRepository ??= new BookRepository(_context);
        public IRentalRepository Rentals => _rentalRepository ??= new RentalRepository(_context);

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync(); // Tüm değişiklikleri kaydet
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
