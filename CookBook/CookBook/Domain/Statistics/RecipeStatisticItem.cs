namespace Domain.Statistics;

public record CategoryStatisticItem
{
    public required string CategoryName { get; set; }
    public required int RecipeCount { get; set; }
}

public record DifficultyStatisticItem
{
    public required string DifficultyLevel { get; set; }
    public required int RecipeCount { get; set; }
}

public record MonthStatisticItem
{
    public required int Year { get; set; }
    public required int Month { get; set; }
    public required int RecipeCount { get; set; }

    public string GetMonthName()
    {
        var date = new DateTime(Year, Month, 1);
        return date.ToString("MMMM yyyy");
    }
}
