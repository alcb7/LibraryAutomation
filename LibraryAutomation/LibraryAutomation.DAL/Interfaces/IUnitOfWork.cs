using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Books { get; }
        IRentalRepository Rentals { get; }

        Task<int> SaveAsync(); // Tüm işlemleri tek bir transaction içinde kaydeder
    }
}
