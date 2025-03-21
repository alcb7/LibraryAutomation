﻿using LibraryAutomation.Core.Entity;
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
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Book>> GetBooksByAuthorAsync(string author)
        {
            return await _context.Books
                .Where(b => b.Author == author)
                .ToListAsync();
        }
        public async Task<List<Rental>> GetRentalHistoryByBookIdAsync(int bookId)
        {
            return await _context.Rentals
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.BookId == bookId)
                .ToListAsync();
        }
    }
}
