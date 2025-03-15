using LibraryAutomation.Core.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.DAL.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Rental> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Identity tablolarının yapılandırılması için temel ayarlar
            base.OnModelCreating(builder);

            // Book entity ayarları
            builder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title)
                      .IsRequired();
                entity.Property(b => b.ISBN)
                      .IsRequired();
            });

            // BookRental entity ayarları
            builder.Entity<Rental>(entity =>
            {
                entity.HasKey(br => br.Id);

                // Book ile ilişki: Her kiralama kaydı bir kitaba bağlı
                entity.HasOne(br => br.Book)
                      .WithMany()
                      .HasForeignKey(br => br.BookId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Kullanıcı ile ilişki: Her kiralama kaydı bir kullanıcıya bağlı
                entity.HasOne(br => br.User)
                      .WithMany()
                      .HasForeignKey(br => br.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

