using System.Windows;
using Data.Interfaces;
using Services;
using Interfaces;
using UI.Views;
using Data.InMemory;

namespace UI;

public partial class App : Application
{
    private ICategoryRepository? _categoryRepository;
    private IRecipeRepository? _recipeRepository;
    private IIngredientRepository? _ingredientRepository;
    private ILicenseRepository? _licenseRepository;
    private LicenseService? _licenseService;
    private StatisticsService? _statisticsService;

    private bool _isPremiumMode = false;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Для начала используем InMemory репозитории
        InitializeInMemoryRepositories();

        // Проверяем лицензию
        CheckLicense();

        // Заполняем тестовыми данными
        SeedTestData();

        // Открываем главное окно
        var mainWindow = new MainWindow(
            _recipeRepository!,
            _categoryRepository!,
            _ingredientRepository!,
            _licenseService!,
            _statisticsService!,
            _isPremiumMode);

        mainWindow.Show();
    }

    private void InitializeInMemoryRepositories()
    {
        _categoryRepository = new InMemoryCategoryRepository();
        _recipeRepository = new InMemoryRecipeRepository();
        _ingredientRepository = new InMemoryIngredientRepository();
        _licenseRepository = new InMemoryLicenseRepository();

        _licenseService = new LicenseService(_licenseRepository);
        _statisticsService = new StatisticsService(_recipeRepository, _categoryRepository);
    }

    private void CheckLicense()
    {
        // Проверяем наличие активной лицензии
        _isPremiumMode = _licenseService!.IsPremiumActive();
    }

    private void SeedTestData()
    {
        // Добавляем тестовые категории
        var categories = new[]
        {
            "Супы",
            "Основные блюда",
            "Десерты",
            "Выпечка",
            "Салаты",
            "Напитки"
        };

        foreach (var categoryName in categories)
        {
            _categoryRepository!.Add(new Domain.Entities.Category
            {
                Name = categoryName,
                Description = $"Рецепты из категории {categoryName}"
            });
        }

        // Добавляем тестовые ингредиенты
        var ingredients = new[]
        {
            ("Мука", "г", "Бакалея"),
            ("Сахар", "г", "Бакалея"),
            ("Соль", "г", "Приправы"),
            ("Яйца", "шт", "Молочные"),
            ("Молоко", "мл", "Молочные"),
            ("Масло сливочное", "г", "Молочные"),
            ("Куриное филе", "г", "Мясо"),
            ("Лук", "шт", "Овощи"),
            ("Морковь", "шт", "Овощи"),
            ("Картофель", "шт", "Овощи")
        };

        foreach (var (name, unit, category) in ingredients)
        {
            _ingredientRepository!.Add(new Domain.Entities.Ingredient
            {
                Name = name,
                Unit = unit,
                Category = category
            });
        }

        // Добавляем тестовые рецепты
        var testRecipes = new[]
        {
            new
            {
                Title = "Борщ",
                Category = "Супы",
                Difficulty = "Средний",
                PrepTime = 30,
                CookTime = 60,
                IsPremium = false
            },
            new
            {
                Title = "Курица с картошкой",
                Category = "Основные блюда",
                Difficulty = "Легкий",
                PrepTime = 20,
                CookTime = 40,
                IsPremium = false
            },
            new
            {
                Title = "Шоколадный торт",
                Category = "Десерты",
                Difficulty = "Сложный",
                PrepTime = 60,
                CookTime = 45,
                IsPremium = true
            },
            new
            {
                Title = "Сырники",
                Category = "Выпечка",
                Difficulty = "Легкий",
                PrepTime = 15,
                CookTime = 20,
                IsPremium = false
            },
            new
            {
                Title = "Греческий салат",
                Category = "Салаты",
                Difficulty = "Легкий",
                PrepTime = 20,
                CookTime = 0,
                IsPremium = false
            }
        };

        foreach (var recipe in testRecipes)
        {
            var category = _categoryRepository!.GetAll()
                .FirstOrDefault(c => c.Name == recipe.Category);

            _recipeRepository!.Add(new Domain.Entities.Recipe
            {
                Title = recipe.Title,
                CategoryId = category?.Id ?? 1,
                DifficultyLevel = recipe.Difficulty,
                PreparationTime = recipe.PrepTime,
                CookingTime = recipe.CookTime,
                Servings = 4,
                Instructions = $"Инструкция по приготовлению {recipe.Title}...",
                Description = $"Описание рецепта {recipe.Title}",
                IsFavorite = false,
                IsPremium = recipe.IsPremium
            });
        }
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // Метод для переключения на EF Core при необходимости
        // InitializeEfCoreRepositories();
    }

    private void InitializeEfCoreRepositories()
    {
        // Код для инициализации EF Core репозиториев
        // var configuration = new ConfigurationBuilder()
        //     .SetBasePath(Directory.GetCurrentDirectory())
        //     .AddJsonFile("appsettings.database.json")
        //     .Build();
        //     
        // var factory = new CookBook.Data.SqlServer.CookBookDbContextFactory();
        // var context = factory.CreateDbContext(configuration);
        // 
        // // Применяем миграции
        // context.Database.Migrate();
        // 
        // // Создаем репозитории
        // _categoryRepository = new CookBook.Data.SqlServer.Repositories.EfCategoryRepository(context);
        // _recipeRepository = new CookBook.Data.SqlServer.Repositories.EfRecipeRepository(context);
        // _ingredientRepository = new CookBook.Data.SqlServer.Repositories.EfIngredientRepository(context);
        // _licenseRepository = new CookBook.Data.SqlServer.Repositories.EfLicenseRepository(context);
    }
}
