using LibraryAutomation.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.DAL.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        
        Task<List<Book>> GetBooksByAuthorAsync(string author);
    }
}
