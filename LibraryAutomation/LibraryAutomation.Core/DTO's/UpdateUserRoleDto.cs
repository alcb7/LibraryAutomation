using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.Core.DTO_s
{
    public class UpdateUserRoleDto
    {
        public string UserId { get; set; }
        public string NewRole { get; set; }
    }
}
