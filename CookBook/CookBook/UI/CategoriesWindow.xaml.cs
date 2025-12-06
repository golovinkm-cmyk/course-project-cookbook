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
    public partial class CategoriesWindow : Window
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRecipeRepository _recipeRepository; // Для подсчёта рецептов
        private Category? _selectedCategory;
        private readonly ObservableCollection<Category> _categories = new();

        // Один конструктор с Dependency Injection
        public CategoriesWindow(ICategoryRepository categoryRepository, IRecipeRepository recipeRepository)
        {
            InitializeComponent();

            _categoryRepository = categoryRepository;
            _recipeRepository = recipeRepository;

            LoadCategories();
            categoriesDataGrid.ItemsSource = _categories;
        }

        // Конструктор для редактирования конкретной категории
        public CategoriesWindow(ICategoryRepository categoryRepository, IRecipeRepository recipeRepository, int categoryId)
            : this(categoryRepository, recipeRepository)
        {
            // Найти и выделить категорию для редактирования
            _selectedCategory = _categories.FirstOrDefault(c => c.Id == categoryId);
            if (_selectedCategory != null)
            {
                categoriesDataGrid.SelectedItem = _selectedCategory;
                categoriesDataGrid.ScrollIntoView(_selectedCategory);
                descriptionTextBox.Text = _selectedCategory.Description;
            }
        }

        private void LoadCategories()
        {
            _categories.Clear();

            // Загружаем категории из репозитория
            var categories = _categoryRepository.GetAll();

            foreach (var category in categories)
            {
                // Подсчитываем количество рецептов в категории
                var recipesCount = _recipeRepository.GetByCategory(category.Id).Count;

                // Создаём копию с дополнительным свойством RecipesCount
                // (в реальном проекте лучше использовать ViewModel)
                var categoryWithCount = new Category(category.Name, category.Description)
                {
                    Id = category.Id,
                    CreatedDate = category.CreatedDate
                };

                // Добавляем в коллекцию
                _categories.Add(categoryWithCount);
            }
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            string name = categoryNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название категории", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверяем, нет ли уже категории с таким именем
            if (_categoryRepository.GetByName(name) != null)
            {
                MessageBox.Show($"Категория '{name}' уже существует", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаём новую категорию
            var newCategory = new Category(name, "Новая категория рецептов");

            // Сохраняем в репозиторий
            int newId = _categoryRepository.Add(newCategory);

            // Загружаем обновлённый список
            LoadCategories();

            // Выделяем новую категорию
            _selectedCategory = _categories.FirstOrDefault(c => c.Id == newId);
            if (_selectedCategory != null)
            {
                categoriesDataGrid.SelectedItem = _selectedCategory;
                categoriesDataGrid.ScrollIntoView(_selectedCategory);
            }

            categoryNameTextBox.Text = "Новая категория";
        }

        private void CategoriesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCategory = categoriesDataGrid.SelectedItem as Category;
            if (_selectedCategory != null)
            {
                descriptionTextBox.Text = _selectedCategory.Description;
            }
            else
            {
                descriptionTextBox.Text = "";
            }
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory != null)
            {
                // Обновляем описание
                _selectedCategory.Description = descriptionTextBox.Text;

                // Сохраняем в репозитории
                if (_categoryRepository.Update(_selectedCategory))
                {
                    categoriesDataGrid.Items.Refresh();
                    MessageBox.Show("Изменения сохранены", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении изменений", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите категорию для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory != null)
            {
                // Проверяем, есть ли рецепты в этой категории
                var recipesInCategory = _recipeRepository.GetByCategory(_selectedCategory.Id);

                if (recipesInCategory.Any())
                {
                    MessageBox.Show($"Нельзя удалить категорию '{_selectedCategory.Name}'. В ней есть {recipesInCategory.Count} рецептов.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show($"Удалить категорию '{_selectedCategory.Name}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_categoryRepository.Delete(_selectedCategory.Id))
                    {
                        // Удаляем из коллекции
                        _categories.Remove(_selectedCategory);
                        _selectedCategory = null;
                        descriptionTextBox.Text = "";

                        MessageBox.Show("Категория удалена", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении категории", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите категорию для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}