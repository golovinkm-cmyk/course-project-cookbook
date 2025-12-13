using Data.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Data.SqlServer
{
    public class RecipeIngredientRepository : IRecipeIngredientRepository
    {
        private readonly CookbookDbContext _context;

        public RecipeIngredientRepository(CookbookDbContext context)
        {
            _context = context;
        }

        public int Add(RecipeIngredient recipeIngredient)
        {
            _context.RecipeIngredients.Add(recipeIngredient);
            _context.SaveChanges();
            return recipeIngredient.Id;
        }

        public RecipeIngredient? GetById(int id)
        {
            return _context.RecipeIngredients.Find(id);
        }

        public List<RecipeIngredient> GetAll()
        {
            return _context.RecipeIngredients
                .Include(ri => ri.Recipe)
                .Include(ri => ri.Ingredient)
                .ToList();
        }

        public bool Update(RecipeIngredient recipeIngredient)
        {
            var existing = _context.RecipeIngredients.Find(recipeIngredient.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(recipeIngredient);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var recipeIngredient = _context.RecipeIngredients.Find(id);
            if (recipeIngredient == null) return false;

            _context.RecipeIngredients.Remove(recipeIngredient);
            _context.SaveChanges();
            return true;
        }

        public List<RecipeIngredient> GetByRecipeId(int recipeId)
        {
            return _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .Where(ri => ri.RecipeId == recipeId)
                .ToList();
        }

        public bool DeleteByRecipeId(int recipeId)
        {
            var itemsToRemove = _context.RecipeIngredients
                .Where(ri => ri.RecipeId == recipeId)
                .ToList();

            if (itemsToRemove.Any())
            {
                _context.RecipeIngredients.RemoveRange(itemsToRemove);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
