using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UI
{
    public partial class CategoriesWindow : Window
    {
        public class CategoryItem
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; } = DateTime.Now;
            public int RecipesCount { get; set; } = 0;
        }

        private ObservableCollection<CategoryItem> _categories = new();
        private CategoryItem? _selectedCategory;

        public CategoriesWindow()
        {
            InitializeComponent();
            LoadSampleCategories();
            categoriesDataGrid.ItemsSource = _categories;
        }

        private void LoadSampleCategories()
        {
            _categories.Add(new CategoryItem
            {
                Name = "Основные блюда",
                Description = "Горячие блюда на обед или ужин",
                CreatedDate = new DateTime(2024, 1, 15),
                RecipesCount = 12
            });

            _categories.Add(new CategoryItem
            {
                Name = "Закуски",
                Description = "Холодные и горячие закуски",
                CreatedDate = new DateTime(2024, 2, 10),
                RecipesCount = 8
            });

            _categories.Add(new CategoryItem
            {
                Name = "Десерты",
                Description = "Сладкие блюда и выпечка",
                CreatedDate = new DateTime(2024, 3, 5),
                RecipesCount = 15
            });

            _categories.Add(new CategoryItem
            {
                Name = "Завтраки",
                Description = "Блюда для утреннего приема пищи",
                CreatedDate = new DateTime(2024, 1, 20),
                RecipesCount = 10
            });

            _categories.Add(new CategoryItem
            {
                Name = "Напитки",
                Description = "Горячие и холодные напитки",
                CreatedDate = new DateTime(2024, 2, 28),
                RecipesCount = 7
            });
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

            var newCategory = new CategoryItem
            {
                Name = name,
                Description = "Новая категория рецептов",
                CreatedDate = DateTime.Now,
                RecipesCount = 0
            };

            _categories.Add(newCategory);
            categoriesDataGrid.SelectedItem = newCategory;
            categoriesDataGrid.ScrollIntoView(newCategory);

            categoryNameTextBox.Text = "Новая категория";
        }

        private void CategoriesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCategory = categoriesDataGrid.SelectedItem as CategoryItem;
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
                _selectedCategory.Description = descriptionTextBox.Text;
                categoriesDataGrid.Items.Refresh();
                MessageBox.Show("Изменения сохранены", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
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
                if (_selectedCategory.RecipesCount > 0)
                {
                    MessageBox.Show("Нельзя удалить категорию, содержащую рецепты", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show($"Удалить категорию '{_selectedCategory.Name}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _categories.Remove(_selectedCategory);
                    _selectedCategory = null;
                    descriptionTextBox.Text = "";
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
