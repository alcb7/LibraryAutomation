using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.Core.Entity
{
    public class Rental
    {
        public int Id { get; set; }            // Birincil anahtar
        public int BookId { get; set; }              // İlgili kitabın ID'si (Foreign Key)
        public string UserId { get; set; }           // Kiralayan kullanıcının ID'si (Foreign Key)
        public DateTime RentalDate { get; set; }     // Kiralama tarihi
        public DateTime? DueDate { get; set; }         // Teslim tarihi (opsiyonel)
        public DateTime? ReturnDate { get; set; }      // İade tarihi (opsiyonel)

        // Navigation property'ler
        public Book Book { get; set; }
        public AppUser User { get; set; }
    }
}
