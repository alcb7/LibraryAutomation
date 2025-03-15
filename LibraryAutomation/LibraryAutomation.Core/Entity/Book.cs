using LibraryAutomation.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.Core.Entity
{
    public class Book
    {
        public int Id { get; set; }              // Birincil anahtar
        public string Title { get; set; }            // Kitap adı
        public string Author { get; set; }           // Yazar adı
        public string ISBN { get; set; }             // Eşsiz ISBN numarası
        public int? PublicationYear { get; set; }    // Yayın yılı (opsiyonel)
        public string Publisher { get; set; }        // Yayıncı (opsiyonel)
        public string Description { get; set; }      // Kitap açıklaması (opsiyonel)
        public BookStatus Status { get; set; }       // Kitabın durumu
    }

}
