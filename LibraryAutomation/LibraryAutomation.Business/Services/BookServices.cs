using LibraryAutomation.Business.Interfaces;
using LibraryAutomation.Core.Entity;
using LibraryAutomation.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.Business.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _unitOfWork.Books.GetAllAsync();
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _unitOfWork.Books.GetByIdAsync(id);
        }

        public async Task AddBookAsync(Book book)
        {
            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateBookAsync(Book book)
        {
            await _unitOfWork.Books.UpdateAsync(book);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            await _unitOfWork.Books.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<Book>> GetBooksByAuthorAsync(string author)
        {
            return await _unitOfWork.Books.GetBooksByAuthorAsync(author);
        }
    }
}
