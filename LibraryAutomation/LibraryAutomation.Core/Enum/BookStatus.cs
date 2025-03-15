using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.Core.Enum
{
    public enum BookStatus
    {
        Available,  // Mevcut
        Rented,     // Kirada
        Unavailable // Örneğin, bakımda veya kullanılamaz durumda
    }
}
