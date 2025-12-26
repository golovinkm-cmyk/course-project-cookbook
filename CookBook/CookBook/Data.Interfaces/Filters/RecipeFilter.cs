namespace Data.Interfaces.Filters;

public class RecipeFilter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? CategoryId { get; set; }
    public string? DifficultyLevel { get; set; }
    public int? MaxCookingTime { get; set; }
    public bool? IsFavorite { get; set; }
    public bool? IsPremium { get; set; }
    public string? SearchKeyword { get; set; }
}