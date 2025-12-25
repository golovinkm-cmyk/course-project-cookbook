using System.Windows;
using Data.Interfaces;
using Services;
using Interfaces;
using UI.Views;
using Data.InMemory;
using Data.SqlServer;
using Data.SqlServer.Repositories;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using CookBook.Data.SqlServer;

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
    private CookBookDbContext? _dbContext;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            // Для разработки можно использовать InMemory репозитории
            // Для продакшена - переключаемся на EF Core
            bool useInMemory = false; // Измените на false для использования БД

            if (useInMemory)
            {
                InitializeInMemoryRepositories();
            }
            else
            {
                // 1. Чтение конфигурации из файла
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.database.json")
                    .Build();

                // 2. Создание DbContext через фабрику
                var factory = new CookBookDbContextFactory();
                _dbContext = factory.CreateDbContext(configuration);

                // 3. ВАЖНО: Применение миграций автоматически при запуске
                _dbContext.Database.Migrate();

                // 4. Создание EF Core репозиториев
                _categoryRepository = new EfCategoryRepository(_dbContext);
                _recipeRepository = new EfRecipeRepository(_dbContext);
                _ingredientRepository = new EfIngredientRepository(_dbContext);
                _licenseRepository = new EfLicenseRepository(_dbContext);

                _licenseService = new LicenseService(_licenseRepository);
                _statisticsService = new StatisticsService(_recipeRepository, _categoryRepository);
            }

            // Проверяем лицензию
            CheckLicense();

            // Заполняем тестовыми данными (только если БД пустая)
            SeedTestDataIfNeeded();

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
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}\n\nДетали: {ex.InnerException?.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            // В случае ошибки используем InMemory репозитории как запасной вариант
            InitializeInMemoryRepositories();
            CheckLicense();
            SeedTestData();

            var mainWindow = new MainWindow(
                _recipeRepository!,
                _categoryRepository!,
                _ingredientRepository!,
                _licenseService!,
                _statisticsService!,
                _isPremiumMode);

            mainWindow.Show();
        }
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

    private void SeedTestDataIfNeeded()
    {
        // Проверяем, есть ли уже данные в БД (проверяем по категориям)
        if (_categoryRepository!.GetAll().Any())
        {
            // Данные уже есть, пропускаем заполнение
            return;
        }

        // Заполняем тестовыми данными
        SeedTestData();
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

        var allCategories = _categoryRepository!.GetAll().ToList();

        foreach (var recipe in testRecipes)
        {
            var category = allCategories
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
                IsPremium = recipe.IsPremium,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            });
        }

        // Добавляем тестовую лицензию (для демонстрации премиум-режима)
        _licenseRepository!.Add(new Domain.Entities.License
        {
            LicenseKey = "TEST1234567890ABCDEF",
            LicenseType = "Годовая",
            CustomerName = "Тестовый пользователь",
            CustomerEmail = "test@example.com",
            IsActive = true,
            Amount = 2999.99m,
            PurchaseDate = DateTime.Now,
            ActivationDate = DateTime.Now,
            ExpiryDate = DateTime.Now.AddYears(1)
        });
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // ВАЖНО: Освобождаем ресурсы DbContext при закрытии приложения
        _dbContext?.Dispose();

        base.OnExit(e);
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // Метод для переключения на EF Core при необходимости
        // Оставлен для совместимости
    }
}