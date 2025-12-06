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

        private ObservableCollection<IngredientRow> _ingredients = new();

        public RecipeViewWindow()
        {
            InitializeComponent();

            // Заполняем тестовыми данными
            titleTextBlock.Text = "Картофель по-деревенски";
            categoryTextBlock.Text = "Основные блюда";
            difficultyTextBlock.Text = "Легкий";
            timeTextBlock.Text = "45 мин.";
            servingsTextBlock.Text = "4";
            descriptionTextBlock.Text = "Вкусное и простое блюдо из картофеля с ароматными травами.";
            instructionsTextBox.Text = "1. Картофель помыть и нарезать дольками.\n2. Смешать с маслом и специями.\n3. Запекать в духовке 30-40 минут при 200°C.";

            // Тестовые ингредиенты
            _ingredients.Add(new IngredientRow { Name = "Картофель", Quantity = 1, Unit = "кг" });
            _ingredients.Add(new IngredientRow { Name = "Масло оливковое", Quantity = 3, Unit = "ст.л." });
            _ingredients.Add(new IngredientRow { Name = "Чеснок", Quantity = 3, Unit = "зуб." });
            _ingredients.Add(new IngredientRow { Name = "Розмарин", Quantity = 1, Unit = "ч.л.", Notes = "сушеный" });
            _ingredients.Add(new IngredientRow { Name = "Соль", Quantity = 1, Unit = "ч.л." });

            ingredientsDataGrid.ItemsSource = _ingredients;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new RecipeEditWindow();
            editWindow.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var licenseWindow = new LicenseWindow();
            licenseWindow.ShowDialog();
        }
    }
}
