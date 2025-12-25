using System.Windows;
using System.Windows.Controls;
using Data.Interfaces;
using Domain.Entities;
using Services;
using Interfaces;

namespace UI.Views;

public partial class MainWindow : Window
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly LicenseService _licenseService;
    private readonly StatisticsService _statisticsService;
    private readonly bool _isPremiumMode;

    private Recipe? _selectedRecipe;

    public MainWindow(IRecipeRepository recipeRepository,
                     ICategoryRepository categoryRepository,
                     IIngredientRepository ingredientRepository,
                     LicenseService licenseService,
                     StatisticsService statisticsService,
                     bool isPremiumMode)
    {
        InitializeComponent();

        _recipeRepository = recipeRepository;
        _categoryRepository = categoryRepository;
        _ingredientRepository = ingredientRepository;
        _licenseService = licenseService;
        _statisticsService = statisticsService;
        _isPremiumMode = isPremiumMode;

        InitializeUI();
        LoadData();
        UpdateStatus();
    }

    private void InitializeUI()
    {
        // Заполняем комбобокс категорий
        CategoryComboBox.Items.Clear();
        CategoryComboBox.Items.Add("Все категории");

        foreach (var category in _categoryRepository.GetAll())
        {
            CategoryComboBox.Items.Add(category.Name);
        }

        CategoryComboBox.SelectedIndex = 0;

        // Обновляем тексты в зависимости от режима
        ModeText.Text = _isPremiumMode ? "Полная версия" : "Демо";
        DemoInfoBorder.Visibility = _isPremiumMode ? Visibility.Collapsed : Visibility.Visible;
    }

    private void LoadData()
    {
        var recipes = _recipeRepository.GetAll();

        if (!_isPremiumMode)
        {
            // В демо-режиме показываем только 5 рецептов и скрываем премиум
            recipes = recipes.Where(r => !r.IsPremium).Take(5);
        }

        RecipesDataGrid.ItemsSource = recipes;
        RecipeCountText.Text = recipes.Count().ToString();
    }

    private void UpdateStatus()
    {
        var totalRecipes = _recipeRepository.GetAll().Count();
        var demoRecipes = _recipeRepository.GetAll().Where(r => !r.IsPremium).Take(5).Count();

        StatusText.Text = _isPremiumMode
            ? $"Полная версия. Всего рецептов: {totalRecipes}"
            : $"Демо-режим. Показано: {demoRecipes} из {totalRecipes} рецептов";
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Реализация поиска по мере ввода
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        var keyword = SearchTextBox.Text;

        if (string.IsNullOrWhiteSpace(keyword))
        {
            LoadData();
        }
        else
        {
            var recipes = _recipeRepository.Search(keyword);

            if (!_isPremiumMode)
            {
                recipes = recipes.Where(r => !r.IsPremium).Take(5);
            }

            RecipesDataGrid.ItemsSource = recipes;
            RecipeCountText.Text = recipes.Count().ToString();
        }
    }

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        FilterRecipes();
    }

    private void DifficultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        FilterRecipes();
    }

    private void FilterRecipes()
    {
        var selectedCategory = CategoryComboBox.SelectedItem?.ToString();
        var selectedDifficulty = DifficultyComboBox.SelectedItem?.ToString();

        var recipes = _recipeRepository.GetAll();

        if (!_isPremiumMode)
        {
            recipes = recipes.Where(r => !r.IsPremium);
        }

        if (selectedCategory != null && selectedCategory != "Все категории")
        {
            var category = _categoryRepository.GetByName(selectedCategory);
            if (category != null)
            {
                recipes = recipes.Where(r => r.CategoryId == category.Id);
            }
        }

        if (selectedDifficulty != null && selectedDifficulty != "Все уровни сложности")
        {
            recipes = recipes.Where(r => r.DifficultyLevel == selectedDifficulty);
        }

        if (!_isPremiumMode)
        {
            recipes = recipes.Take(5);
        }

        RecipesDataGrid.ItemsSource = recipes;
        RecipeCountText.Text = recipes.Count().ToString();
    }

    private void RecipesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedRecipe = RecipesDataGrid.SelectedItem as Recipe;

        bool isEnabled = _selectedRecipe != null;
        EditButton.IsEnabled = isEnabled;
        ViewButton.IsEnabled = isEnabled;
        FavoriteButton.IsEnabled = isEnabled;
        DeleteButton.IsEnabled = isEnabled;

        if (_selectedRecipe != null)
        {
            FavoriteButton.Content = _selectedRecipe.IsFavorite ? "★ Убрать из избранного" : "☆ В избранное";
        }
    }

    private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_isPremiumMode)
        {
            var result = MessageBox.Show(
                "В демо-режиме можно добавлять только ограниченное количество рецептов.\nХотите приобрести полную версию?",
                "Демо-режим",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                ShowLicenseWindow();
                return;
            }
        }

        var editWindow = new RecipeEditWindow(_recipeRepository, _categoryRepository,
            _ingredientRepository, _isPremiumMode, null);

        if (editWindow.ShowDialog() == true)
        {
            LoadData();
            UpdateStatus();
        }
    }

    private void EditRecipeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRecipe == null) return;

        if (_selectedRecipe.IsPremium && !_isPremiumMode)
        {
            MessageBox.Show("Этот рецепт доступен только в полной версии.", "Демо-режим",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var editWindow = new RecipeEditWindow(_recipeRepository, _categoryRepository,
            _ingredientRepository, _isPremiumMode, _selectedRecipe);

        if (editWindow.ShowDialog() == true)
        {
            LoadData();
            UpdateStatus();
        }
    }

    private void ViewRecipeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRecipe == null) return;

        if (_selectedRecipe.IsPremium && !_isPremiumMode)
        {
            var result = MessageBox.Show(
                "Этот рецепт доступен только в полной версии.\nХотите приобрести полную версию?",
                "Демо-режим",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                ShowLicenseWindow();
            }
            return;
        }

        var viewWindow = new RecipeViewWindow(_selectedRecipe, _isPremiumMode);
        viewWindow.ShowDialog();
    }

    private void ToggleFavoriteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRecipe == null) return;

        _selectedRecipe.IsFavorite = !_selectedRecipe.IsFavorite;
        _recipeRepository.Update(_selectedRecipe);

        LoadData();
        RecipesDataGrid.SelectedItem = _selectedRecipe;
    }

    private void DeleteRecipeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRecipe == null) return;

        var result = MessageBox.Show(
            $"Вы уверены, что хотите удалить рецепт \"{_selectedRecipe.Title}\"?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            _recipeRepository.Delete(_selectedRecipe.Id);
            LoadData();
            UpdateStatus();
        }
    }

    private void StatisticsButton_Click(object sender, RoutedEventArgs e)
    {
        var statisticsWindow = new StatisticsWindow(_statisticsService, _isPremiumMode);
        statisticsWindow.ShowDialog();
    }

    private void CategoriesButton_Click(object sender, RoutedEventArgs e)
    {
        var categoriesWindow = new CategoriesWindow(_categoryRepository, _isPremiumMode);

        if (categoriesWindow.ShowDialog() == true)
        {
            InitializeUI(); // Обновляем список категорий
            LoadData();
        }
    }

    private void LicenseButton_Click(object sender, RoutedEventArgs e)
    {
        ShowLicenseWindow();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadData();
        UpdateStatus();
    }

    private void BuyPremiumButton_Click(object sender, RoutedEventArgs e)
    {
        ShowLicenseWindow();
    }

    private void ShowLicenseWindow()
    {
        var licenseWindow = new LicenseWindow(_licenseService, _isPremiumMode);

        if (licenseWindow.ShowDialog() == true)
        {
            // Перезапускаем приложение для применения лицензии
            MessageBox.Show("Лицензия успешно активирована! Приложение будет перезапущено.",
                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            var newWindow = new MainWindow(_recipeRepository, _categoryRepository,
                _ingredientRepository, _licenseService, _statisticsService, true);

            newWindow.Show();
            this.Close();
        }
    }
}

internal class RecipeViewWindow
{
    private Recipe selectedRecipe;
    private bool isPremiumMode;

    public RecipeViewWindow(Recipe selectedRecipe, bool isPremiumMode)
    {
        this.selectedRecipe = selectedRecipe;
        this.isPremiumMode = isPremiumMode;
    }

    internal void ShowDialog()
    {
        throw new NotImplementedException();
    }
}

internal class CategoriesWindow
{
    private ICategoryRepository categoryRepository;
    private bool isPremiumMode;

    public CategoriesWindow(ICategoryRepository categoryRepository, bool isPremiumMode)
    {
        this.categoryRepository = categoryRepository;
        this.isPremiumMode = isPremiumMode;
    }

    internal bool ShowDialog()
    {
        throw new NotImplementedException();
    }
}