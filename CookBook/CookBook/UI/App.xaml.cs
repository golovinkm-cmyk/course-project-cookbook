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

        var addedCategories = new List<Domain.Entities.Category>();
        foreach (var categoryName in categories)
        {
            var category = new Domain.Entities.Category
            {
                Name = categoryName,
                Description = $"Рецепты из категории {categoryName}",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            _categoryRepository!.Add(category);
            addedCategories.Add(category);
        }

        // Добавляем тестовые ингредиенты
        var ingredients = new[]
        {
            ("Мука пшеничная", "г", "Бакалея"),
            ("Сахар", "г", "Бакалея"),
            ("Соль", "г", "Приправы"),
            ("Перец черный молотый", "г", "Приправы"),
            ("Яйца куриные", "шт", "Молочные"),
            ("Молоко", "мл", "Молочные"),
            ("Масло сливочное", "г", "Молочные"),
            ("Масло растительное", "мл", "Бакалея"),
            ("Куриное филе", "г", "Мясо"),
            ("Говядина", "г", "Мясо"),
            ("Свинина", "г", "Мясо"),
            ("Лук репчатый", "шт", "Овощи"),
            ("Морковь", "шт", "Овощи"),
            ("Картофель", "шт", "Овощи"),
            ("Помидоры", "шт", "Овощи"),
            ("Огурцы", "шт", "Овощи"),
            ("Капуста белокочанная", "г", "Овощи"),
            ("Свекла", "шт", "Овощи"),
            ("Чеснок", "зубчик", "Овощи"),
            ("Укроп", "г", "Зелень"),
            ("Петрушка", "г", "Зелень"),
            ("Сметана", "г", "Молочные"),
            ("Творог", "г", "Молочные"),
            ("Сыр", "г", "Молочные"),
            ("Майонез", "г", "Соусы"),
            ("Томатная паста", "г", "Соусы"),
            ("Вода", "мл", "Бакалея"),
            ("Чай черный", "г", "Напитки"),
            ("Кофе молотый", "г", "Напитки")
        };

        var addedIngredients = new List<Domain.Entities.Ingredient>();
        foreach (var (name, unit, category) in ingredients)
        {
            var ingredient = new Domain.Entities.Ingredient
            {
                Name = name,
                Unit = unit,
                Category = category
            };
            _ingredientRepository!.Add(ingredient);
            addedIngredients.Add(ingredient);
        }

        // Добавляем тестовые рецепты с полными описаниями
        var testRecipes = new[]
        {
            new
            {
                Title = "Борщ классический",
                Category = "Супы",
                Difficulty = "Средний",
                PrepTime = 40,
                CookTime = 90,
                Servings = 6,
                IsPremium = false,
                Description = "Наваристый борщ по традиционному рецепту с говядиной и свеклой",
                Instructions = "1. Говядину залить холодной водой, довести до кипения, снять пену.\n" +
                             "2. Варить мясо 1,5 часа до готовности.\n" +
                             "3. Свеклу, морковь и лук нарезать соломкой, обжарить на растительном масле.\n" +
                             "4. Добавить томатную пасту, тушить 10 минут.\n" +
                             "5. Картофель нарезать кубиками, добавить в бульон.\n" +
                             "6. Капусту нашинковать, добавить через 10 минут.\n" +
                             "7. Через 15 минут добавить зажарку, варить еще 10 минут.\n" +
                             "8. Добавить измельченный чеснок, зелень, дать настояться 20 минут.",
                Ingredients = new[]
                {
                    ("Говядина", 500m, "г"),
                    ("Картофель", 3m, "шт"),
                    ("Капуста белокочанная", 300m, "г"),
                    ("Свекла", 2m, "шт"),
                    ("Морковь", 1m, "шт"),
                    ("Лук репчатый", 1m, "шт"),
                    ("Томатная паста", 2m, "ст.л."),
                    ("Чеснок", 3m, "зубчик"),
                    ("Укроп", 20m, "г"),
                    ("Сметана", 100m, "г")
                }
            },
            new
            {
                Title = "Курица с картошкой в духовке",
                Category = "Основные блюда",
                Difficulty = "Легкий",
                PrepTime = 20,
                CookTime = 60,
                Servings = 4,
                IsPremium = false,
                Description = "Сочная курица с золотистой картошкой, запеченная с пряными травами",
                Instructions = "1. Курицу промыть, обсушить, натереть солью и перцем.\n" +
                             "2. Картофель очистить, нарезать крупными дольками.\n" +
                             "3. Лук нарезать кольцами.\n" +
                             "4. Смешать картофель и лук с растительным маслом, солью, перцем.\n" +
                             "5. Выложить курицу и картофель в форму для запекания.\n" +
                             "6. Посыпать сушеными травами.\n" +
                             "7. Запекать при 200°C 50-60 минут до румяной корочки.\n" +
                             "8. Подавать с зеленью и свежими овощами.",
                Ingredients = new[]
                {
                    ("Куриное филе", 800m, "г"),
                    ("Картофель", 6m, "шт"),
                    ("Лук репчатый", 2m, "шт"),
                    ("Масло растительное", 3m, "ст.л."),
                    ("Соль", 10m, "г"),
                    ("Перец черный молотый", 5m, "г"),
                    ("Чеснок", 4m, "зубчик"),
                    ("Петрушка", 15m, "г")
                }
            },
            new
            {
                Title = "Шоколадный торт Наполеон",
                Category = "Десерты",
                Difficulty = "Сложный",
                PrepTime = 90,
                CookTime = 45,
                Servings = 8,
                IsPremium = true,
                Description = "Нежнейший торт из слоеного теста с шоколадным кремом",
                Instructions = "1. Приготовить слоеное тесто или использовать готовое.\n" +
                             "2. Раскатать тесто в тонкий пласт, нарезать на коржи.\n" +
                             "3. Выпекать коржи при 200°C 10-15 минут до золотистого цвета.\n" +
                             "4. Для крема: сливки взбить с сахаром до устойчивых пиков.\n" +
                             "5. Добавить растопленный шоколад, тщательно перемешать.\n" +
                             "6. Каждый корж промазать кремом, собирая торт.\n" +
                             "7. Верх и бока украсить крошкой из коржей и шоколадной стружкой.\n" +
                             "8. Охладить в холодильнике минимум 4 часа перед подачей.",
                Ingredients = new[]
                {
                    ("Слоеное тесто", 500m, "г"),
                    ("Сливки 33%", 500m, "мл"),
                    ("Шоколад темный", 200m, "г"),
                    ("Сахар", 150m, "г"),
                    ("Масло сливочное", 100m, "г"),
                    ("Яйца куриные", 2m, "шт"),
                    ("Ванильный сахар", 10m, "г")
                }
            },
            new
            {
                Title = "Сырники с изюмом",
                Category = "Выпечка",
                Difficulty = "Легкий",
                PrepTime = 20,
                CookTime = 20,
                Servings = 4,
                IsPremium = false,
                Description = "Нежные творожные сырники с изюмом, жареные до золотистой корочки",
                Instructions = "1. Творог протереть через сино или измельчить блендером.\n" +
                             "2. Добавить яйца, сахар, соль, ванилин, перемешать.\n" +
                             "3. Постепенно всыпать муку, замесить мягкое тесто.\n" +
                             "4. Добавить промытый изюм.\n" +
                             "5. Сформировать сырники, обвалять в муке.\n" +
                             "6. Разогреть сковороду с маслом.\n" +
                             "7. Жарить сырники на среднем огне 4-5 минут с каждой стороны.\n" +
                             "8. Подавать со сметаной, вареньем или медом.",
                Ingredients = new[]
                {
                    ("Творог", 500m, "г"),
                    ("Мука пшеничная", 100m, "г"),
                    ("Яйца куриные", 2m, "шт"),
                    ("Сахар", 80m, "г"),
                    ("Изюм", 50m, "г"),
                    ("Ванильный сахар", 5m, "г"),
                    ("Масло растительное", 50m, "мл"),
                    ("Сметана", 100m, "г")
                }
            },
            new
            {
                Title = "Греческий салат",
                Category = "Салаты",
                Difficulty = "Легкий",
                PrepTime = 20,
                CookTime = 0,
                Servings = 4,
                IsPremium = false,
                Description = "Классический греческий салат с фетаксой, свежими овощами и оливковым маслом",
                Instructions = "1. Помидоры нарезать крупными дольками.\n" +
                             "2. Огурцы нарезать полукольцами.\n" +
                             "3. Лук нарезать тонкими полукольцами.\n" +
                             "4. Перец нарезать соломкой.\n" +
                             "5. Сыр фета нарезать кубиками.\n" +
                             "6. Маслины оставить целыми.\n" +
                             "7. Смешать все ингредиенты в салатнице.\n" +
                             "8. Заправить оливковым маслом, посыпать орегано, соль, перец.\n" +
                             "9. Аккуратно перемешать и сразу подавать.",
                Ingredients = new[]
                {
                    ("Помидоры", 4m, "шт"),
                    ("Огурцы", 2m, "шт"),
                    ("Лук репчатый", 1m, "шт"),
                    ("Перец сладкий", 1m, "шт"),
                    ("Сыр фета", 200m, "г"),
                    ("Маслины", 100m, "г"),
                    ("Масло оливковое", 3m, "ст.л."),
                    ("Орегано", 5m, "г"),
                    ("Соль", 5m, "г"),
                    ("Перец черный молотый", 3m, "г")
                }
            },
            new
            {
                Title = "Латте с карамелью",
                Category = "Напитки",
                Difficulty = "Средний",
                PrepTime = 10,
                CookTime = 5,
                Servings = 1,
                IsPremium = true,
                Description = "Нежный кофейный напиток с молоком и карамельным сиропом",
                Instructions = "1. Приготовить эспрессо (30 мл).\n" +
                             "2. Молоко подогреть до 65°C, взбить капучинатором до пены.\n" +
                             "3. В бокал налить карамельный сироп.\n" +
                             "4. Добавить приготовленный эспрессо.\n" +
                             "5. Аккуратно влить взбитое молоко, сохраняя пену.\n" +
                             "6. Украсить карамельным соусом.\n" +
                             "7. Подавать сразу после приготовления.",
                Ingredients = new[]
                {
                    ("Кофе молотый", 15m, "г"),
                    ("Молоко", 200m, "мл"),
                    ("Карамельный сироп", 20m, "мл"),
                    ("Сахар", 10m, "г"),
                    ("Карамельный соус", 10m, "мл")
                }
            }
        };

        foreach (var recipe in testRecipes)
        {
            var category = addedCategories.FirstOrDefault(c => c.Name == recipe.Category);

            var newRecipe = new Domain.Entities.Recipe
            {
                Title = recipe.Title,
                CategoryId = category?.Id ?? 1,
                DifficultyLevel = recipe.Difficulty,
                PreparationTime = recipe.PrepTime,
                CookingTime = recipe.CookTime,
                Servings = recipe.Servings,
                Instructions = recipe.Instructions,
                Description = recipe.Description,
                IsFavorite = false,
                IsPremium = recipe.IsPremium,
                CreatedDate = DateTime.Now.AddDays(-new Random().Next(1, 30)),
                ModifiedDate = DateTime.Now
            };

            _recipeRepository!.Add(newRecipe);
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