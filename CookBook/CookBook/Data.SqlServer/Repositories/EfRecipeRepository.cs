using Data.Interfaces;
using Data.Interfaces.Filters;
using Domain.Entities;
using Data.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace CookBook.Data.SqlServer.Repositories;

public class EfRecipeRepository : IRecipeRepository
{
    private readonly CookBookDbContext _context;

    public EfRecipeRepository(CookBookDbContext context)
    {
        _context = context;
    }

    public Recipe? GetById(int id)
    {
        return _context.Recipes
            .Include(r => r.Category)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefault(r => r.Id == id);
    }

    public IEnumerable<Recipe> GetAll()
    {
        return _context.Recipes
            .Include(r => r.Category)
            .ToList();
    }

    public int Add(Recipe recipe)
    {
        recipe.CreatedDate = DateTime.Now;
        recipe.ModifiedDate = DateTime.Now;
        _context.Recipes.Add(recipe);
        _context.SaveChanges();
        return recipe.Id;
    }

    public bool Update(Recipe recipe)
    {
        var existing = GetById(recipe.Id);
        if (existing == null) return false;

        recipe.ModifiedDate = DateTime.Now;
        _context.Entry(existing).CurrentValues.SetValues(recipe);
        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var recipe = GetById(id);
        if (recipe == null) return false;

        _context.Recipes.Remove(recipe);
        _context.SaveChanges();
        return true;
    }

    public IEnumerable<Recipe> GetByCategory(int categoryId)
    {
        return _context.Recipes
            .Where(r => r.CategoryId == categoryId)
            .Include(r => r.Category)
            .ToList();
    }

    public IEnumerable<Recipe> GetFavorites()
    {
        return _context.Recipes
            .Where(r => r.IsFavorite)
            .Include(r => r.Category)
            .ToList();
    }

    public IEnumerable<Recipe> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return GetAll();

        return _context.Recipes
            .Where(r => r.Title.Contains(keyword) ||
                       (r.Description != null && r.Description.Contains(keyword)) ||
                       r.Instructions.Contains(keyword))
            .Include(r => r.Category)
            .ToList();
    }

    public IEnumerable<Recipe> GetPremiumRecipes()
    {
        return _context.Recipes
            .Where(r => r.IsPremium)
            .Include(r => r.Category)
            .ToList();
    }

    public IEnumerable<Recipe> GetRecipesWithIngredients()
    {
        return _context.Recipes
            .Include(r => r.Category)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .ToList();
    }

    public IEnumerable<Recipe> GetFiltered(RecipeFilter filter)
    {
        var query = _context.Recipes
            .Include(r => r.Category)
            .AsQueryable();

        if (filter.CategoryId.HasValue)
            query = query.Where(r => r.CategoryId == filter.CategoryId.Value);

        if (!string.IsNullOrEmpty(filter.DifficultyLevel))
            query = query.Where(r => r.DifficultyLevel == filter.DifficultyLevel);

        if (filter.MaxCookingTime.HasValue)
            query = query.Where(r => (r.PreparationTime + r.CookingTime) <= filter.MaxCookingTime.Value);

        if (filter.IsFavorite.HasValue)
            query = query.Where(r => r.IsFavorite == filter.IsFavorite.Value);

        if (filter.IsPremium.HasValue)
            query = query.Where(r => r.IsPremium == filter.IsPremium.Value);

        if (!string.IsNullOrEmpty(filter.SearchKeyword))
            query = query.Where(r => r.Title.Contains(filter.SearchKeyword) ||
                                   (r.Description != null && r.Description.Contains(filter.SearchKeyword)));

        if (filter.StartDate.HasValue)
            query = query.Where(r => r.CreatedDate >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(r => r.CreatedDate <= filter.EndDate.Value);

        return query.ToList();
    }
}
