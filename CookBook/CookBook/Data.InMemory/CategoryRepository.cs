
using Domain;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Data.InMemory
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categories = new();
        private int _nextId = 1;

        public CategoryRepository()
        {
            // Заполнение тестовыми данными
            SeedData();
        }

        private void SeedData()
        {
            Add(new Category("Основные блюда", "Горячие блюда на обед или ужин"));
            Add(new Category("Закуски", "Холодные и горячие закуски"));
            Add(new Category("Десерты", "Сладкие блюда и выпечка"));
            Add(new Category("Завтраки", "Блюда для утреннего приема пищи"));
            Add(new Category("Напитки", "Горячие и холодные напитки"));
        }

        public int Add(Category category)
        {
            category.Id = _nextId++;
            category.CreatedDate = DateTime.Now;
            _categories.Add(category);
            return category.Id;
        }

        public Category? GetById(int id)
        {
            return _categories.FirstOrDefault(c => c.Id == id);
        }

        public List<Category> GetAll()
        {
            return _categories.ToList();
        }

        public bool Update(Category category)
        {
            var existing = GetById(category.Id);
            if (existing == null) return false;

            existing.Name = category.Name;
            existing.Description = category.Description;

            return true;
        }

        public bool Delete(int id)
        {
            var category = GetById(id);
            if (category == null) return false;

            return _categories.Remove(category);
        }

        public bool HasRecipes(int categoryId)
        {
            // Здесь нужно будет проверить, есть ли рецепты в этой категории
            // Для этого нужен доступ к RecipeRepository
            // Пока возвращаем false
            return false;
        }

        public Category? GetByName(string name)
        {
            return _categories.FirstOrDefault(c =>
                c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
