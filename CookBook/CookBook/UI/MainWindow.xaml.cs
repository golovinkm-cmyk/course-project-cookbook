using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
    public partial class MainWindow : Window
    {
        // Классы для временного хранения данных
        public class RecipeItem
        {
            public string Title { get; set; } = string.Empty;
            public string CategoryName { get; set; } = "Без категории";
            public int TotalTime { get; set; } = 0;
            public int Servings { get; set; } = 1;
            public string DifficultyLevel { get; set; } = "Средний";
            public bool IsFavorite { get; set; }
            public bool IsPremium { get; set; }
        }

        public class CategoryItem
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string CreatedDate { get; set; } = DateTime.Now.ToString("dd.MM.yyyy");
            public int RecipesCount { get; set; } = 0;
        }

        private ObservableCollection<RecipeItem> _recipes = new();
        private ObservableCollection<CategoryItem> _categories = new();

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация данных
            LoadSampleData();

            recipesDataGrid.ItemsSource = _recipes;
            categoriesDataGrid.ItemsSource = _categories;
        }

        private void LoadSampleData()
        {
            // Тестовые рецепты
            _recipes.Add(new RecipeItem
            {
                Title = "Картофель по-деревенски",
                CategoryName = "Основные блюда",
                TotalTime = 45,
                Servings = 4,
                DifficultyLevel = "Легкий",
                IsFavorite = true,
                IsPremium = false
            });

            _recipes.Add(new RecipeItem
            {
                Title = "Салат Цезарь",
                CategoryName = "Закуски",
                TotalTime = 30,
                Servings = 2,
                DifficultyLevel = "Средний",
                IsFavorite = false,
                IsPremium = true
            });

            _recipes.Add(new RecipeItem
            {
                Title = "Шоколадный торт",
                CategoryName = "Десерты",
                TotalTime = 120,
                Servings = 8,
                DifficultyLevel = "Сложный",
                IsFavorite = true,
                IsPremium = true
            });

            // Тестовые категории
            _categories.Add(new CategoryItem
            {
                Name = "Основные блюда",
                Description = "Горячие блюда на обед или ужин",
                RecipesCount = 12
            });

            _categories.Add(new CategoryItem
            {
                Name = "Закуски",
                Description = "Холодные и горячие закуски",
                RecipesCount = 8
            });

            _categories.Add(new CategoryItem
            {
                Name = "Десерты",
                Description = "Сладкие блюда и выпечка",
                RecipesCount = 15
            });
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Фильтрация рецептов по поисковому запросу
            string searchText = searchTextBox.Text.ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                recipesDataGrid.ItemsSource = _recipes;
            }
            else
            {
                var filtered = new ObservableCollection<RecipeItem>(
                    _recipes.Where(r => r.Title.ToLower().Contains(searchText))
                );
                recipesDataGrid.ItemsSource = filtered;
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Фильтрация рецептов по категории
            if (categoryComboBox.SelectedItem != null)
            {
                string selectedCategory = categoryComboBox.SelectedItem.ToString();
                var filtered = new ObservableCollection<RecipeItem>(
                    _recipes.Where(r => r.CategoryName == selectedCategory)
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
            var editWindow = new RecipeEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                // Здесь будет добавление нового рецепта
                MessageBox.Show("Рецепт добавлен", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (recipesDataGrid.SelectedItem != null)
            {
                var editWindow = new RecipeEditWindow();
                editWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите рецепт для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ViewRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (recipesDataGrid.SelectedItem != null)
            {
                var viewWindow = new RecipeViewWindow();
                viewWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите рецепт для просмотра", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (recipesDataGrid.SelectedItem != null)
            {
                var result = MessageBox.Show("Удалить выбранный рецепт?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _recipes.Remove((RecipeItem)recipesDataGrid.SelectedItem);
                    MessageBox.Show("Рецепт удален", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ToggleFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (recipesDataGrid.SelectedItem is RecipeItem selectedRecipe)
            {
                selectedRecipe.IsFavorite = !selectedRecipe.IsFavorite;
                recipesDataGrid.Items.Refresh();
            }
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            // Для статистики нужно будет создать отдельное окно
            MessageBox.Show("Форма статистики будет добавлена в следующей лабораторной работе",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Для категорий нужно будет создать отдельное окно
            MessageBox.Show("Форма управления категориями будет добавлена позже",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (categoriesDataGrid.SelectedItem != null)
            {
                // Редактирование категории
                MessageBox.Show("Редактирование категории",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (categoriesDataGrid.SelectedItem != null)
            {
                var result = MessageBox.Show("Удалить выбранную категорию?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _categories.Remove((CategoryItem)categoriesDataGrid.SelectedItem);
                }
            }
        }

        private void BuyLicenseButton_Click(object sender, RoutedEventArgs e)
        {
            // Для покупки лицензии нужно будет создать отдельное окно
            MessageBox.Show("Форма покупки лицензии будет добавлена позже",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}