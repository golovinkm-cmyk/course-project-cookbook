using Data.Interfaces;
using Data.Interfaces.Filters;
using Domain.Statistics;
using Interfaces;

namespace Services;

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
        var recipes = ApplyFilter(_recipeRepository.GetAll(), filter).ToList();

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
        var recipes = ApplyFilter(_recipeRepository.GetAll(), filter).ToList();

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
        var recipes = ApplyFilter(_recipeRepository.GetAll(), filter).ToList();

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
        var recipes = ApplyFilter(_recipeRepository.GetAll(), filter).ToList();

        return new RecipeStatistics
        {
            TotalRecipes = recipes.Count,
            TotalCookingTime = recipes.Sum(r => r.TotalTime),
            AverageCookingTime = recipes.Any() ?
                (int)recipes.Average(r => r.TotalTime) : 0,
            FavoriteRecipes = recipes.Count(r => r.IsFavorite),
            PremiumRecipes = recipes.Count(r => r.IsPremium)
        };
    }

    private IEnumerable<Domain.Entities.Recipe> ApplyFilter(
        IEnumerable<Domain.Entities.Recipe> recipes, RecipeFilter filter)
    {
        var filteredRecipes = recipes;

        if (filter.StartDate.HasValue)
        {
            filteredRecipes = filteredRecipes.Where(r => r.CreatedDate >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            filteredRecipes = filteredRecipes.Where(r => r.CreatedDate <= filter.EndDate.Value);
        }

        return filteredRecipes;
    }
}

public class RecipeStatistics
{
    public int TotalRecipes { get; init; }
    public int TotalCookingTime { get; init; }
    public int AverageCookingTime { get; init; }
    public int FavoriteRecipes { get; init; }
    public int PremiumRecipes { get; init; }
}