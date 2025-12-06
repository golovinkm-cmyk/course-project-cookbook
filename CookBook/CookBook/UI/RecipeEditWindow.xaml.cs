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
    public partial class RecipeEditWindow : Window
    {
        public class IngredientRow
        {
            public string Name { get; set; } = string.Empty;
            public decimal Quantity { get; set; }
            public string Unit { get; set; } = "г";
            public string Notes { get; set; } = string.Empty;
        }

        private ObservableCollection<IngredientRow> _ingredients = new();

        public RecipeEditWindow()
        {
            InitializeComponent();
            ingredientsDataGrid.ItemsSource = _ingredients;

            // Добавляем тестовые данные
            _ingredients.Add(new IngredientRow { Name = "Картофель", Quantity = 500, Unit = "г" });
            _ingredients.Add(new IngredientRow { Name = "Лук", Quantity = 1, Unit = "шт", Notes = "Красный" });
            _ingredients.Add(new IngredientRow { Name = "Масло растительное", Quantity = 2, Unit = "ст.л." });
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            _ingredients.Add(new IngredientRow());
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

            // Здесь будет логика сохранения рецепта
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
