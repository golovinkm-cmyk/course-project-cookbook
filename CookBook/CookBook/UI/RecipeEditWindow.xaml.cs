
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

        public RecipeEditWindow(
            IRecipeRepository recipeRepository,
            ICategoryRepository categoryRepository,
            IIngredientRepository ingredientRepository,
            IRecipeIngredientRepository recipeIngredientRepository,
            int? recipeId = null)
        {
            InitializeComponent();

            _recipeRepository = recipeRepository;
            _categoryRepository = categoryRepository;
            _ingredientRepository = ingredientRepository;
            _recipeIngredientRepository = recipeIngredientRepository;
            _recipeId = recipeId;

            InitializeData();
        }

        private void InitializeData()
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
                categoryComboBox.SelectedIndex = 0;
                servingsUpDown.Value = 4;
            }

            ingredientsDataGrid.ItemsSource = _ingredients;
        }

        private void LoadRecipeData()
        {
            if (_recipe == null) return;

            titleTextBox.Text = _recipe.Title;
            descriptionTextBox.Text = _recipe.Description ?? "";
            instructionsTextBox.Text = _recipe.Instructions;
            preparationTimeUpDown.Value = _recipe.PreparationTime;
            cookingTimeUpDown.Value = _recipe.CookingTime;
            servingsUpDown.Value = _recipe.Servings;
            difficultyComboBox.SelectedItem = _recipe.DifficultyLevel;
            categoryComboBox.SelectedValue = _recipe.CategoryId;
            favoriteCheckBox.IsChecked = _recipe.IsFavorite;
            premiumCheckBox.IsChecked = _recipe.IsPremium;
        }

        private void LoadRecipeIngredients()
        {
            if (_recipeId == null) return;

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
                        Unit = ingredient.Unit,
                        Notes = recipeIngredient.Notes ?? ""
                    });
                }
            }
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
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
                        Unit = selectedIngredient.Unit,
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

        private void RemoveIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            if (ingredientsDataGrid.SelectedItem != null)
            {
                _ingredients.Remove((IngredientRow)ingredientsDataGrid.SelectedItem);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

            // Сохранение рецепта
            if (_recipe == null)
            {
                _recipe = new Recipe
                {
                    Title = titleTextBox.Text,
                    Description = descriptionTextBox.Text,
                    Instructions = instructionsTextBox.Text,
                    PreparationTime = (int)preparationTimeUpDown.Value,
                    CookingTime = (int)cookingTimeUpDown.Value,
                    Servings = (int)servingsUpDown.Value,
                    DifficultyLevel = difficultyComboBox.SelectedItem as string ?? "Средний",
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
                _recipe.PreparationTime = (int)preparationTimeUpDown.Value;
                _recipe.CookingTime = (int)cookingTimeUpDown.Value;
                _recipe.Servings = (int)servingsUpDown.Value;
                _recipe.DifficultyLevel = difficultyComboBox.SelectedItem as string ?? "Средний";
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
                        Notes = ingredientRow.Notes
                    };
                    _recipeIngredientRepository.Add(recipeIngredient);
                }
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}