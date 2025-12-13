
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
        public int PreparationTime { get; set; } // в минутах
        public int CookingTime { get; set; } // в минутах
        public int Servings { get; set; }
        public string DifficultyLevel { get; set; } = "Средний";
        public int CategoryId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public bool IsFavorite { get; set; }
        public bool IsPremium { get; set; }
        public string? ImagePath { get; set; }

        // Вычисляемые свойства
        public int TotalTime => PreparationTime + CookingTime;
        public string? CategoryName { get; set; } // Для отображения в UI

        // Навигационные свойства
        public virtual Category? Category { get; set; }
        public virtual ICollection<RecipeIngredient>? RecipeIngredients { get; set; }
    }
}