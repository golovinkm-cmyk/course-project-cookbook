using Data.Interfaces;
using Domain;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Data.InMemory
{
    public class RecipeIngredientRepository : IRecipeIngredientRepository
    {
        private readonly List<RecipeIngredient> _recipeIngredients = new();
        private int _nextId = 1;

        public RecipeIngredientRepository()
        {
            // Тестовые данные
            SeedData();
        }

        private void SeedData()
        {
            // Ингредиенты для первого рецепта (Картофель по-деревенски)
            Add(new RecipeIngredient(1, 1, 0.5m, "Молодой картофель")); // Картофель
            Add(new RecipeIngredient(1, 16, 2m)); // Масло растительное
            Add(new RecipeIngredient(1, 13, 1m)); // Соль
            Add(new RecipeIngredient(1, 14, 0.5m)); // Перец

            // Ингредиенты для второго рецепта (Салат Цезарь)
            Add(new RecipeIngredient(2, 6, 300m)); // Куриное филе
            Add(new RecipeIngredient(2, 11, 100m)); // Сыр
            Add(new RecipeIngredient(2, 13, 1m)); // Соль
        }

        public int Add(RecipeIngredient recipeIngredient)
        {
            recipeIngredient.Id = _nextId++;
            _recipeIngredients.Add(recipeIngredient);
            return recipeIngredient.Id;
        }

        public RecipeIngredient? GetById(int id)
        {
            return _recipeIngredients.FirstOrDefault(ri => ri.Id == id);
        }

        public List<RecipeIngredient> GetAll()
        {
            return _recipeIngredients.ToList();
        }

        public bool Update(RecipeIngredient recipeIngredient)
        {
            var existing = GetById(recipeIngredient.Id);
            if (existing == null) return false;

            existing.RecipeId = recipeIngredient.RecipeId;
            existing.IngredientId = recipeIngredient.IngredientId;
            existing.Quantity = recipeIngredient.Quantity;
            existing.Notes = recipeIngredient.Notes;

            return true;
        }

        public bool Delete(int id)
        {
            var recipeIngredient = GetById(id);
            if (recipeIngredient == null) return false;

            return _recipeIngredients.Remove(recipeIngredient);
        }

        public List<RecipeIngredient> GetByRecipeId(int recipeId)
        {
            return _recipeIngredients.Where(ri => ri.RecipeId == recipeId).ToList();
        }

        public bool DeleteByRecipeId(int recipeId)
        {
            var itemsToRemove = _recipeIngredients.Where(ri => ri.RecipeId == recipeId).ToList();
            foreach (var item in itemsToRemove)
            {
                _recipeIngredients.Remove(item);
            }
            return itemsToRemove.Count > 0;
        }
    }
}