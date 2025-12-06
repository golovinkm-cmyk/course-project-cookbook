
using Data.Interfaces;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Data.InMemory
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly List<Recipe> _recipes = new();
        private int _nextId = 1;

        public RecipeRepository()
        {
            // Заполнение тестовыми данными
            SeedData();
        }

        private void SeedData()
        {
            // Тестовые рецепты
            Add(new Recipe
            {
                Title = "Картофель по-деревенски",
                Description = "Вкусный запеченный картофель с травами",
                Instructions = "1. Картофель помыть и нарезать дольками\n2. Смешать с маслом и специями\n3. Запекать 30-40 минут при 200°C",
                PreparationTime = 15,
                CookingTime = 40,
                Servings = 4,
                DifficultyLevel = "Легкий",
                CategoryId = 1,
                IsFavorite = true,
                IsPremium = false,
                ImagePath = null
            });

            Add(new Recipe
            {
                Title = "Салат Цезарь",
                Description = "Классический салат с курицей и сухариками",
                Instructions = "1. Куриное филе обжарить\n2. Салат романо порвать руками\n3. Приготовить соус\n4. Смешать все ингредиенты",
                PreparationTime = 20,
                CookingTime = 15,
                Servings = 2,
                DifficultyLevel = "Средний",
                CategoryId = 2,
                IsFavorite = false,
                IsPremium = true,
                ImagePath = null
            });

            Add(new Recipe
            {
                Title = "Шоколадный торт",
                Description = "Нежный шоколадный торт с кремом",
                Instructions = "1. Приготовить тесто\n2. Выпекать 30 минут\n3. Приготовить крем\n4. Собрать торт",
                PreparationTime = 40,
                CookingTime = 60,
                Servings = 8,
                DifficultyLevel = "Сложный",
                CategoryId = 3,
                IsFavorite = true,
                IsPremium = true,
                ImagePath = null
            });
        }

        public int Add(Recipe recipe)
        {
            recipe.Id = _nextId++;
            recipe.CreatedDate = DateTime.Now;
            recipe.ModifiedDate = DateTime.Now;
            _recipes.Add(recipe);
            return recipe.Id;
        }

        public Recipe? GetById(int id)
        {
            return _recipes.FirstOrDefault(r => r.Id == id);
        }

        public List<Recipe> GetAll()
        {
            return _recipes.ToList();
        }

        public bool Update(Recipe recipe)
        {
            var existing = GetById(recipe.Id);
            if (existing == null) return false;

            // Копируем свойства
            existing.Title = recipe.Title;
            existing.Description = recipe.Description;
            existing.Instructions = recipe.Instructions;
            existing.PreparationTime = recipe.PreparationTime;
            existing.CookingTime = recipe.CookingTime;
            existing.Servings = recipe.Servings;
            existing.DifficultyLevel = recipe.DifficultyLevel;
            existing.CategoryId = recipe.CategoryId;
            existing.ModifiedDate = DateTime.Now;
            existing.IsFavorite = recipe.IsFavorite;
            existing.IsPremium = recipe.IsPremium;
            existing.ImagePath = recipe.ImagePath;

            return true;
        }

        public bool Delete(int id)
        {
            var recipe = GetById(id);
            if (recipe == null) return false;

            return _recipes.Remove(recipe);
        }

        public List<Recipe> GetByCategory(int categoryId)
        {
            return _recipes.Where(r => r.CategoryId == categoryId).ToList();
        }

        public List<Recipe> GetFavoriteRecipes()
        {
            return _recipes.Where(r => r.IsFavorite).ToList();
        }

        public List<Recipe> SearchRecipes(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _recipes.ToList();

            return _recipes.Where(r =>
                r.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.Description != null && r.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                r.Instructions.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }
    }
}
