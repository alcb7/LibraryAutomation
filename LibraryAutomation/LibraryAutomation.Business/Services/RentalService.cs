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
        private readonly IRentalRepository _rentalRepository;

        public RentalService(IRentalRepository rentalRepository)
        {
            _rentalRepository = rentalRepository;
        }

        public async Task<List<Rental>> GetAllRentalsAsync()
        {
            return await _rentalRepository.GetAllAsync();
        }

        public async Task<Rental> GetRentalByIdAsync(int id)
        {
            return await _rentalRepository.GetByIdAsync(id);
        }

        public async Task AddRentalAsync(Rental rental)
        {
            await _rentalRepository.AddAsync(rental);
        }

        public async Task UpdateRentalAsync(Rental rental)
        {
            await _rentalRepository.UpdateAsync(rental);
        }

        public async Task DeleteRentalAsync(int id)
        {
            await _rentalRepository.DeleteAsync(id);
        }

        public async Task<List<Rental>> GetRentalsByUserIdAsync(string userId)
        {
            return await _rentalRepository.GetRentalsByUserIdAsync(userId);
        }
    }
}
