using Data.Interfaces;
using Domain.Entities;
using Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UI.Views;

public partial class RecipeEditWindow : Window
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IRecipeIngredientRepository _recipeIngredientRepository;
    private readonly bool _isPremiumMode;
    private readonly Recipe? _existingRecipe;

    private readonly ObservableCollection<Category> _categories;
    private readonly ObservableCollection<Ingredient> _availableIngredients;
    private readonly ObservableCollection<RecipeIngredientViewModel> _recipeIngredients;

    public RecipeEditWindow(IRecipeRepository recipeRepository,
                           ICategoryRepository categoryRepository,
                           IIngredientRepository ingredientRepository,
                           IRecipeIngredientRepository recipeIngredientRepository,
                           bool isPremiumMode,
                           Recipe? existingRecipe = null)
    {
        try
        {
            _recipeRepository = recipeRepository;
            _categoryRepository = categoryRepository;
            _ingredientRepository = ingredientRepository;
            _recipeIngredientRepository = recipeIngredientRepository;
            _isPremiumMode = isPremiumMode;
            _existingRecipe = existingRecipe;

            // Загружаем данные
            _categories = new ObservableCollection<Category>(_categoryRepository.GetAll());
            _availableIngredients = new ObservableCollection<Ingredient>(_ingredientRepository.GetAll());
            _recipeIngredients = new ObservableCollection<RecipeIngredientViewModel>();

            InitializeComponent();
            InitializeData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при создании окна: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    private void InitializeData()
    {
        try
        {
            // ВАЖНО: Сначала очищаем ItemsSource, потом устанавливаем
            if (CategoryComboBox != null)
            {
                CategoryComboBox.ItemsSource = null; // Очищаем сначала
                CategoryComboBox.ItemsSource = _categories;
                CategoryComboBox.DisplayMemberPath = "Name";
                CategoryComboBox.SelectedValuePath = "Id";
            }

            // ВАЖНО: Для DifficultyComboBox тоже очищаем и устанавливаем простые строки
            if (DifficultyComboBox != null)
            {
                DifficultyComboBox.ItemsSource = null; // Очищаем сначала
                DifficultyComboBox.ItemsSource = new[] { "Легкий", "Средний", "Сложный" };
            }

            // ВАЖНО: Для NewIngredientComboBox очищаем и устанавливаем
            if (NewIngredientComboBox != null)
            {
                NewIngredientComboBox.ItemsSource = null; // Очищаем сначала
                NewIngredientComboBox.ItemsSource = _availableIngredients;
                NewIngredientComboBox.DisplayMemberPath = "Name";
                if (_availableIngredients.Any())
                {
                    NewIngredientComboBox.SelectedIndex = 0;
                }
            }

            // Настраиваем DataGrid для ингредиентов
            if (IngredientsDataGrid != null)
            {
                // ВАЖНО: Сначала очищаем ItemsSource
                IngredientsDataGrid.ItemsSource = null;
                IngredientsDataGrid.ItemsSource = _recipeIngredients;
                IngredientsDataGrid.CanUserAddRows = false;
                IngredientsDataGrid.CanUserDeleteRows = false;
                IngredientsDataGrid.AutoGenerateColumns = false;

                // Настраиваем колонки
                ConfigureIngredientsDataGridColumns();
            }

            if (_existingRecipe != null)
            {
                // Режим редактирования
                Title = $"Редактирование: {_existingRecipe.Title}";

                if (TitleTextBox != null)
                    TitleTextBox.Text = _existingRecipe.Title;

                if (CategoryComboBox != null)
                {
                    var category = _categories.FirstOrDefault(c => c.Id == _existingRecipe.CategoryId);
                    CategoryComboBox.SelectedItem = category;
                }

                if (DifficultyComboBox != null)
                {
                    // Устанавливаем выбранную сложность
                    DifficultyComboBox.SelectedItem = _existingRecipe.DifficultyLevel;
                }

                if (PrepTimeTextBox != null)
                    PrepTimeTextBox.Text = _existingRecipe.PreparationTime.ToString();

                if (CookTimeTextBox != null)
                    CookTimeTextBox.Text = _existingRecipe.CookingTime.ToString();

                if (ServingsTextBox != null)
                    ServingsTextBox.Text = _existingRecipe.Servings.ToString();

                if (DescriptionTextBox != null)
                    DescriptionTextBox.Text = _existingRecipe.Description ?? "";

                if (InstructionsTextBox != null)
                    InstructionsTextBox.Text = _existingRecipe.Instructions;

                if (IsFavoriteCheckBox != null)
                    IsFavoriteCheckBox.IsChecked = _existingRecipe.IsFavorite;

                if (PremiumCheckBox != null)
                {
                    PremiumCheckBox.IsChecked = _existingRecipe.IsPremium;
                    PremiumCheckBox.IsEnabled = _isPremiumMode;
                }

                // Загружаем ингредиенты рецепта
                LoadRecipeIngredients();
            }
            else
            {
                // Режим создания
                Title = "Добавление нового рецепта";

                if (DifficultyComboBox != null)
                    DifficultyComboBox.SelectedIndex = 0;

                if (PremiumCheckBox != null)
                    PremiumCheckBox.IsEnabled = _isPremiumMode;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при инициализации формы: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ConfigureIngredientsDataGridColumns()
    {
        if (IngredientsDataGrid == null) return;

        // Очищаем существующие колонки
        IngredientsDataGrid.Columns.Clear();

        // Колонка с названием ингредиента
        var ingredientNameColumn = new DataGridTextColumn
        {
            Header = "Ингредиент",
            Width = 200,
            IsReadOnly = true,
            Binding = new Binding("IngredientName")
        };

        // Колонка для количества
        var quantityColumn = new DataGridTextColumn
        {
            Header = "Количество",
            Width = 100,
            IsReadOnly = true,
            Binding = new Binding("Quantity")
            {
                StringFormat = "N2"
            }
        };

        // Колонка для единиц измерения
        var unitColumn = new DataGridTextColumn
        {
            Header = "Единица",
            Width = 80,
            IsReadOnly = true,
            Binding = new Binding("Unit")
        };

        // Колонка для примечаний
        var notesColumn = new DataGridTextColumn
        {
            Header = "Примечания",
            Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            IsReadOnly = true,
            Binding = new Binding("Notes")
        };

        // Колонка для удаления
        var deleteColumn = new DataGridTemplateColumn
        {
            Header = "Действия",
            Width = 80
        };

        var deleteButtonFactory = new FrameworkElementFactory(typeof(Button));
        deleteButtonFactory.SetValue(Button.ContentProperty, "Удалить");
        deleteButtonFactory.SetValue(Button.WidthProperty, 70.0);
        deleteButtonFactory.SetValue(Button.HeightProperty, 25.0);
        deleteButtonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(DeleteIngredientButton_Click));

        var cellTemplate = new DataTemplate();
        cellTemplate.VisualTree = deleteButtonFactory;
        deleteColumn.CellTemplate = cellTemplate;

        // Добавляем колонки
        IngredientsDataGrid.Columns.Add(ingredientNameColumn);
        IngredientsDataGrid.Columns.Add(quantityColumn);
        IngredientsDataGrid.Columns.Add(unitColumn);
        IngredientsDataGrid.Columns.Add(notesColumn);
        IngredientsDataGrid.Columns.Add(deleteColumn);
    }

    private void DeleteIngredientButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is RecipeIngredientViewModel ingredient)
        {
            _recipeIngredients.Remove(ingredient);
        }
    }

    private void LoadRecipeIngredients()
    {
        try
        {
            _recipeIngredients.Clear();

            if (_existingRecipe != null)
            {
                // Загружаем ингредиенты рецепта из базы данных
                var recipeIngredients = _recipeIngredientRepository.GetByRecipeId(_existingRecipe.Id);

                foreach (var recipeIngredient in recipeIngredients)
                {
                    // Находим ингредиент по ID
                    var ingredient = _availableIngredients.FirstOrDefault(i => i.Id == recipeIngredient.IngredientId);

                    if (ingredient != null)
                    {
                        _recipeIngredients.Add(new RecipeIngredientViewModel
                        {
                            IngredientId = ingredient.Id,
                            IngredientName = ingredient.Name,
                            Quantity = recipeIngredient.Quantity,
                            Unit = recipeIngredient.Unit,
                            Notes = recipeIngredient.Notes
                        });
                    }
                    else
                    {
                        // Если ингредиент не найден, используем данные из RecipeIngredient
                        _recipeIngredients.Add(new RecipeIngredientViewModel
                        {
                            IngredientId = recipeIngredient.IngredientId,
                            IngredientName = $"Ингредиент ID: {recipeIngredient.IngredientId}",
                            Quantity = recipeIngredient.Quantity,
                            Unit = recipeIngredient.Unit,
                            Notes = recipeIngredient.Notes
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке ингредиентов: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Получаем название ингредиента
            string ingredientName = string.Empty;
            int ingredientId = 0;

            if (NewIngredientComboBox.SelectedItem is Ingredient selectedIngredient)
            {
                // Если выбрали из списка
                ingredientName = selectedIngredient.Name;
                ingredientId = selectedIngredient.Id;
            }
            else if (!string.IsNullOrWhiteSpace(NewIngredientComboBox.Text))
            {
                // Если ввели текст вручную
                ingredientName = NewIngredientComboBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(ingredientName))
                {
                    MessageBox.Show("Введите название ингредиента", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Проверяем, есть ли такой ингредиент в базе
                var existingIngredient = _availableIngredients.FirstOrDefault(i =>
                    i.Name.Equals(ingredientName, StringComparison.OrdinalIgnoreCase));

                if (existingIngredient == null)
                {
                    // Создаем новый ингредиент
                    var newIngredient = new Ingredient
                    {
                        Name = ingredientName,
                        Unit = NewUnitComboBox.Text
                    };

                    // Сохраняем в базу данных
                    _ingredientRepository.Add(newIngredient);

                    // Обновляем ID
                    ingredientId = newIngredient.Id;

                    // Обновляем список
                    _availableIngredients.Add(newIngredient);
                    NewIngredientComboBox.ItemsSource = null;
                    NewIngredientComboBox.ItemsSource = _availableIngredients;
                    NewIngredientComboBox.SelectedItem = newIngredient;

                    ingredientName = newIngredient.Name;
                }
                else
                {
                    ingredientName = existingIngredient.Name;
                    ingredientId = existingIngredient.Id;
                }
            }
            else
            {
                MessageBox.Show("Выберите или введите название ингредиента", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем количество
            if (!decimal.TryParse(NewQuantityTextBox.Text, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем единицу измерения
            string unit = NewUnitComboBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(unit))
            {
                unit = "г";
            }

            // Получаем примечания
            string notes = NewNotesTextBox.Text.Trim();

            // Добавляем ингредиент в список рецепта
            var recipeIngredient = new RecipeIngredientViewModel
            {
                IngredientId = ingredientId,
                IngredientName = ingredientName,
                Quantity = quantity,
                Unit = unit,
                Notes = notes
            };

            _recipeIngredients.Add(recipeIngredient);

            // Прокручиваем к последней добавленной строке
            if (IngredientsDataGrid != null)
            {
                IngredientsDataGrid.ScrollIntoView(recipeIngredient);
            }

            // Очищаем поля ввода
            NewIngredientComboBox.Text = "";
            NewQuantityTextBox.Text = "1";
            NewUnitComboBox.Text = "г";
            NewNotesTextBox.Text = "";

            // Фокусируемся на поле ввода ингредиента
            NewIngredientComboBox.Focus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при добавлении ингредиента: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(TitleTextBox?.Text))
            {
                MessageBox.Show("Введите название рецепта", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка ингредиентов
            if (!_recipeIngredients.Any())
            {
                MessageBox.Show("Добавьте хотя бы один ингредиент", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(PrepTimeTextBox?.Text, out int prepTime) || prepTime < 0)
            {
                MessageBox.Show("Введите корректное время подготовки", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(CookTimeTextBox?.Text, out int cookTime) || cookTime < 0)
            {
                MessageBox.Show("Введите корректное время приготовления", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(ServingsTextBox?.Text, out int servings) || servings <= 0)
            {
                MessageBox.Show("Введите корректное количество порций", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CategoryComboBox?.SelectedItem is not Category selectedCategory)
            {
                MessageBox.Show("Выберите категорию", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InstructionsTextBox?.Text))
            {
                MessageBox.Show("Введите инструкцию приготовления", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем выбранную сложность
            var difficulty = DifficultyComboBox?.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(difficulty))
            {
                MessageBox.Show("Выберите уровень сложности", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Удаляем префикс System.Windows.Controls.ComboBoxItem: если есть
            if (difficulty.Contains(":"))
            {
                difficulty = difficulty.Split(':').Last().Trim();
            }

            // Создание/обновление рецепта
            Recipe recipe;
            bool isNewRecipe = _existingRecipe == null;

            if (_existingRecipe != null)
            {
                // Редактирование существующего
                recipe = _existingRecipe;
                recipe.Title = TitleTextBox.Text.Trim();
                recipe.Description = DescriptionTextBox?.Text?.Trim() ?? "";
                recipe.Instructions = InstructionsTextBox.Text.Trim();
                recipe.PreparationTime = prepTime;
                recipe.CookingTime = cookTime;
                recipe.Servings = servings;
                recipe.DifficultyLevel = difficulty;
                recipe.CategoryId = selectedCategory.Id;
                recipe.IsFavorite = IsFavoriteCheckBox?.IsChecked ?? false;
                recipe.IsPremium = PremiumCheckBox?.IsChecked ?? false;
                recipe.ModifiedDate = DateTime.Now;

                _recipeRepository.Update(recipe);
            }
            else
            {
                // Создание нового
                recipe = new Recipe
                {
                    Title = TitleTextBox.Text.Trim(),
                    Description = DescriptionTextBox?.Text?.Trim() ?? "",
                    Instructions = InstructionsTextBox.Text.Trim(),
                    PreparationTime = prepTime,
                    CookingTime = cookTime,
                    Servings = servings,
                    DifficultyLevel = difficulty,
                    CategoryId = selectedCategory.Id,
                    IsFavorite = IsFavoriteCheckBox?.IsChecked ?? false,
                    IsPremium = PremiumCheckBox?.IsChecked ?? false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                _recipeRepository.Add(recipe);
            }

            // Сохраняем ингредиенты рецепта в базу данных
            SaveRecipeIngredients(recipe.Id);

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении рецепта: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SaveRecipeIngredients(int recipeId)
    {
        try
        {
            if (_existingRecipe != null)
            {
                // Удаляем старые ингредиенты рецепта
                _recipeIngredientRepository.DeleteByRecipeId(recipeId);
            }

            // Добавляем новые ингредиенты
            foreach (var viewModel in _recipeIngredients)
            {
                var recipeIngredient = new RecipeIngredient
                {
                    RecipeId = recipeId,
                    IngredientId = viewModel.IngredientId,
                    Quantity = viewModel.Quantity,
                    Unit = viewModel.Unit,
                    Notes = viewModel.Notes
                };

                _recipeIngredientRepository.Add(recipeIngredient);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении ингредиентов: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

public class RecipeIngredientViewModel
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}