using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Responsive.Models;

namespace Responsive.Data
{
    public class BookDBContext : IdentityDbContext<Account>
    {
        public BookDBContext() { }
        public BookDBContext(DbContextOptions<BookDBContext> options) : base(options) { }
        public DbSet<Responsive.Models.Book> Books { get; set; }
        public DbSet<Responsive.Models.Order> Orders { get; set; }
        public DbSet<Responsive.Models.CartItem> CartItems { get; set; }
        public DbSet<Responsive.Models.Review> Reviews { get; set; }
        public DbSet<Responsive.Models.BookCategory> BookCategories { get; set; }
        public DbSet<Responsive.Models.News> Newsdb { get; set; }
        public DbSet<Responsive.Models.Event> Events { get; set; }

        public DbSet<Responsive.Models.Payment> Payments { get; set; }
        public DbSet<Responsive.Models.UserAddress> Addressesdb { get; set; }
        public DbSet<Responsive.Models.OrderCartItem> OrderCartItems { get; set; }  
        public DbSet<Responsive.Models.BookType> BookTypes { get; set; }
        internal Task SaveChangesasync()
        {
            throw new NotImplementedException();
        }
    }
}
