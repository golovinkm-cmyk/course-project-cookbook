namespace CookBook.Domain
{
    public class RecipeIngredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public string? Notes { get; set; }

        public virtual Recipe? Recipe { get; set; }
        public virtual Ingredient? Ingredient { get; set; }

        public RecipeIngredient() { }

        public RecipeIngredient(int recipeId, int ingredientId, decimal quantity, string? notes = null)
        {
            RecipeId = recipeId;
            IngredientId = ingredientId;
            Quantity = quantity;
            Notes = notes;
        }
    }
}
