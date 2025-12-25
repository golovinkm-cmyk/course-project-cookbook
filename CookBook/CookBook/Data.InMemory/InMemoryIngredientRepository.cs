using System.Xml.Linq;
using Data.Interfaces;
using Domain.Entities;



namespace Data.InMemory;

public class InMemoryIngredientRepository : IIngredientRepository
{
    private readonly List<Ingredient> _ingredients = new();
    private int _nextId = 1;

    public Ingredient? GetById(int id)
    {
        return _ingredients.FirstOrDefault(i => i.Id == id);
    }

    public IEnumerable<Ingredient> GetAll()
    {
        return _ingredients;
    }

    public int Add(Ingredient ingredient)
    {
        ingredient.Id = _nextId++;
        _ingredients.Add(ingredient);
        return ingredient.Id;
    }

    public bool Update(Ingredient ingredient)
    {
        var existing = GetById(ingredient.Id);
        if (existing == null) return false;

        var index = _ingredients.IndexOf(existing);
        _ingredients[index] = ingredient;
        return true;
    }

    public bool Delete(int id)
    {
        var ingredient = GetById(id);
        if (ingredient == null) return false;

        return _ingredients.Remove(ingredient);
    }

    public Ingredient? GetByName(string name)
    {
        return _ingredients.FirstOrDefault(i =>
            i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Ingredient> GetByCategory(string category)
    {
        return _ingredients.Where(i =>
            i.Category != null &&
            string.Equals((string?)i.Category, category, StringComparison.OrdinalIgnoreCase));
    }
}