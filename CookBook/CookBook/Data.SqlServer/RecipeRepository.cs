using Data.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Data.SqlServer
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly CookbookDbContext _context;

        // Конструктор принимает DbContext
        public RecipeRepository(CookbookDbContext context)
        {
            _context = context;
        }

        public int Add(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            _context.SaveChanges();
            return recipe.Id;
        }

        public Recipe? GetById(int id)
        {
            return _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefault(r => r.Id == id);
        }

        public List<Recipe> GetAll()
        {
            return _context.Recipes
                .Include(r => r.Category)
                .ToList();
        }

        public bool Update(Recipe recipe)
        {
            var existing = _context.Recipes.Find(recipe.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(recipe);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var recipe = _context.Recipes.Find(id);
            if (recipe == null) return false;

            _context.Recipes.Remove(recipe);
            _context.SaveChanges();
            return true;
        }

        public List<Recipe> GetByCategory(int categoryId)
        {
            return _context.Recipes
                .Where(r => r.CategoryId == categoryId)
                .ToList();
        }

        public List<Recipe> GetFavoriteRecipes()
        {
            return _context.Recipes
                .Where(r => r.IsFavorite)
                .ToList();
        }

        public List<Recipe> SearchRecipes(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            return _context.Recipes
                .Where(r => r.Title.Contains(searchTerm) ||
                           (r.Description != null && r.Description.Contains(searchTerm)) ||
                           r.Instructions.Contains(searchTerm))
                .ToList();
        }
    }
}