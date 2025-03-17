using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LibraryAutomation.Core.DTO_s
{
    public class RentalDto
    {
        [JsonPropertyName("rentalId")]  // 📌 JSON Key ile birebir eşleşmeyi sağlıyoruz
        public int RentalId { get; set; }

        [JsonPropertyName("bookId")]
        public int BookId { get; set; }

        [JsonPropertyName("bookTitle")]
        public string BookTitle { get; set; }

        [JsonPropertyName("borrowerEmail")]
        public string BorrowerEmail { get; set; }

        [JsonPropertyName("rentalDate")]
        public DateTime RentalDate { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("returnDate")]
        public DateTime? ReturnDate { get; set; }
    }
}
