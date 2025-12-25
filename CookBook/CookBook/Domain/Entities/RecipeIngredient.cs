namespace Domain.Entities;

public class RecipeIngredient
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Навигационные свойства
    public Recipe? Recipe { get; set; }
    public Ingredient? Ingredient { get; set; }
}