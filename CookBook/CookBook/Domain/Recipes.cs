
using System;
using System.Collections.Generic;

namespace Domain
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public int PreparationTime { get; set; }
        public int CookingTime { get; set; }
        public int Servings { get; set; }
        public string DifficultyLevel { get; set; } = "Средний";
        public int CategoryId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public bool IsFavorite { get; set; }
        public bool IsPremium { get; set; }
        public string? ImagePath { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<RecipeIngredient>? RecipeIngredients { get; set; }

        public Recipe()
        {
            RecipeIngredients = new List<RecipeIngredient>();
        }

        public Recipe(string title, string instructions, int preparationTime, int cookingTime, int servings, string difficultyLevel, int categoryId)
        {
            Title = title;
            Instructions = instructions;
            PreparationTime = preparationTime;
            CookingTime = cookingTime;
            Servings = servings;
            DifficultyLevel = difficultyLevel;
            CategoryId = categoryId;
            RecipeIngredients = new List<RecipeIngredient>();
        }
    }
}