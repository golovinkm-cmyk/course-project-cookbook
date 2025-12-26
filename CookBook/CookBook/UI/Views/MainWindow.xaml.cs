using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
    private readonly IRecipeIngredientRepository _recipeIngredientRepository;
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

        // Создаем репозиторий для связи рецептов и ингредиентов
        _recipeIngredientRepository = new InMemoryRecipeIngredientRepository();

        InitializeUI();
        LoadData();
        UpdateStatus();

        // Настраиваем DataGrid для корректного отображения данных
        ConfigureDataGrid();
    }

    private void ConfigureDataGrid()
    {
        // Настраиваем колонку сложности для корректного отображения
        var difficultyColumn = new DataGridTextColumn
        {
            Header = "Сложность",
            Binding = new Binding("DifficultyLevel"),
            Width = 100
        };

        // Удаляем старую колонку и добавляем новую
        var columnsToRemove = RecipesDataGrid.Columns.Where(c => c.Header?.ToString() == "Сложность").ToList();
        foreach (var column in columnsToRemove)
        {
            RecipesDataGrid.Columns.Remove(column);
        }

        RecipesDataGrid.Columns.Insert(2, difficultyColumn);

        // Добавляем CellStyle для красивого отображения сложности
        var style = new Style(typeof(TextBlock));
        style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));

        style.Triggers.Add(new DataTrigger
        {
            Binding = new Binding("DifficultyLevel"),
            Value = "Легкий",
            Setters = {
                new Setter(TextBlock.ForegroundProperty, System.Windows.Media.Brushes.Green),
                new Setter(TextBlock.FontWeightProperty, FontWeights.Bold)
            }
        });

        style.Triggers.Add(new DataTrigger
        {
            Binding = new Binding("DifficultyLevel"),
            Value = "Средний",
            Setters = {
                new Setter(TextBlock.ForegroundProperty, System.Windows.Media.Brushes.Orange),
                new Setter(TextBlock.FontWeightProperty, FontWeights.Bold)
            }
        });

        style.Triggers.Add(new DataTrigger
        {
            Binding = new Binding("DifficultyLevel"),
            Value = "Сложный",
            Setters = {
                new Setter(TextBlock.ForegroundProperty, System.Windows.Media.Brushes.Red),
                new Setter(TextBlock.FontWeightProperty, FontWeights.Bold)
            }
        });

        difficultyColumn.ElementStyle = style;
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

        // Заполняем комбобокс сложности
        DifficultyComboBox.Items.Clear();
        DifficultyComboBox.Items.Add("Все уровни сложности");
        DifficultyComboBox.Items.Add("Легкий");
        DifficultyComboBox.Items.Add("Средний");
        DifficultyComboBox.Items.Add("Сложный");
        DifficultyComboBox.SelectedIndex = 0;

        // Обновляем тексты в зависимости от режима
        ModeText.Text = _isPremiumMode ? "Полная версия" : "Демо";
        DemoInfoBorder.Visibility = _isPremiumMode ? Visibility.Collapsed : Visibility.Visible;
    }

    private void LoadData()
    {
        var recipes = _recipeRepository.GetAll().ToList();

        if (!_isPremiumMode)
        {
            // В демо-режиме показываем только 5 рецептов и скрываем премиум
            recipes = recipes.Where(r => !r.IsPremium).Take(5).ToList();
        }

        // Загружаем категории для каждого рецепта
        var allCategories = _categoryRepository.GetAll().ToList();
        foreach (var recipe in recipes)
        {
            recipe.Category = allCategories.FirstOrDefault(c => c.Id == recipe.CategoryId);

            // Убеждаемся, что DifficultyLevel - строка, а не объект ComboBox
            if (recipe.DifficultyLevel != null && recipe.DifficultyLevel.Contains("Controls.ComboBox"))
            {
                recipe.DifficultyLevel = "Средний"; // Значение по умолчанию
            }
        }

        RecipesDataGrid.ItemsSource = recipes;
        RecipeCountText.Text = recipes.Count.ToString();
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
        // Реализация поиска по мере ввода (по мере набора)
        var keyword = SearchTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(keyword))
        {
            LoadData();
        }
        else
        {
            var recipes = _recipeRepository.Search(keyword).ToList();

            if (!_isPremiumMode)
            {
                recipes = recipes.Where(r => !r.IsPremium).Take(5).ToList();
            }

            // Загружаем категории для каждого рецепта
            var allCategories = _categoryRepository.GetAll().ToList();
            foreach (var recipe in recipes)
            {
                recipe.Category = allCategories.FirstOrDefault(c => c.Id == recipe.CategoryId);

                // Убеждаемся, что DifficultyLevel - строка, а не объект ComboBox
                if (recipe.DifficultyLevel != null && recipe.DifficultyLevel.Contains("Controls.ComboBox"))
                {
                    recipe.DifficultyLevel = "Средний"; // Значение по умолчанию
                }
            }

            RecipesDataGrid.ItemsSource = recipes;
            RecipeCountText.Text = recipes.Count.ToString();
        }
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        var keyword = SearchTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            LoadData();
        }
        else
        {
            var recipes = _recipeRepository.Search(keyword).ToList();

            if (!_isPremiumMode)
            {
                recipes = recipes.Where(r => !r.IsPremium).Take(5).ToList();
            }

            // Загружаем категории для каждого рецепта
            var allCategories = _categoryRepository.GetAll().ToList();
            foreach (var recipe in recipes)
            {
                recipe.Category = allCategories.FirstOrDefault(c => c.Id == recipe.CategoryId);

                // Убеждаемся, что DifficultyLevel - строка, а не объект ComboBox
                if (recipe.DifficultyLevel != null && recipe.DifficultyLevel.Contains("Controls.ComboBox"))
                {
                    recipe.DifficultyLevel = "Средний"; // Значение по умолчанию
                }
            }

            RecipesDataGrid.ItemsSource = recipes;
            RecipeCountText.Text = recipes.Count.ToString();
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

        var recipes = _recipeRepository.GetAll().ToList();

        if (!_isPremiumMode)
        {
            recipes = recipes.Where(r => !r.IsPremium).ToList();
        }

        if (selectedCategory != null && selectedCategory != "Все категории")
        {
            var category = _categoryRepository.GetByName(selectedCategory);
            if (category != null)
            {
                recipes = recipes.Where(r => r.CategoryId == category.Id).ToList();
            }
        }

        if (selectedDifficulty != null && selectedDifficulty != "Все уровни сложности")
        {
            recipes = recipes.Where(r => r.DifficultyLevel == selectedDifficulty).ToList();
        }

        if (!_isPremiumMode)
        {
            recipes = recipes.Take(5).ToList();
        }

        // Загружаем категории для каждого рецепта
        var allCategories = _categoryRepository.GetAll().ToList();
        foreach (var recipe in recipes)
        {
            recipe.Category = allCategories.FirstOrDefault(c => c.Id == recipe.CategoryId);

            // Убеждаемся, что DifficultyLevel - строка, а не объект ComboBox
            if (recipe.DifficultyLevel != null && recipe.DifficultyLevel.Contains("Controls.ComboBox"))
            {
                recipe.DifficultyLevel = "Средний"; // Значение по умолчанию
            }
        }

        RecipesDataGrid.ItemsSource = recipes;
        RecipeCountText.Text = recipes.Count.ToString();
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

            // Исправляем DifficultyLevel если он содержит Controls.ComboBox
            if (_selectedRecipe.DifficultyLevel != null && _selectedRecipe.DifficultyLevel.Contains("Controls.ComboBox"))
            {
                _selectedRecipe.DifficultyLevel = "Средний";
                _recipeRepository.Update(_selectedRecipe);
                LoadData();
            }
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
            _ingredientRepository, _recipeIngredientRepository, _isPremiumMode, null);

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
            _ingredientRepository, _recipeIngredientRepository, _isPremiumMode, _selectedRecipe);

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

        // Загружаем ингредиенты для рецепта
        var recipeIngredients = _recipeIngredientRepository.GetByRecipeId(_selectedRecipe.Id);
        var viewWindow = new RecipeViewWindow(_selectedRecipe, recipeIngredients, _isPremiumMode);
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
            // Удаляем связанные ингредиенты
            _recipeIngredientRepository.DeleteByRecipeId(_selectedRecipe.Id);
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
        // Сбрасываем фильтры
        CategoryComboBox.SelectedIndex = 0;
        DifficultyComboBox.SelectedIndex = 0;
        SearchTextBox.Text = "";

        LoadData();
        UpdateStatus();
        StatusText.Text = "Данные обновлены";
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

// Вспомогательный класс для диалога ввода
public class InputDialog : Window
{
    public string ResponseText { get; set; }

    public InputDialog(string title, string prompt)
    {
        this.Title = title;
        this.Width = 300;
        this.Height = 150;
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        var stackPanel = new StackPanel { Margin = new Thickness(10) };

        stackPanel.Children.Add(new TextBlock
        {
            Text = prompt,
            Margin = new Thickness(0, 0, 0, 10)
        });

        var textBox = new TextBox();
        textBox.KeyDown += (s, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ResponseText = textBox.Text;
                DialogResult = true;
                Close();
            }
        };

        stackPanel.Children.Add(textBox);

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 10, 0, 0)
        };

        var okButton = new Button
        {
            Content = "OK",
            Width = 80,
            Margin = new Thickness(0, 0, 10, 0)
        };
        okButton.Click += (s, e) =>
        {
            ResponseText = textBox.Text;
            DialogResult = true;
            Close();
        };

        var cancelButton = new Button
        {
            Content = "Отмена",
            Width = 80
        };
        cancelButton.Click += (s, e) =>
        {
            DialogResult = false;
            Close();
        };

        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);

        stackPanel.Children.Add(buttonPanel);
        this.Content = stackPanel;
    }
}

// Реализуем полноценный CategoriesWindow
public class CategoriesWindow : Window
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly bool _isPremiumMode;
    private readonly System.Collections.ObjectModel.ObservableCollection<Domain.Entities.Category> _categories;

    public CategoriesWindow(ICategoryRepository categoryRepository, bool isPremiumMode)
    {
        _categoryRepository = categoryRepository;
        _isPremiumMode = isPremiumMode;
        _categories = new System.Collections.ObjectModel.ObservableCollection<Domain.Entities.Category>(
            _categoryRepository.GetAll()
        );

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Title = "Управление категориями";
        this.Width = 400;
        this.Height = 300;
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        var mainGrid = new Grid();
        mainGrid.Margin = new Thickness(10);

        // Строка для кнопок
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Панель кнопок добавления/удаления
        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 10)
        };

        var addButton = new Button
        {
            Content = "➕ Добавить",
            Width = 100,
            Margin = new Thickness(0, 0, 10, 0)
        };
        addButton.Click += AddButton_Click;

        var deleteButton = new Button
        {
            Content = "🗑 Удалить",
            Width = 100,
            IsEnabled = false
        };
        deleteButton.Click += DeleteButton_Click;

        buttonPanel.Children.Add(addButton);
        buttonPanel.Children.Add(deleteButton);

        Grid.SetRow(buttonPanel, 0);
        mainGrid.Children.Add(buttonPanel);

        // Список категорий
        var listBox = new ListBox
        {
            ItemsSource = _categories,
            DisplayMemberPath = "Name"
        };
        listBox.SelectionChanged += (s, e) =>
        {
            deleteButton.IsEnabled = listBox.SelectedItem != null;
        };

        Grid.SetRow(listBox, 1);
        mainGrid.Children.Add(listBox);

        // Панель кнопок OK/Отмена
        var okCancelPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 10, 0, 0)
        };

        var okButton = new Button
        {
            Content = "OK",
            Width = 80,
            Margin = new Thickness(0, 0, 10, 0)
        };
        okButton.Click += (s, e) =>
        {
            DialogResult = true;
            Close();
        };

        var cancelButton = new Button
        {
            Content = "Отмена",
            Width = 80
        };
        cancelButton.Click += (s, e) =>
        {
            DialogResult = false;
            Close();
        };

        okCancelPanel.Children.Add(okButton);
        okCancelPanel.Children.Add(cancelButton);

        Grid.SetRow(okCancelPanel, 2);
        mainGrid.Children.Add(okCancelPanel);

        this.Content = mainGrid;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var inputDialog = new InputDialog("Новая категория", "Введите название категории:");
        if (inputDialog.ShowDialog() == true)
        {
            var categoryName = inputDialog.ResponseText;
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                var newCategory = new Domain.Entities.Category
                {
                    Name = categoryName.Trim(),
                    Description = "",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                _categoryRepository.Add(newCategory);
                _categories.Add(newCategory);
            }
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedCategory = (Domain.Entities.Category)((ListBox)sender).SelectedItem;
        if (selectedCategory == null) return;

        var result = MessageBox.Show(
            $"Вы уверены, что хотите удалить категорию \"{selectedCategory.Name}\"?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            _categoryRepository.Delete(selectedCategory.Id);
            _categories.Remove(selectedCategory);
        }
    }
}

// Реализация InMemoryRecipeIngredientRepository для работы со связями рецептов и ингредиентов
public class InMemoryRecipeIngredientRepository : IRecipeIngredientRepository
{
    private readonly List<RecipeIngredient> _recipeIngredients = new();
    private int _nextId = 1;

    public void Add(RecipeIngredient recipeIngredient)
    {
        recipeIngredient.Id = _nextId++;
        _recipeIngredients.Add(recipeIngredient);
    }

    public void AddRange(IEnumerable<RecipeIngredient> recipeIngredients)
    {
        foreach (var ingredient in recipeIngredients)
        {
            ingredient.Id = _nextId++;
            _recipeIngredients.Add(ingredient);
        }
    }

    public void Update(RecipeIngredient recipeIngredient)
    {
        var existing = GetById(recipeIngredient.Id);
        if (existing != null)
        {
            _recipeIngredients.Remove(existing);
            _recipeIngredients.Add(recipeIngredient);
        }
    }

    public void Delete(int id)
    {
        var ingredient = GetById(id);
        if (ingredient != null)
        {
            _recipeIngredients.Remove(ingredient);
        }
    }

    public void DeleteByRecipeId(int recipeId)
    {
        var ingredientsToRemove = _recipeIngredients.Where(ri => ri.RecipeId == recipeId).ToList();
        foreach (var ingredient in ingredientsToRemove)
        {
            _recipeIngredients.Remove(ingredient);
        }
    }

    public RecipeIngredient GetById(int id)
    {
        return _recipeIngredients.FirstOrDefault(ri => ri.Id == id);
    }

    public IEnumerable<RecipeIngredient> GetByRecipeId(int recipeId)
    {
        return _recipeIngredients.Where(ri => ri.RecipeId == recipeId).ToList();
    }

    public IEnumerable<RecipeIngredient> GetAll()
    {
        return _recipeIngredients;
    }
}