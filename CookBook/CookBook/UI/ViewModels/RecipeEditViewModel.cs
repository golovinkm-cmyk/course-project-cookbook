using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data.Interfaces;
using Domain.Entities;
using Interfaces;

namespace UI.ViewModels;

public class Recipe : INotifyPropertyChanged
{
    private string _title;
    private string _description;
    private string _instructions;
    private int _preparationTime;
    private int _cookingTime;
    private int _servings;
    private string _difficultyLevel;
    private bool _isFavorite;
    private bool _isPremium;
    private int _categoryId;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged();
            }
        }
    }

    public string Instructions
    {
        get => _instructions;
        set
        {
            if (_instructions != value)
            {
                _instructions = value;
                OnPropertyChanged();
            }
        }
    }

    public int PreparationTime
    {
        get => _preparationTime;
        set
        {
            if (_preparationTime != value)
            {
                _preparationTime = value;
                OnPropertyChanged();
            }
        }
    }

    public int CookingTime
    {
        get => _cookingTime;
        set
        {
            if (_cookingTime != value)
            {
                _cookingTime = value;
                OnPropertyChanged();
            }
        }
    }

    public int Servings
    {
        get => _servings;
        set
        {
            if (_servings != value)
            {
                _servings = value;
                OnPropertyChanged();
            }
        }
    }

    public string DifficultyLevel
    {
        get => _difficultyLevel;
        set
        {
            if (_difficultyLevel != value)
            {
                _difficultyLevel = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsFavorite
    {
        get => _isFavorite;
        set
        {
            if (_isFavorite != value)
            {
                _isFavorite = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsPremium
    {
        get => _isPremium;
        set
        {
            if (_isPremium != value)
            {
                _isPremium = value;
                OnPropertyChanged();
            }
        }
    }

    public int CategoryId
    {
        get => _categoryId;
        set
        {
            if (_categoryId != value)
            {
                _categoryId = value;
                OnPropertyChanged();
            }
        }
    }

    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
