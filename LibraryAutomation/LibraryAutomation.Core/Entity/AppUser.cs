using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.Core.Entity
{
    public class AppUser : IdentityUser
    {
        // Ek özellikler buraya eklenebilir
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsApproved { get; set; }
    }
}
