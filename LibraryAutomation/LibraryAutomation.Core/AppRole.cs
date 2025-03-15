using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.Core
{
    public class AppRole : IdentityRole
    {
        public string FullName { get; set; } = string.Empty;
    }
}
