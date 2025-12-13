using Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Data.SqlServer
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CookbookDbContext _context;

        public CategoryRepository(CookbookDbContext context)
        {
            _context = context;
        }

        public int Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category.Id;
        }

        public Category? GetById(int id)
        {
            return _context.Categories.Find(id);
        }

        public List<Category> GetAll()
        {
            return _context.Categories.ToList();
        }

        public bool Update(Category category)
        {
            var existing = _context.Categories.Find(category.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(category);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return true;
        }

        public bool HasRecipes(int categoryId)
        {
            return _context.Recipes.Any(r => r.CategoryId == categoryId);
        }

        public Category? GetByName(string name)
        {
            return _context.Categories
                .FirstOrDefault(c => c.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}