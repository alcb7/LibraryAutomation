using LibraryAutomation.Core.Entity;
using LibraryAutomation.Core.Enum;
using LibraryAutomation.DAL.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAutomation.DAL.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // 1️⃣ Roller (Admin, Librarian, User) ekle
            string[] roles = { Roles.Admin, Roles.Librarian, Roles.User };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2️⃣ Admin Kullanıcısı
            var adminEmail = "admin@library.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    IsApproved = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                }
            }

            // 3️⃣ Kütüphane Görevlileri
            var librarian1 = new AppUser
            {
                UserName = "librarian1@library.com",
                Email = "librarian1@library.com",
                FirstName = "Ayşe",
                LastName = "Görevli",
                EmailConfirmed = true,
                IsApproved = true
            };
            var librarian2 = new AppUser
            {
                UserName = "librarian2@library.com",
                Email = "librarian2@library.com",
                FirstName = "Mehmet",
                LastName = "Görevli",
                EmailConfirmed = true,
                IsApproved = true
            };

            if (await userManager.FindByEmailAsync(librarian1.Email) == null)
            {
                await userManager.CreateAsync(librarian1, "Librarian123!");
                await userManager.AddToRoleAsync(librarian1, Roles.Librarian);
            }
            if (await userManager.FindByEmailAsync(librarian2.Email) == null)
            {
                await userManager.CreateAsync(librarian2, "Librarian123!");
                await userManager.AddToRoleAsync(librarian2, Roles.Librarian);
            }

            // 4️⃣ Kullanıcılar
            var user1 = new AppUser
            {
                UserName = "user1@library.com",
                Email = "user1@library.com",
                FirstName = "Ali",
                LastName = "Okur",
                EmailConfirmed = true,
                IsApproved = true
            };
            var user2 = new AppUser
            {
                UserName = "user2@library.com",
                Email = "user2@library.com",
                FirstName = "Fatma",
                LastName = "Kitapsever",
                EmailConfirmed = true,
                IsApproved = true
            };
            var user3 = new AppUser
            {
                UserName = "user3@library.com",
                Email = "user3@library.com",
                FirstName = "Ece",
                LastName = "Araştırmacı",
                EmailConfirmed = true,
                IsApproved = true
            };

            if (await userManager.FindByEmailAsync(user1.Email) == null)
            {
                await userManager.CreateAsync(user1, "User123!");
                await userManager.AddToRoleAsync(user1, Roles.User);
            }
            if (await userManager.FindByEmailAsync(user2.Email) == null)
            {
                await userManager.CreateAsync(user2, "User123!");
                await userManager.AddToRoleAsync(user2, Roles.User);
            }
            if (await userManager.FindByEmailAsync(user3.Email) == null)
            {
                await userManager.CreateAsync(user3, "User123!");
                await userManager.AddToRoleAsync(user3, Roles.User);
            }

            // 5️⃣ Kitapları Ekle
            if (!dbContext.Books.Any())
            {
                var books = new List<Book>
                {
                    new Book { Title = "Savaş ve Barış", Author = "Lev Tolstoy", PublicationYear = 1869, Publisher = "Rus Klasikleri", Status = BookStatus.Available },
                    new Book { Title = "Suç ve Ceza", Author = "Fyodor Dostoyevski", PublicationYear = 1866, Publisher = "Rus Klasikleri", Status = BookStatus.Available },
                    new Book { Title = "Kürk Mantolu Madonna", Author = "Sabahattin Ali", PublicationYear = 1943, Publisher = "Yapı Kredi Yayınları", Status = BookStatus.Available },
                    new Book { Title = "1984", Author = "George Orwell", PublicationYear = 1949, Publisher = "Can Yayınları", Status = BookStatus.Available },
                    new Book { Title = "Dune", Author = "Frank Herbert", PublicationYear = 1965, Publisher = "İthaki Yayınları", Status = BookStatus.Available }
                };
                await dbContext.Books.AddRangeAsync(books);
                await dbContext.SaveChangesAsync();
            }

            // 6️⃣ Kiralama İşlemleri (User1 & User2 Kitap Kiralıyor)
            if (!dbContext.Rentals.Any())
            {
                var rental1 = new Rental
                {
                    BookId = dbContext.Books.First().Id,
                    UserId = user1.Id,
                    RentalDate = DateTime.UtcNow.AddDays(-10),
                    DueDate = DateTime.UtcNow.AddDays(4),
                    ReturnDate = null
                };

                var rental2 = new Rental
                {
                    BookId = dbContext.Books.Skip(1).First().Id,
                    UserId = user2.Id,
                    RentalDate = DateTime.UtcNow.AddDays(-15),
                    DueDate = DateTime.UtcNow.AddDays(-1),
                    ReturnDate = DateTime.UtcNow.AddDays(-3) // Zamanında iade edilmiş
                };

                await dbContext.Rentals.AddRangeAsync(rental1, rental2);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
