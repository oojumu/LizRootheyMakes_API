using LizRootheyMakes_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LizRootheyMakes_API.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<MenuItem> MenuItems { get; set; }
		public DbSet<CartItem> CartItems { get; set; }
		public DbSet<ShoppingCart> ShoppingCarts { get; set; }
		public DbSet<OrderHeader> OrderHeaders { get; set; }
		public DbSet<OrderDetails> OrderDetails { get; set; }



		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<MenuItem>().HasData(new MenuItem
			{
				Id = 1,
				Name = "Black hat",
				Description = "Black hat with Feathers",
				Image = "https://dotnetmasteryimages.images.blob.core.windows.net/redmango/spring roll.jpg",
				Price = 7.99,
				Category = "Hat",
				SpecialTag = ""
			}, new MenuItem
			{
				Id = 2,
				Name = "Pink hat",
				Description = "Pink hat with Feathers",
				Image = "https://dotnetmasteryimages.images.blob.core.windows.net/redmango/idli.jpg",
				Price = 8.99,
				Category = "Hat",
				SpecialTag = ""
			}, new MenuItem
			{
				Id = 3,
				Name = "Purple hat",
				Description = "Purple hat with Feathers",
				Image = "https://dotnetmasteryimages.images.blob.core.windows.net/redmango/idli.jpg",
				Price = 8.99,
				Category = "Hat",
				SpecialTag = ""
		    });



		}
	}
}
