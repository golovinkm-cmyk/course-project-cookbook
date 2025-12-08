using Data.Interfaces;
using Domain;
using Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class RecipeEditWindow : Window
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IRecipeIngredientRepository _recipeIngredientRepository;

        private readonly int? _recipeId;
        private Recipe? _recipe;
        private ObservableCollection<Category> _categories = new();
        private ObservableCollection<Ingredient> _allIngredients = new();

        public class IngredientRow
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Quantity { get; set; }
            public string Unit { get; set; } = "г";
            public string Notes { get; set; } = string.Empty;
        }

        private ObservableCollection<IngredientRow> _ingredients = new();

        // УДАЛИТЬ эти строки - они конфликтуют с элементами из XAML
        // private object categoryComboBox;
        // private object difficultyComboBox;
        // private object titleTextBox;
        // private object servingsUpDown;
        // private object ingredientsDataGrid;
        // private object descriptionTextBox;
        // private object instructionsTextBox;
        // private object preparationTimeUpDown;
        // private object cookingTimeUpDown;
        // private object favoriteCheckBox;
        // private object premiumCheckBox;

        public RecipeEditWindow(
            IRecipeRepository recipeRepository,
            ICategoryRepository categoryRepository,
            IIngredientRepository ingredientRepository,
            IRecipeIngredientRepository recipeIngredientRepository,
            int? recipeId = null)
        {
            _recipeRepository = recipeRepository;
            _categoryRepository = categoryRepository;
            _ingredientRepository = ingredientRepository;
            _recipeIngredientRepository = recipeIngredientRepository;
            _recipeId = recipeId;

            InitializeComponent(); // Этот метод должен существовать автоматически
            InitializeData();
        }

        private void InitializeData()
        {
            try
            {
                // Загрузка категорий
                var categories = _categoryRepository.GetAll();
                foreach (var category in categories)
                {
                    _categories.Add(category);
                }
                categoryComboBox.ItemsSource = _categories;
                categoryComboBox.DisplayMemberPath = "Name";
                categoryComboBox.SelectedValuePath = "Id";

                // Загрузка всех ингредиентов
                var allIngredients = _ingredientRepository.GetAll();
                foreach (var ingredient in allIngredients)
                {
                    _allIngredients.Add(ingredient);
                }

                // Настройка ComboBox для сложности
                difficultyComboBox.ItemsSource = new[] { "Легкий", "Средний", "Сложный" };
                difficultyComboBox.SelectedIndex = 1;

                // Если editing existing recipe
                if (_recipeId.HasValue)
                {
                    _recipe = _recipeRepository.GetById(_recipeId.Value);
                    if (_recipe != null)
                    {
                        LoadRecipeData();
                        LoadRecipeIngredients();
                    }
                }
                else
                {
                    // Новый рецепт
                    titleTextBox.Text = "Новый рецепт";
                    if (categoryComboBox.Items.Count > 0)
                        categoryComboBox.SelectedIndex = 0;

                    // Так как у нас нет IntegerUpDown, используем TextBox
                    servingsTextBox.Text = "4";
                }

                ingredientsDataGrid.ItemsSource = _ingredients;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecipeData()
        {
            if (_recipe == null) return;

            try
            {
                titleTextBox.Text = _recipe.Title;
                descriptionTextBox.Text = _recipe.Description ?? "";
                instructionsTextBox.Text = _recipe.Instructions;

                // Вместо IntegerUpDown используем TextBox
                preparationTimeTextBox.Text = _recipe.PreparationTime.ToString();
                cookingTimeTextBox.Text = _recipe.CookingTime.ToString();
                servingsTextBox.Text = _recipe.Servings.ToString();

                // Установка сложности
                if (!string.IsNullOrEmpty(_recipe.DifficultyLevel))
                {
                    for (int i = 0; i < difficultyComboBox.Items.Count; i++)
                    {
                        if (difficultyComboBox.Items[i].ToString() == _recipe.DifficultyLevel)
                        {
                            difficultyComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                }

                // Установка категории
                if (_recipe.CategoryId > 0)
                {
                    categoryComboBox.SelectedValue = _recipe.CategoryId;
                }

                favoriteCheckBox.IsChecked = _recipe.IsFavorite;
                premiumCheckBox.IsChecked = _recipe.IsPremium;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных рецепта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecipeIngredients()
        {
            if (_recipeId == null) return;

            try
            {
                _ingredients.Clear();
                var recipeIngredients = _recipeIngredientRepository.GetByRecipeId(_recipeId.Value);

                foreach (var recipeIngredient in recipeIngredients)
                {
                    var ingredient = _ingredientRepository.GetById(recipeIngredient.IngredientId);
                    if (ingredient != null)
                    {
                        _ingredients.Add(new IngredientRow
                        {
                            Id = ingredient.Id,
                            Name = ingredient.Name,
                            Quantity = recipeIngredient.Quantity,
                            Unit = recipeIngredient.Unit ?? ingredient.Unit,
                            Notes = recipeIngredient.Notes ?? ""
                        });
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
                // Открываем диалог для выбора ингредиента
                var ingredientDialog = new Window
                {
                    Title = "Выбор ингредиента",
                    Width = 400,
                    Height = 300,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this
                };

                var listBox = new ListBox
                {
                    ItemsSource = _allIngredients,
                    DisplayMemberPath = "Name",
                    Margin = new Thickness(10)
                };

                var buttonPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(10)
                };

                var okButton = new Button { Content = "OK", Width = 80, Height = 30 };
                var cancelButton = new Button { Content = "Отмена", Width = 80, Height = 30, Margin = new Thickness(10, 0, 0, 0) };

                okButton.Click += (s, args) =>
                {
                    if (listBox.SelectedItem is Ingredient selectedIngredient)
                    {
                        _ingredients.Add(new IngredientRow
                        {
                            Id = selectedIngredient.Id,
                            Name = selectedIngredient.Name,
                            Quantity = 1,
                            Unit = selectedIngredient.Unit ?? "г",
                            Notes = ""
                        });
                    }
                    ingredientDialog.Close();
                };

                cancelButton.Click += (s, args) => ingredientDialog.Close();

                buttonPanel.Children.Add(okButton);
                buttonPanel.Children.Add(cancelButton);

                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                Grid.SetRow(listBox, 0);
                Grid.SetRow(buttonPanel, 1);

                grid.Children.Add(listBox);
                grid.Children.Add(buttonPanel);

                ingredientDialog.Content = grid;
                ingredientDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении ингредиента: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ingredientsDataGrid.SelectedItem != null)
                {
                    _ingredients.Remove((IngredientRow)ingredientsDataGrid.SelectedItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении ингредиента: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(titleTextBox.Text))
                {
                    MessageBox.Show("Введите название рецепта", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(instructionsTextBox.Text))
                {
                    MessageBox.Show("Введите инструкцию приготовления", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (categoryComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите категорию", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Парсим числовые значения
                if (!int.TryParse(preparationTimeTextBox.Text, out int prepTime))
                {
                    MessageBox.Show("Введите корректное время подготовки", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(cookingTimeTextBox.Text, out int cookTime))
                {
                    MessageBox.Show("Введите корректное время готовки", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(servingsTextBox.Text, out int servings))
                {
                    MessageBox.Show("Введите корректное количество порций", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Сохранение рецепта
                if (_recipe == null)
                {
                    _recipe = new Recipe
                    {
                        Title = titleTextBox.Text,
                        Description = descriptionTextBox.Text,
                        Instructions = instructionsTextBox.Text,
                        PreparationTime = prepTime,
                        CookingTime = cookTime,
                        Servings = servings,
                        DifficultyLevel = difficultyComboBox.SelectedItem?.ToString() ?? "Средний",
                        CategoryId = (int)categoryComboBox.SelectedValue,
                        IsFavorite = favoriteCheckBox.IsChecked == true,
                        IsPremium = premiumCheckBox.IsChecked == true
                    };

                    _recipe.Id = _recipeRepository.Add(_recipe);
                }
                else
                {
                    _recipe.Title = titleTextBox.Text;
                    _recipe.Description = descriptionTextBox.Text;
                    _recipe.Instructions = instructionsTextBox.Text;
                    _recipe.PreparationTime = prepTime;
                    _recipe.CookingTime = cookTime;
                    _recipe.Servings = servings;
                    _recipe.DifficultyLevel = difficultyComboBox.SelectedItem?.ToString() ?? "Средний";
                    _recipe.CategoryId = (int)categoryComboBox.SelectedValue;
                    _recipe.IsFavorite = favoriteCheckBox.IsChecked == true;
                    _recipe.IsPremium = premiumCheckBox.IsChecked == true;
                    _recipe.ModifiedDate = DateTime.Now;

                    _recipeRepository.Update(_recipe);
                }

                // Сохранение ингредиентов
                if (_recipe.Id > 0)
                {
                    // Удаляем старые ингредиенты
                    _recipeIngredientRepository.DeleteByRecipeId(_recipe.Id);

                    // Добавляем новые ингредиенты
                    foreach (var ingredientRow in _ingredients)
                    {
                        var recipeIngredient = new RecipeIngredient
                        {
                            RecipeId = _recipe.Id,
                            IngredientId = ingredientRow.Id,
                            Quantity = ingredientRow.Quantity,
                            Unit = ingredientRow.Unit,
                            Notes = ingredientRow.Notes
                        };
                        _recipeIngredientRepository.Add(recipeIngredient);
                    }
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Обработчики для кнопок порций (если они есть в XAML)
        private void ServingsUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(servingsTextBox.Text, out int servings))
            {
                servings = Math.Min(20, servings + 1);
                servingsTextBox.Text = servings.ToString();
            }
            else
            {
                servingsTextBox.Text = "4";
            }
        }

        private void ServingsDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(servingsTextBox.Text, out int servings))
            {
                servings = Math.Max(1, servings - 1);
                servingsTextBox.Text = servings.ToString();
            }
            else
            {
                servingsTextBox.Text = "4";
            }
        }
    }
}