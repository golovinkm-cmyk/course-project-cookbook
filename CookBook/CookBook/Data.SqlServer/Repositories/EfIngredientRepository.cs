using Data.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.SqlServer.Repositories;

public class EfIngredientRepository : IIngredientRepository
{
    private readonly CookBookDbContext _context;

    public EfIngredientRepository(CookBookDbContext context)
    {
        _context = context;
    }

    public Ingredient? GetById(int id)
    {
        return _context.Ingredients.Find(id);
    }

    public IEnumerable<Ingredient> GetAll()
    {
        return _context.Ingredients.AsNoTracking().ToList();
    }

    public int Add(Ingredient ingredient)
    {
        _context.Ingredients.Add(ingredient);
        _context.SaveChanges();
        return ingredient.Id;
    }

    public bool Update(Ingredient ingredient)
    {
        var existing = GetById(ingredient.Id);
        if (existing == null) return false;

        existing.Name = ingredient.Name;
        existing.Unit = ingredient.Unit;
        existing.Category = ingredient.Category;

        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var ingredient = GetById(id);
        if (ingredient == null) return false;

        _context.Ingredients.Remove(ingredient);
        _context.SaveChanges();
        return true;
    }

    public Ingredient? GetByName(string name)
    {
        return _context.Ingredients
            .AsNoTracking()
            .FirstOrDefault(i => i.Name == name);
    }

    public IEnumerable<Ingredient> GetByCategory(string category)
    {
        return _context.Ingredients
            .Where(i => i.Category == category)
            .AsNoTracking()
            .ToList();
    }
}