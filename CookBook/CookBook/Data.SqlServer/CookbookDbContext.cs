using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Data.SqlServer;

public class CookBookDbContext : DbContext
{
    public CookBookDbContext(DbContextOptions<CookBookDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Recipe> Recipes { get; set; } = null!;
    public DbSet<Ingredient> Ingredients { get; set; } = null!;
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = null!;
    public DbSet<License> Licenses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Отключаем каскадное удаление по умолчанию
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Настройка Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            entity.Property(e => e.ModifiedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            entity.HasMany(e => e.Recipes)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Индекс для ускорения поиска по имени
            entity.HasIndex(e => e.Name)
                .IsUnique();
        });

        // Настройка Recipe
        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Instructions)
                .IsRequired();

            entity.Property(e => e.DifficultyLevel)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.PreparationTime)
                .IsRequired();

            entity.Property(e => e.CookingTime)
                .IsRequired();

            entity.Property(e => e.Servings)
                .IsRequired();

            entity.Property(e => e.IsFavorite)                                  //Конфигурация БД
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.IsPremium)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            entity.Property(e => e.ModifiedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Вычисляемое свойство TotalTime (только в модели EF Core, не в БД)
            entity.Ignore(e => e.TotalTime);

            entity.HasOne(e => e.Category)
                .WithMany(e => e.Recipes)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.RecipeIngredients)
                .WithOne(e => e.Recipe)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы для ускорения поиска
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.IsFavorite);
            entity.HasIndex(e => e.IsPremium);
            entity.HasIndex(e => e.CreatedDate);
            entity.HasIndex(e => e.Title);
        });

        // Настройка Ingredient
        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Unit)
                .HasMaxLength(20);

            entity.Property(e => e.Category)
                .HasMaxLength(100);

            entity.HasMany(e => e.RecipeIngredients)
                .WithOne(e => e.Ingredient)
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Индекс для ускорения поиска по имени
            entity.HasIndex(e => e.Name);
        });

        // Настройка RecipeIngredient (связь многие-ко-многим)
        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Quantity)
                .IsRequired()
                .HasPrecision(10, 2);

            entity.Property(e => e.Unit)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Notes)
                .HasMaxLength(500);

            entity.HasOne(e => e.Recipe)
                .WithMany(e => e.RecipeIngredients)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Ingredient)
                .WithMany(e => e.RecipeIngredients)
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

                                                                //Запрет дублирования ингредиента в одном рецепте.
            entity.HasIndex(e => new { e.RecipeId, e.IngredientId })
                .IsUnique();
        });

        // Настройка License
        modelBuilder.Entity<License>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.LicenseKey)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LicenseType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CustomerEmail)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50);

            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50);

            entity.Property(e => e.TransactionId)
                .HasMaxLength(100);

            entity.Property(e => e.CardLastFour)
                .HasMaxLength(4);

            entity.Property(e => e.Amount)
                .HasPrecision(18, 2);

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("GETDATE()");

            entity.Property(e => e.ActivationDate);

            entity.Property(e => e.ExpiryDate);

            // Индексы для ускорения поиска
            entity.HasIndex(e => e.LicenseKey)
                .IsUnique();

            entity.HasIndex(e => e.IsActive);

            entity.HasIndex(e => e.ExpiryDate);

            entity.HasIndex(e => e.CustomerEmail);
        });
    }

    public override int SaveChanges()               //Автоматическое обновление дат
    {
        // Автоматическое обновление ModifiedDate для измененных сущностей
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

        foreach (var entityEntry in entries)
        {
            if (entityEntry.Entity is Category category)
            {
                if (entityEntry.State == EntityState.Modified)
                {
                    category.ModifiedDate = DateTime.Now;
                }
                else if (entityEntry.State == EntityState.Added)
                {
                    category.CreatedDate = DateTime.Now;
                    category.ModifiedDate = DateTime.Now;
                }
            }
            else if (entityEntry.Entity is Recipe recipe)
            {
                if (entityEntry.State == EntityState.Modified)
                {
                    recipe.ModifiedDate = DateTime.Now;
                }
                else if (entityEntry.State == EntityState.Added)
                {
                    recipe.CreatedDate = DateTime.Now;
                    recipe.ModifiedDate = DateTime.Now;
                }
            }
        }

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Автоматическое обновление ModifiedDate для измененных сущностей (асинхронная версия)
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

        foreach (var entityEntry in entries)
        {
            if (entityEntry.Entity is Category category)
            {
                if (entityEntry.State == EntityState.Modified)
                {
                    category.ModifiedDate = DateTime.Now;
                }
                else if (entityEntry.State == EntityState.Added)
                {
                    category.CreatedDate = DateTime.Now;
                    category.ModifiedDate = DateTime.Now;
                }
            }
            else if (entityEntry.Entity is Recipe recipe)
            {
                if (entityEntry.State == EntityState.Modified)
                {
                    recipe.ModifiedDate = DateTime.Now;
                }
                else if (entityEntry.State == EntityState.Added)
                {
                    recipe.CreatedDate = DateTime.Now;
                    recipe.ModifiedDate = DateTime.Now;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}