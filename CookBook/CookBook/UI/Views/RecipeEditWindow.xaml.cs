using System;
using System.Collections.Generic;
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

namespace UI.Views
{
    /// <summary>
    /// Логика взаимодействия для RecipeEditWindow.xaml
    /// </summary>
    public partial class RecipeEditWindow : Window
    {
        public RecipeEditWindow(Data.Interfaces.IRecipeRepository _recipeRepository, Interfaces.ICategoryRepository _categoryRepository, Data.Interfaces.IIngredientRepository _ingredientRepository, bool _isPremiumMode, Domain.Entities.Recipe _selectedRecipe)
        {
            InitializeComponent();
        }

        // Добавьте эти методы в класс RecipeEditWindow
        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для добавления ингредиента
            MessageBox.Show("Добавление ингредиента");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для сохранения рецепта
            MessageBox.Show("Сохранение рецепта");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Закрытие окна без сохранения
            this.Close();
        }
    }
}
