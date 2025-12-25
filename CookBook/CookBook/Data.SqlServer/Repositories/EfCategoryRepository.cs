
using Domain.Entities;
using Data.SqlServer;
using Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Data.SqlServer.Repositories;

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
        return _context.Categories.ToList();
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

        _context.Entry(existing).CurrentValues.SetValues(category);
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
            .ToList();
    }

    public Category? GetByName(string name)
    {
        return _context.Categories
            .FirstOrDefault(c => c.Name.ToLower() == name.ToLower());
    }
}