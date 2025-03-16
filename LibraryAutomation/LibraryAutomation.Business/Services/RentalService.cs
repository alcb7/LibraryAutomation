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
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RentalService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Rental>> GetAllRentalsAsync()
        {
            return await _unitOfWork.Rentals.GetAllAsync();
        }

        public async Task<Rental> GetRentalByIdAsync(int id)
        {
            return await _unitOfWork.Rentals.GetByIdAsync(id);
        }

        public async Task AddRentalAsync(Rental rental)
        {
            await _unitOfWork.Rentals.AddAsync(rental);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateRentalAsync(Rental rental)
        {
            await _unitOfWork.Rentals.UpdateAsync(rental);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteRentalAsync(int id)
        {
            await _unitOfWork.Rentals.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<Rental>> GetRentalsByUserIdAsync(string userId)
        {
            return await _unitOfWork.Rentals.GetRentalsByUserIdAsync(userId);
        }
        public async Task<List<Rental>> GetRentalHistoryByBookIdAsync(int bookId)
        {
            return await _unitOfWork.Rentals.GetRentalHistoryByBookIdAsync(bookId);
        }
    }
}
