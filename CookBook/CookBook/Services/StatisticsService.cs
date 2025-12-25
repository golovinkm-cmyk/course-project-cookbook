using Data.Interfaces;  // ← исправлено
using Data.Interfaces.Filters;
using Domain.Statistics;
using Interfaces;





namespace Services;  // ← исправлено

public class StatisticsService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICategoryRepository _categoryRepository;

    public StatisticsService(IRecipeRepository recipeRepository,
        ICategoryRepository categoryRepository)
    {
        _recipeRepository = recipeRepository;
        _categoryRepository = categoryRepository;
    }

    public IEnumerable<CategoryStatisticItem> GetRecipesByCategory(RecipeFilter filter)
    {
        var categories = _categoryRepository.GetAll().ToList();
        var recipes = _recipeRepository.GetAll(); 

        return categories
            .Select(c => new CategoryStatisticItem
            {
                CategoryName = c.Name,
                RecipeCount = recipes.Count(r => r.CategoryId == c.Id)
            })
            .Where(s => s.RecipeCount > 0)
            .OrderByDescending(s => s.RecipeCount)
            .ToList();
    }

    public IEnumerable<DifficultyStatisticItem> GetRecipesByDifficulty(RecipeFilter filter)
    {
        var recipes = _recipeRepository.GetAll(); 

        return recipes
            .GroupBy(r => r.DifficultyLevel)
            .Select(g => new DifficultyStatisticItem
            {
                DifficultyLevel = g.Key,
                RecipeCount = g.Count()
            })
            .OrderBy(s => s.DifficultyLevel)
            .ToList();
    }

    public IEnumerable<MonthStatisticItem> GetRecipesByMonth(RecipeFilter filter)
    {
        var recipes = _recipeRepository.GetAll(); 

        return recipes
            .GroupBy(r => new { r.CreatedDate.Year, r.CreatedDate.Month })
            .Select(g => new MonthStatisticItem
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                RecipeCount = g.Count()
            })
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();
    }

    public RecipeStatistics GetRecipeStatistics(RecipeFilter filter)
    {
        var recipes = _recipeRepository.GetAll(); // ← упростили
        var recipesList = recipes.ToList();

        return new RecipeStatistics
        {
            TotalRecipes = recipesList.Count,
            TotalCookingTime = recipesList.Sum(r => r.TotalTime),
            AverageCookingTime = recipesList.Any() ?
                (int)recipesList.Average(r => r.TotalTime) : 0,
            FavoriteRecipes = recipesList.Count(r => r.IsFavorite),
            PremiumRecipes = recipesList.Count(r => r.IsPremium)
        };
    }
}

public class RecipeStatistics
{
    public int TotalRecipes { get; set; }
    public int TotalCookingTime { get; set; } // в минутах
    public int AverageCookingTime { get; set; } // в минутах
    public int FavoriteRecipes { get; set; }
    public int PremiumRecipes { get; set; }
}

