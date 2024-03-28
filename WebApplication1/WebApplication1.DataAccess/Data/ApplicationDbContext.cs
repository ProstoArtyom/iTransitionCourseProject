using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Collection> Collections { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemTag>()
                .HasKey(it => new { it.ItemId, it.TagId });

            modelBuilder.Entity<ItemTag>()
                .HasOne(u => u.Item)
                .WithMany(u => u.ItemTags)
                .HasForeignKey(u => u.ItemId);

            modelBuilder.Entity<ItemTag>()
                .HasOne(u => u.Tag)
                .WithMany(u => u.ItemTags)
                .HasForeignKey(u => u.TagId);

            modelBuilder.Entity<Theme>().HasData(
                new Theme { Id = 1, Name = "Books" },
                new Theme { Id = 2, Name = "Signs" },
                new Theme { Id = 3, Name = "Silverware" }
            );

            modelBuilder.Entity<Collection>().HasData(
                new Collection
                {
                    Id = 1,
                    Name = "Collection №1",
                    Description = "It's a description for collection №1",
                    ThemeId = 2
                }
            );
            
            modelBuilder.Entity<Tag>().HasData(
                new Tag
                {
                    Id = 1,
                    Name = "Cool_Collection"
                }
            );

            var customFields = new
            {
                Book = "Librarian of Basra",
                Author = "Jeanette Winter",
                Published = new DateOnly(1939, 12, 04)
            };
            var customFieldsJson = JsonConvert.SerializeObject(customFields);

            modelBuilder.Entity<Item>().HasData(
                new Item
                {
                    Id = 1, 
                    Name = "Book 1",
                    CustomFields = customFieldsJson,
                    CollectionId = 1
                }
            );
        }
    }
}
