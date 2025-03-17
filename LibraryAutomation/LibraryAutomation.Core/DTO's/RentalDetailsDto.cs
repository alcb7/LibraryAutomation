using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.Core.DTO_s
{
    public class RentalDetailsDto
    {
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public string BorrowerEmail { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
