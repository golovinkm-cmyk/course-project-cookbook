namespace Domain.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public int PreparationTime { get; set; } // в минутах
    public int CookingTime { get; set; } // в минутах
    public int Servings { get; set; }
    public string DifficultyLevel { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
    public bool IsPremium { get; set; }

    // Вычисляемое свойство (не мапится в БД)
    public int TotalTime => PreparationTime + CookingTime;

    // Внешний ключ и навигационное свойство для категории
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Даты
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    // Навигационные свойства
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
