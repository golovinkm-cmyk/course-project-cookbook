using Domain.Entities;
using Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.SqlServer.Repositories;

public class EfCategoryRepository : ICategoryRepository
{
    private readonly CookBookDbContext _context;

    public EfCategoryRepository(CookBookDbContext context)
    {
        _context = context;
    }

    public Category? GetById(int id)
    {
        return _context.Categories.Find(id);
    }

    public IEnumerable<Category> GetAll()
    {
        return _context.Categories.AsNoTracking().ToList();
    }

    public int Add(Category category)
    {
        category.CreatedDate = DateTime.Now;
        _context.Categories.Add(category);
        _context.SaveChanges();
        return category.Id;
    }

    public bool Update(Category category)
    {
        var existing = GetById(category.Id);
        if (existing == null) return false;

        // Копируем свойства, кроме Id и CreatedDate
        existing.Name = category.Name;
        existing.Description = category.Description;
        existing.ModifiedDate = DateTime.Now;

        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var category = GetById(id);
        if (category == null) return false;

        _context.Categories.Remove(category);
        _context.SaveChanges();
        return true;
    }

    public IEnumerable<Category> GetCategoriesWithRecipes()
    {
        return _context.Categories
            .Include(c => c.Recipes)
            .AsNoTracking()
            .ToList();
    }

    public Category? GetByName(string name)
    {
        return _context.Categories
            .AsNoTracking()
            .FirstOrDefault(c => c.Name == name);
    }
}