using Data.InMemory;
using Data.Interfaces;
using Domain;
using Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UI;

namespace UI
{
    public partial class MainWindow : Window
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IRecipeIngredientRepository _recipeIngredientRepository;
        private readonly ILicenseRepository _licenseRepository;

        private ObservableCollection<Recipe> _recipes = new();
        private ObservableCollection<Category> _categories = new();

        public MainWindow() : this(
        new RecipeRepository(),
        new CategoryRepository(),
        new IngredientRepository(),
        new RecipeIngredientRepository(),
        new LicenseRepository())
        {
        }

        // Добавляем новый конструктор с параметрами
        public MainWindow(
            IRecipeRepository recipeRepository,
            ICategoryRepository categoryRepository,
            IIngredientRepository ingredientRepository,
            IRecipeIngredientRepository recipeIngredientRepository,
            ILicenseRepository licenseRepository)
        {
            InitializeComponent();

            // Инициализация репозиториев
            _recipeRepository = recipeRepository;
            _categoryRepository = categoryRepository;
            _ingredientRepository = ingredientRepository;
            _recipeIngredientRepository = recipeIngredientRepository;
            _licenseRepository = licenseRepository;

            // Загрузка данных
            LoadRecipes();
            LoadCategories();
            UpdateStatus();

            // Настройка ComboBox категорий
            categoryComboBox.ItemsSource = _categories;
            categoryComboBox.DisplayMemberPath = "Name";
            categoryComboBox.SelectedValuePath = "Id";
        }

        private void LoadRecipes()
        {
            _recipes.Clear();
            var recipes = _recipeRepository.GetAll();
            foreach (var recipe in recipes)
            {
                _recipes.Add(recipe);
            }
            recipesDataGrid.ItemsSource = _recipes;
            UpdateStatus();
        }

        private void LoadCategories()
        {
            _categories.Clear();
            var categories = _categoryRepository.GetAll();
            foreach (var category in categories)
            {
                _categories.Add(category);
            }
            categoriesDataGrid.ItemsSource = _categories;
        }

        private void UpdateStatus()
        {
            int totalRecipes = _recipes.Count;
            int demoLimit = 5;

            statusTextBlock.Text = $"Всего рецептов: {totalRecipes}";

            if (totalRecipes <= demoLimit)
            {
                demoLimitTextBlock.Text = $"Доступно рецептов: {totalRecipes}/{demoLimit}";
                demoLimitTextBlock.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                demoLimitTextBlock.Text = $"Ограничение демо-версии: {demoLimit} рецептов";
                demoLimitTextBlock.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                recipesDataGrid.ItemsSource = _recipes;
            }
            else
            {
                var filtered = new ObservableCollection<Recipe>(
                    _recipes.Where(r =>
                        r.Title.ToLower().Contains(searchText) ||
                        (r.Description != null && r.Description.ToLower().Contains(searchText)) ||
                        r.Instructions.ToLower().Contains(searchText)
                    )
                );
                recipesDataGrid.ItemsSource = filtered;
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoryComboBox.SelectedValue is int categoryId)
            {
                var filtered = new ObservableCollection<Recipe>(
                    _recipes.Where(r => r.CategoryId == categoryId)
                );
                recipesDataGrid.ItemsSource = filtered;
            }
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";
            categoryComboBox.SelectedIndex = -1;
            recipesDataGrid.ItemsSource = _recipes;
        }

        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new RecipeEditWindow(
                _recipeRepository,
                _categoryRepository,
                _ingredientRepository,
                _recipeIngredientRepository,
                null // null для создания нового рецепта
            );

            if (editWindow.ShowDialog() == true)
            {
                LoadRecipes();
            }
        }

        private void EditRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (recipesDataGrid.SelectedItem is Recipe selectedRecipe)
            {
                var editWindow = new RecipeEditWindow(
                    _recipeRepository,
                    _categoryRepository,
                    _ingredientRepository,
                    _recipeIngredientRepository,
                    selectedRecipe.Id
                );

                if (editWindow.ShowDialog() == true)
                {
                    LoadRecipes();
                }
            }
            else
            {
                MessageBox.Show("Выберите рецепт для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ViewRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (recipesDataGrid.SelectedItem is Recipe selectedRecipe)
            {
                // Получаем полную информацию о рецепте
                var recipe = _recipeRepository.GetById(selectedRecipe.Id);
                if (recipe != null)
                {
                    var viewWindow = new RecipeViewWindow(
                        recipe,
                        _recipeIngredientRepository,
                        _ingredientRepository,
                        _categoryRepository);
                    viewWindow.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Выберите рецепт для просмотра", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (recipesDataGrid.SelectedItem is Recipe selectedRecipe)
            {
                var result = MessageBox.Show($"Удалить рецепт '{selectedRecipe.Title}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_recipeRepository.Delete(selectedRecipe.Id))
                    {
                        // Также удаляем связанные ингредиенты
                        _recipeIngredientRepository.DeleteByRecipeId(selectedRecipe.Id);

                        LoadRecipes();
                        MessageBox.Show("Рецепт удален", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private void ToggleFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (recipesDataGrid.SelectedItem is Recipe selectedRecipe)
            {
                var recipe = _recipeRepository.GetById(selectedRecipe.Id);
                if (recipe != null)
                {
                    recipe.IsFavorite = !recipe.IsFavorite;
                    _recipeRepository.Update(recipe);
                    LoadRecipes();

                    string message = recipe.IsFavorite ?
                        "Рецепт добавлен в избранное" :
                        "Рецепт удален из избранного";
                    MessageBox.Show(message, "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            var statisticsWindow = new StatisticsWindow(_recipeRepository);
            statisticsWindow.ShowDialog();
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var categoryWindow = new CategoriesWindow(_categoryRepository, _recipeRepository);  // ← Добавили _recipeRepository
            if (categoryWindow.ShowDialog() == true)
            {
                LoadCategories();
                MessageBox.Show("Категория добавлена", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (categoriesDataGrid.SelectedItem is Category selectedCategory)
            {
                var categoryWindow = new CategoriesWindow(_categoryRepository, _recipeRepository, selectedCategory.Id);  // ← Три параметра
                if (categoryWindow.ShowDialog() == true)
                {
                    LoadCategories();
                }
            }
        }

        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (categoriesDataGrid.SelectedItem is Category selectedCategory)
            {
                // Проверяем, есть ли рецепты в категории
                var recipesInCategory = _recipeRepository.GetByCategory(selectedCategory.Id);

                if (recipesInCategory.Any())
                {
                    MessageBox.Show($"Нельзя удалить категорию '{selectedCategory.Name}'. В ней есть рецепты.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show($"Удалить категорию '{selectedCategory.Name}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_categoryRepository.Delete(selectedCategory.Id))
                    {
                        LoadCategories();
                        MessageBox.Show("Категория удалена", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private void BuyLicenseButton_Click(object sender, RoutedEventArgs e)
        {
            var licenseWindow = new LicenseWindow(_licenseRepository);  // ← УЖЕ ПРАВИЛЬНО
            licenseWindow.ShowDialog();
        }
    }
}