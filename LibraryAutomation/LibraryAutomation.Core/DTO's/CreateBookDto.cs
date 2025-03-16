using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.Core.DTO_s
{
   public class CreateBookDto
    {
        public string Title { get; set; }              // Kitap Adı
        public string Author { get; set; }            // Yazar
        public int? PublicationYear { get; set; }     // Yayın Yılı (Opsiyonel)
        public string? Publisher { get; set; }        // Yayıncı (Opsiyonel)
        public string? Description { get; set; }      // Açıklama (Opsiyonel)
    }
}
