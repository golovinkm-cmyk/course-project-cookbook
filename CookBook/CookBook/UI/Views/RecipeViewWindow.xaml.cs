using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace UI.Views;

public partial class RecipeViewWindow : Window
{
    public RecipeViewWindow(Domain.Entities.Recipe recipe,
                          IEnumerable<Domain.Entities.RecipeIngredient> recipeIngredients,
                          bool isPremiumMode)
    {
        InitializeComponent();

        // Создаем ViewModel для отображения
        var ingredients = new ObservableCollection<IngredientViewModel>();

        if (recipeIngredients != null)
        {
            foreach (var recipeIngredient in recipeIngredients)
            {
                ingredients.Add(new IngredientViewModel
                {
                    Name = recipeIngredient.Ingredient?.Name ?? "Неизвестный ингредиент",
                    Quantity = recipeIngredient.Quantity,
                    Unit = recipeIngredient.Unit,
                    Notes = recipeIngredient.Notes
                });
            }
        }

        this.DataContext = new RecipeViewModel(recipe, ingredients, isPremiumMode);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}

public class RecipeViewModel
{
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public int TotalTime { get; set; }
    public int Servings { get; set; }
    public int PreparationTime { get; set; }
    public int CookingTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public ObservableCollection<IngredientViewModel> Ingredients { get; set; } = new ObservableCollection<IngredientViewModel>();

    public RecipeViewModel(Domain.Entities.Recipe recipe,
                          ObservableCollection<IngredientViewModel> ingredients,
                          bool isPremiumMode)
    {
        Title = recipe.Title;
        CategoryName = recipe.Category?.Name ?? "Без категории";
        DifficultyLevel = recipe.DifficultyLevel;
        TotalTime = recipe.TotalTime;
        Servings = recipe.Servings;
        PreparationTime = recipe.PreparationTime;
        CookingTime = recipe.CookingTime;
        Description = recipe.Description ?? "";
        Instructions = recipe.Instructions;
        
        // Добавляем элементы, если передана коллекция
        if (ingredients != null)
        {
            foreach (var ingredient in ingredients)
            {
                Ingredients.Add(ingredient);
            }
        }
    }
}

public class IngredientViewModel
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}