using Data.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Data.SqlServer
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly CookbookDbContext _context;

        public IngredientRepository(CookbookDbContext context)
        {
            _context = context;
        }

        public int Add(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            _context.SaveChanges();
            return ingredient.Id;
        }

        public Ingredient? GetById(int id)
        {
            return _context.Ingredients.Find(id);
        }

        public List<Ingredient> GetAll()
        {
            return _context.Ingredients.ToList();
        }

        public bool Update(Ingredient ingredient)
        {
            var existing = _context.Ingredients.Find(ingredient.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(ingredient);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var ingredient = _context.Ingredients.Find(id);
            if (ingredient == null) return false;

            _context.Ingredients.Remove(ingredient);
            _context.SaveChanges();
            return true;
        }

        public List<Ingredient> SearchIngredients(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            return _context.Ingredients
                .Where(i => i.Name.Contains(searchTerm) ||
                           (i.Category != null && i.Category.Contains(searchTerm)))
                .ToList();
        }

        public Ingredient? GetByName(string name)
        {
            return _context.Ingredients
                .FirstOrDefault(i => i.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
