using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data.Interfaces;
using Domain.Entities;
using Interfaces;

namespace UI.ViewModels;

public class RecipeEditViewModel : INotifyPropertyChanged
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly bool _isPremiumMode;

    private Recipe _recipe;
    private string _windowTitle = "Новый рецепт";
    private bool _canSave = false;

    public Recipe Recipe
    {
        get => _recipe;
        set
        {
            _recipe = value;
            OnPropertyChanged();
            Validate();
        }
    }

    public string WindowTitle
    {
        get => _windowTitle;
        set
        {
            _windowTitle = value;
            OnPropertyChanged();
        }
    }

    public bool CanSave
    {
        get => _canSave;
        set
        {
            _canSave = value;
            OnPropertyChanged();
        }
    }

    public bool IsPremiumMode => _isPremiumMode;

    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<Ingredient> AvailableIngredients { get; } = new();
    public ObservableCollection<RecipeIngredient> RecipeIngredients { get; } = new();

    public RecipeEditViewModel(IRecipeRepository recipeRepository,
                              ICategoryRepository categoryRepository,
                              IIngredientRepository ingredientRepository,
                              bool isPremiumMode,
                              Recipe? existingRecipe = null)
    {
        _recipeRepository = recipeRepository;
        _categoryRepository = categoryRepository;
        _ingredientRepository = ingredientRepository;
        _isPremiumMode = isPremiumMode;

        if (existingRecipe != null)
        {
            Recipe = existingRecipe;
            WindowTitle = $"Редактирование: {existingRecipe.Title}";

            // Загружаем ингредиенты рецепта
            // В реальном приложении здесь бы загружались из репозитория
        }
        else
        {
            Recipe = new Recipe
            {
                Title = "",
                Description = "",
                Instructions = "",
                PreparationTime = 0,
                CookingTime = 0,
                Servings = 1,
                DifficultyLevel = "Средний",
                IsFavorite = false,
                IsPremium = false
            };
        }

        LoadCategories();
        LoadIngredients();

        Recipe.PropertyChanged += (s, e) => Validate();
    }

    private void LoadCategories()
    {
        Categories.Clear();
        foreach (var category in _categoryRepository.GetAll())
        {
            Categories.Add(category);
        }
    }

    private void LoadIngredients()
    {
        AvailableIngredients.Clear();
        foreach (var ingredient in _ingredientRepository.GetAll())
        {
            AvailableIngredients.Add(ingredient);
        }
    }

    private void Validate()
    {
        CanSave = !string.IsNullOrWhiteSpace(Recipe.Title) &&
                 Recipe.CategoryId > 0 &&
                 !string.IsNullOrWhiteSpace(Recipe.Instructions) &&
                 Recipe.PreparationTime >= 0 &&
                 Recipe.CookingTime >= 0 &&
                 Recipe.Servings > 0;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}