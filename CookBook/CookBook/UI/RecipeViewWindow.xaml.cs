
using Data.InMemory;
using Data.Interfaces;
using Domain;
using Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using UI;

namespace UI
{
    public partial class RecipeViewWindow : Window
    {
        public class IngredientRow
        {
            public string Name { get; set; } = string.Empty;
            public decimal Quantity { get; set; }
            public string Unit { get; set; } = "г";
            public string Notes { get; set; } = string.Empty;
        }

        private readonly ObservableCollection<IngredientRow> _ingredients = new();
        private readonly Recipe _recipe;
        private readonly IRecipeIngredientRepository _recipeIngredientRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ICategoryRepository _categoryRepository; // Добавили для получения названия категории

        // Конструктор с Dependency Injection
        public RecipeViewWindow(
            Recipe recipe,
            IRecipeIngredientRepository recipeIngredientRepository,
            IIngredientRepository ingredientRepository,
            ICategoryRepository categoryRepository)
        {
            InitializeComponent();

            _recipe = recipe;
            _recipeIngredientRepository = recipeIngredientRepository;
            _ingredientRepository = ingredientRepository;
            _categoryRepository = categoryRepository;

            LoadRecipeData();
            LoadIngredients();
        }

        private void LoadRecipeData()
        {
            if (_recipe == null) return;

            titleTextBlock.Text = _recipe.Title;

            // Получаем название категории из репозитория
            var category = _categoryRepository.GetById(_recipe.CategoryId);
            categoryTextBlock.Text = $"Категория: {category?.Name ?? "Без категории"}";

            difficultyTextBlock.Text = $"Сложность: {_recipe.DifficultyLevel}";
            timeTextBlock.Text = $"Время приготовления: {_recipe.PreparationTime + _recipe.CookingTime} мин.";
            servingsTextBlock.Text = $"Порции: {_recipe.Servings}";

            if (!string.IsNullOrEmpty(_recipe.Description))
            {
                descriptionTextBlock.Text = _recipe.Description;
            }
            else
            {
                descriptionTextBlock.Text = "Описание отсутствует";
            }

            instructionsTextBox.Text = _recipe.Instructions;

            // Показываем кнопку покупки для премиум рецептов
            buyButton.Visibility = _recipe.IsPremium ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadIngredients()
        {
            if (_recipe == null) return;

            _ingredients.Clear();
            var recipeIngredients = _recipeIngredientRepository.GetByRecipeId(_recipe.Id);

            foreach (var recipeIngredient in recipeIngredients)
            {
                var ingredient = _ingredientRepository.GetById(recipeIngredient.IngredientId);
                if (ingredient != null)
                {
                    _ingredients.Add(new IngredientRow
                    {
                        Name = ingredient.Name,
                        Quantity = recipeIngredient.Quantity,
                        Unit = ingredient.Unit,
                        Notes = recipeIngredient.Notes ?? ""
                    });
                }
            }

            ingredientsDataGrid.ItemsSource = _ingredients;
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Для редактирования нужно передать репозитории
            // В этом окне пока просто закрываем
            MessageBox.Show("Для редактирования используйте главное окно", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var licenseWindow = new LicenseWindow(new LicenseRepository());
            licenseWindow.ShowDialog();
        }
    }
}