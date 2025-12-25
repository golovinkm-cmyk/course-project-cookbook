namespace Domain.Entities;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public string? Category { get; set; }

    // Навигационные свойства
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
