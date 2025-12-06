using Data.Interfaces;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Data.InMemory
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly List<Ingredient> _ingredients = new();
        private int _nextId = 1;

        public IngredientRepository()
        {
            // Заполнение тестовыми данными
            SeedData();
        }

        private void SeedData()
        {
            // Основные ингредиенты
            Add(new Ingredient("Картофель", "кг", "Овощи"));
            Add(new Ingredient("Лук репчатый", "шт", "Овощи"));
            Add(new Ingredient("Морковь", "шт", "Овощи"));
            Add(new Ingredient("Помидор", "шт", "Овощи"));
            Add(new Ingredient("Огурцы", "шт", "Овощи"));

            Add(new Ingredient("Куриное филе", "г", "Мясо и птица"));
            Add(new Ingredient("Говядина", "г", "Мясо и птица"));
            Add(new Ingredient("Свинина", "г", "Мясо и птица"));

            Add(new Ingredient("Молоко", "мл", "Молочные продукты"));
            Add(new Ingredient("Сметана", "г", "Молочные продукты"));
            Add(new Ingredient("Сыр", "г", "Молочные продукты"));

            Add(new Ingredient("Мука пшеничная", "г", "Бакалея"));
            Add(new Ingredient("Сахар", "г", "Бакалея"));
            Add(new Ingredient("Соль", "ч.л.", "Бакалея"));
            Add(new Ingredient("Перец черный", "ч.л.", "Бакалея"));
            Add(new Ingredient("Масло растительное", "ст.л.", "Бакалея"));
            Add(new Ingredient("Масло сливочное", "г", "Бакалея"));
        }

        public int Add(Ingredient ingredient)
        {
            ingredient.Id = _nextId++;
            _ingredients.Add(ingredient);
            return ingredient.Id;
        }

        public Ingredient? GetById(int id)
        {
            return _ingredients.FirstOrDefault(i => i.Id == id);
        }

        public List<Ingredient> GetAll()
        {
            return _ingredients.ToList();
        }

        public bool Update(Ingredient ingredient)
        {
            var existing = GetById(ingredient.Id);
            if (existing == null) return false;

            existing.Name = ingredient.Name;
            existing.Unit = ingredient.Unit;
            existing.Category = ingredient.Category;
            existing.Calories = ingredient.Calories;
            existing.Protein = ingredient.Protein;
            existing.Fat = ingredient.Fat;
            existing.Carbs = ingredient.Carbs;

            return true;
        }

        public bool Delete(int id)
        {
            var ingredient = GetById(id);
            if (ingredient == null) return false;

            return _ingredients.Remove(ingredient);
        }

        public List<Ingredient> SearchIngredients(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _ingredients.ToList();

            return _ingredients.Where(i =>
                i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (i.Category != null && i.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        public Ingredient? GetByName(string name)
        {
            return _ingredients.FirstOrDefault(i =>
                i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
