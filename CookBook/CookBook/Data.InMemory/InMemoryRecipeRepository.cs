
using System.Xml.Linq;
using Data.Interfaces;
using Data.Interfaces.Filters;
using Domain.Entities;


namespace Data.InMemory;

public class InMemoryRecipeRepository : IRecipeRepository
{
    private readonly List<Recipe> _recipes = new();
    private int _nextId = 1;

    public Recipe? GetById(int id)
    {
        return _recipes.FirstOrDefault(r => r.Id == id);
    }

    public IEnumerable<Recipe> GetAll()
    {
        return _recipes;
    }

    public int Add(Recipe recipe)
    {
        recipe.Id = _nextId++;
        recipe.CreatedDate = DateTime.Now;
        recipe.ModifiedDate = DateTime.Now;
        _recipes.Add(recipe);
        return recipe.Id;
    }

    public bool Update(Recipe recipe)
    {
        var existing = GetById(recipe.Id);
        if (existing == null) return false;

        var index = _recipes.IndexOf(existing);
        recipe.ModifiedDate = DateTime.Now;
        _recipes[index] = recipe;
        return true;
    }

    public bool Delete(int id)
    {
        var recipe = GetById(id);
        if (recipe == null) return false;

        return _recipes.Remove(recipe);
    }

    public IEnumerable<Recipe> GetByCategory(int categoryId)
    {
        return _recipes.Where(r => r.CategoryId == categoryId);
    }

    public IEnumerable<Recipe> GetFavorites()
    {
        return _recipes.Where(r => r.IsFavorite);
    }

    public IEnumerable<Recipe> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return _recipes;

        return _recipes.Where(r =>
            r.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            r.Description?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true ||
            r.Instructions.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Recipe> GetPremiumRecipes()
    {
        return _recipes.Where(r => r.IsPremium);
    }

    public IEnumerable<Recipe> GetRecipesWithIngredients()
    {
        return _recipes; // В реальной реализации загружались бы ингредиенты
    }

    // Дополнительный метод для фильтрации
    public IEnumerable<Recipe> GetFiltered(RecipeFilter filter)
    {
        var query = _recipes.AsEnumerable();

        if (filter.CategoryId.HasValue)
            query = query.Where(r => r.CategoryId == filter.CategoryId.Value);

        if (!string.IsNullOrEmpty(filter.DifficultyLevel))
            query = query.Where(r => r.DifficultyLevel == filter.DifficultyLevel);

        if (filter.MaxCookingTime.HasValue)
            query = query.Where(r => r.TotalTime <= filter.MaxCookingTime.Value);

        if (filter.IsFavorite.HasValue)
            query = query.Where(r => r.IsFavorite == filter.IsFavorite.Value);

        if (filter.IsPremium.HasValue)
            query = query.Where(r => r.IsPremium == filter.IsPremium.Value);

        if (!string.IsNullOrEmpty(filter.SearchKeyword))
            query = query.Where(r => r.Title.Contains(filter.SearchKeyword));

        if (filter.StartDate.HasValue)
            query = query.Where(r => r.CreatedDate >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(r => r.CreatedDate <= filter.EndDate.Value);

        return query.ToList();
    }
}