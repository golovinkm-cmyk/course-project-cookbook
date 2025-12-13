using Data.InMemory;
using Data.Interfaces;
using Data.SqlServer;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace UI
{
    public partial class App : Application
    {
        private CookbookDbContext? _dbContext;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // 1. Чтение конфигурации из файла
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.database.json", optional: true, reloadOnChange: false)
                    .Build();

                // 2. Создание DbContext через фабрику
                var factory = new CookbookDbContextFactory();
                _dbContext = factory.CreateDbContext(configuration);

                // 3. Автоматическое применение миграций (создание БД если её нет)
                _dbContext.Database.Migrate();

                // 4. Заполнение тестовыми данными (только если БД пустая)
                SeedInitialData();

                // 5. Создание репозиториев
                var recipeRepository = new Data.SqlServer.RecipeRepository(_dbContext);
                var categoryRepository = new Data.SqlServer.CategoryRepository(_dbContext);
                var ingredientRepository = new Data.SqlServer.IngredientRepository(_dbContext);
                var recipeIngredientRepository = new Data.SqlServer.RecipeIngredientRepository(_dbContext);
                var licenseRepository = new Data.SqlServer.LicenseRepository(_dbContext);

                // 6. Запуск главного окна
                var mainWindow = new MainWindow(
                    recipeRepository,
                    categoryRepository,
                    ingredientRepository,
                    recipeIngredientRepository,
                    licenseRepository);
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}\n\n" +
                    $"Убедитесь, что SQL Server LocalDB установлен и запущен.\n" +
                    $"Для установки используйте Visual Studio Installer → Individual Components → SQL Server Express LocalDB",
                    "Ошибка запуска",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown();
            }
        }

        private void SeedInitialData()
        {
            if (_dbContext == null) return;

            // Проверяем, есть ли уже данные в БД
            if (_dbContext.Categories.Any() || _dbContext.Recipes.Any())
            {
                // Данные уже есть, пропускаем заполнение
                return;
            }

            try
            {
                // 1. Создание категорий
                var categories = new[]
                {
                    new Category("Основные блюда", "Горячие блюда на обед или ужин"),
                    new Category("Закуски", "Холодные и горячие закуски"),
                    new Category("Десерты", "Сладкие блюда и выпечка"),
                    new Category("Завтраки", "Блюда для утреннего приема пищи"),
                    new Category("Напитки", "Горячие и холодные напитки"),
                    new Category("Супы", "Первые блюда и бульоны")
                };

                _dbContext.Categories.AddRange(categories);
                _dbContext.SaveChanges();

                // 2. Создание ингредиентов
                var ingredients = new[]
                {
                    new Ingredient("Картофель", "кг", "Овощи") { Calories = 77, Protein = 2, Carbs = 17 },
                    new Ingredient("Лук репчатый", "шт", "Овощи") { Calories = 40, Protein = 1.1m, Carbs = 9.3m },
                    new Ingredient("Морковь", "шт", "Овощи") { Calories = 41, Protein = 0.9m, Carbs = 9.6m },
                    new Ingredient("Помидор", "шт", "Овощи") { Calories = 18, Protein = 0.9m, Carbs = 3.9m },
                    new Ingredient("Куриное филе", "г", "Мясо и птица") { Calories = 165, Protein = 31, Fat = 3.6m },
                    new Ingredient("Говядина", "г", "Мясо и птица") { Calories = 250, Protein = 26, Fat = 15 },
                    new Ingredient("Молоко", "мл", "Молочные продукты") { Calories = 42, Protein = 3.4m, Fat = 1 },
                    new Ingredient("Сметана", "г", "Молочные продукты") { Calories = 206, Protein = 2.5m, Fat = 20 },
                    new Ingredient("Сыр", "г", "Молочные продукты") { Calories = 350, Protein = 26, Fat = 26 },
                    new Ingredient("Мука пшеничная", "г", "Бакалея") { Calories = 364, Protein = 10.3m, Carbs = 76.1m },
                    new Ingredient("Сахар", "г", "Бакалея") { Calories = 387, Carbs = 99.8m },
                    new Ingredient("Соль", "ч.л.", "Бакалея"),
                    new Ingredient("Перец черный", "ч.л.", "Бакалея"),
                    new Ingredient("Масло растительное", "ст.л.", "Бакалея") { Calories = 884, Fat = 100 },
                    new Ingredient("Масло сливочное", "г", "Бакалея") { Calories = 717, Fat = 81 },
                    new Ingredient("Яйца", "шт", "Бакалея") { Calories = 155, Protein = 13, Fat = 11 },
                    new Ingredient("Чеснок", "зубчик", "Овощи") { Calories = 149, Protein = 6.4m, Carbs = 33.1m },
                    new Ingredient("Зелень (укроп/петрушка)", "пучок", "Овощи"),
                    new Ingredient("Лимон", "шт", "Фрукты") { Calories = 29, VitaminC = 53 },
                    new Ingredient("Мёд", "г", "Бакалея") { Calories = 304, Carbs = 82.4m }
                };

                _dbContext.Ingredients.AddRange(ingredients);
                _dbContext.SaveChanges();

                // 3. Создание рецептов
                var recipes = new[]
                {
                    new Recipe
                    {
                        Title = "Картофель по-деревенски",
                        Description = "Вкусный запеченный картофель с травами и чесноком",
                        Instructions = "1. Картофель помыть и нарезать дольками\n" +
                                      "2. Смешать с растительным маслом, солью, перцем и измельченным чесноком\n" +
                                      "3. Выложить на противень, застеленный бумагой для выпечки\n" +
                                      "4. Запекать 30-40 минут при 200°C до золотистой корочки\n" +
                                      "5. Перед подачей посыпать свежей зеленью",
                        PreparationTime = 15,
                        CookingTime = 40,
                        Servings = 4,
                        DifficultyLevel = "Легкий",
                        CategoryId = 1, // Основные блюда
                        IsFavorite = true,
                        IsPremium = false,
                        CreatedDate = DateTime.Now.AddDays(-10)
                    },
                    new Recipe
                    {
                        Title = "Салат Цезарь",
                        Description = "Классический салат с курицей, сухариками и соусом",
                        Instructions = "1. Куриное филе нарезать полосками, обжарить до готовности\n" +
                                      "2. Салат романо порвать руками на крупные куски\n" +
                                      "3. Помидоры черри разрезать пополам\n" +
                                      "4. Приготовить соус: смешать майонез, чеснок, лимонный сок, пармезан\n" +
                                      "5. Смешать все ингредиенты, заправить соусом, посыпать сухариками",
                        PreparationTime = 20,
                        CookingTime = 15,
                        Servings = 2,
                        DifficultyLevel = "Средний",
                        CategoryId = 2, // Закуски
                        IsFavorite = false,
                        IsPremium = true,
                        CreatedDate = DateTime.Now.AddDays(-5)
                    },
                    new Recipe
                    {
                        Title = "Шоколадный торт",
                        Description = "Нежный шоколадный торт с кремом из сливок",
                        Instructions = "1. Просеять муку, какао и разрыхлитель\n" +
                                      "2. Взбить яйца с сахаром до пышной массы\n" +
                                      "3. Добавить растопленное сливочное масло и молоко\n" +
                                      "4. Постепенно ввести сухие ингредиенты\n" +
                                      "5. Выпекать 30-35 минут при 180°C\n" +
                                      "6. Для крема взбить сливки с сахарной пудрой\n" +
                                      "7. Остывший торт разрезать на коржи, промазать кремом\n" +
                                      "8. Украсить шоколадной стружкой",
                        PreparationTime = 40,
                        CookingTime = 60,
                        Servings = 8,
                        DifficultyLevel = "Сложный",
                        CategoryId = 3, // Десерты
                        IsFavorite = true,
                        IsPremium = true,
                        CreatedDate = DateTime.Now.AddDays(-3)
                    },
                    new Recipe
                    {
                        Title = "Омлет с овощами",
                        Description = "Пышный омлет с помидорами, перцем и зеленью",
                        Instructions = "1. Яйца взбить с молоком, солью и перцем\n" +
                                      "2. Помидоры и перец нарезать кубиками\n" +
                                      "3. Обжарить овощи на сковороде 3-4 минуты\n" +
                                      "4. Залить яичной смесью\n" +
                                      "5. Накрыть крышкой и готовить на среднем огне 7-10 минут\n" +
                                      "6. Посыпать зеленью перед подачей",
                        PreparationTime = 10,
                        CookingTime = 15,
                        Servings = 2,
                        DifficultyLevel = "Легкий",
                        CategoryId = 4, // Завтраки
                        IsFavorite = true,
                        IsPremium = false,
                        CreatedDate = DateTime.Now.AddDays(-1)
                    },
                    new Recipe
                    {
                        Title = "Домашний лимонад",
                        Description = "Освежающий лимонад с мятой и медом",
                        Instructions = "1. Лимоны вымыть, нарезать дольками\n" +
                                      "2. В кувшин положить лимоны, мяту и мед\n" +
                                      "3. Размять толкушкой для выделения сока\n" +
                                      "4. Залить холодной водой, хорошо перемешать\n" +
                                      "5. Охладить в холодильнике 1-2 часа\n" +
                                      "6. Подавать с кубиками льда и долькой лимона",
                        PreparationTime = 15,
                        CookingTime = 0,
                        Servings = 4,
                        DifficultyLevel = "Легкий",
                        CategoryId = 5, // Напитки
                        IsFavorite = false,
                        IsPremium = false,
                        CreatedDate = DateTime.Now.AddDays(-2)
                    }
                };

                _dbContext.Recipes.AddRange(recipes);
                _dbContext.SaveChanges();

                // 4. Создание связей рецепт-ингредиент
                var recipeIngredients = new[]
                {
                    // Картофель по-деревенски (RecipeId = 1)
                    new RecipeIngredient { RecipeId = 1, IngredientId = 1, Quantity = 1, Unit = "кг", Notes = "Молодой картофель" },
                    new RecipeIngredient { RecipeId = 1, IngredientId = 14, Quantity = 3, Unit = "ст.л.", Notes = "Для заправки" },
                    new RecipeIngredient { RecipeId = 1, IngredientId = 12, Quantity = 1, Unit = "ч.л.", Notes = "По вкусу" },
                    new RecipeIngredient { RecipeId = 1, IngredientId = 13, Quantity = 0.5m, Unit = "ч.л.", Notes = "Свежемолотый" },
                    new RecipeIngredient { RecipeId = 1, IngredientId = 17, Quantity = 3, Unit = "зубчик", Notes = "Измельченный" },
                    new RecipeIngredient { RecipeId = 1, IngredientId = 18, Quantity = 1, Unit = "пучок", Notes = "Для украшения" },
                    
                    // Салат Цезарь (RecipeId = 2)
                    new RecipeIngredient { RecipeId = 2, IngredientId = 5, Quantity = 300, Unit = "г", Notes = "Грудка" },
                    new RecipeIngredient { RecipeId = 2, IngredientId = 4, Quantity = 200, Unit = "г", Notes = "Помидоры черри" },
                    new RecipeIngredient { RecipeId = 2, IngredientId = 9, Quantity = 100, Unit = "г", Notes = "Пармезан" },
                    
                    // Шоколадный торт (RecipeId = 3)
                    new RecipeIngredient { RecipeId = 3, IngredientId = 10, Quantity = 300, Unit = "г", Notes = "Высший сорт" },
                    new RecipeIngredient { RecipeId = 3, IngredientId = 11, Quantity = 250, Unit = "г", Notes = "" },
                    new RecipeIngredient { RecipeId = 3, IngredientId = 16, Quantity = 4, Unit = "шт", Notes = "Крупные" },
                    new RecipeIngredient { RecipeId = 3, IngredientId = 15, Quantity = 200, Unit = "г", Notes = "" },
                    new RecipeIngredient { RecipeId = 3, IngredientId = 7, Quantity = 200, Unit = "мл", Notes = "" },
                    
                    // Омлет с овощами (RecipeId = 4)
                    new RecipeIngredient { RecipeId = 4, IngredientId = 16, Quantity = 4, Unit = "шт", Notes = "" },
                    new RecipeIngredient { RecipeId = 4, IngredientId = 7, Quantity = 100, Unit = "мл", Notes = "" },
                    new RecipeIngredient { RecipeId = 4, IngredientId = 4, Quantity = 2, Unit = "шт", Notes = "Средние" },
                    
                    // Домашний лимонад (RecipeId = 5)
                    new RecipeIngredient { RecipeId = 5, IngredientId = 19, Quantity = 3, Unit = "шт", Notes = "Средние" },
                    new RecipeIngredient { RecipeId = 5, IngredientId = 20, Quantity = 100, Unit = "г", Notes = "По вкусу" }
                };

                _dbContext.RecipeIngredients.AddRange(recipeIngredients);
                _dbContext.SaveChanges();

                // 5. Создание тестовой лицензии
                var license = new License
                {
                    LicenseKey = "ABCD-1234-EFGH-5678",
                    LicenseType = "Пожизненная",
                    PurchaseDate = new DateTime(2024, 1, 15),
                    ActivationDate = new DateTime(2024, 1, 15),
                    ExpiryDate = null,
                    IsActive = true,
                    CustomerName = "Иван Иванов",
                    CustomerEmail = "ivan@example.com",
                    Amount = 999,
                    PaymentMethod = "Карта",
                    PaymentStatus = "Успешно",
                    TransactionId = "TXN123456",
                    CardLastFour = "1234"
                };

                _dbContext.Licenses.Add(license);
                _dbContext.SaveChanges();

                MessageBox.Show("Тестовые данные успешно добавлены в базу данных!",
                    "Инициализация БД",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении тестовых данных: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Освобождаем ресурсы DbContext при закрытии приложения
            _dbContext?.Dispose();
            base.OnExit(e);
        }
    }
}
