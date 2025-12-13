using System;

namespace Domain
{
    public class RecipeIngredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public string? Notes { get; set; }

        // Навигационные свойства
        public virtual Recipe? Recipe { get; set; }
        public virtual Ingredient? Ingredient { get; set; }

        public RecipeIngredient() { }

        public RecipeIngredient(int recipeId, int ingredientId, decimal quantity, string? unit = null, string? notes = null)
        {
            RecipeId = recipeId;
            IngredientId = ingredientId;
            Quantity = quantity;
            Unit = unit;
            Notes = notes;
        }
    }
}