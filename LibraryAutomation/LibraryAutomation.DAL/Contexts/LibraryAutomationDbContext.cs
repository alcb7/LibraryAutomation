using LibraryAutomation.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.DAL.Contexts
{
    public class LibraryAutomationDbContext : IdentityDbContext<AppUser>
    {
        public LibraryAutomationDbContext(DbContextOptions<LibraryAutomationDbContext> options) : base(options) { }

        // public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
