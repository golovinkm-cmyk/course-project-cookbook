using Data.Interfaces;
using Data.SqlServer;
using Domain.Entities;

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
        return _context.Ingredients.ToList();
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

        _context.Entry(existing).CurrentValues.SetValues(ingredient);
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
        // Используем String.Equals с игнорированием регистра
        return _context.Ingredients
            .FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Ingredient> GetByCategory(string category)
    {
        // Используем безопасный подход с String.Equals
        return _context.Ingredients
            .Where(i => i.Category != null &&
                       string.Equals((string?)i.Category, category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}