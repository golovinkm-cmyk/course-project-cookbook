
using System;
using System.Collections.Generic;

namespace Domain
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = "г";
        public string? Category { get; set; }
        public decimal? Calories { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Fat { get; set; }
        public decimal? Carbs { get; set; }

        public virtual ICollection<RecipeIngredient>? RecipeIngredients { get; set; }

        public Ingredient() { }

        public Ingredient(string name, string unit, string? category = null)
        {
            Name = name;
            Unit = unit;
            Category = category;
        }
    }
}