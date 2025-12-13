using Data.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Data.SqlServer
{
    public class CookbookDbContext : DbContext
    {
        public CookbookDbContext(DbContextOptions<CookbookDbContext> options) : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<System.ComponentModel.License> Licenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка отношений
            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.RecipeIngredients)
                .WithOne(ri => ri.Recipe)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<Ingredient>()
                .HasMany(i => i.RecipeIngredients)
                .WithOne(ri => ri.Ingredient)
                .HasForeignKey(ri => ri.IngredientId);
        }
    }
}
