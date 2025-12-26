using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Services;
using Data.Interfaces.Filters;
using Domain.Statistics;

namespace UI.Views;

public partial class StatisticsWindow : Window
{
    private readonly StatisticsService _statisticsService;
    private readonly bool _isPremiumMode;
    private RecipeFilter _currentFilter;

    public StatisticsWindow(StatisticsService statisticsService, bool isPremiumMode)
    {
        InitializeComponent();

        _statisticsService = statisticsService;
        _isPremiumMode = isPremiumMode;
        _currentFilter = new RecipeFilter();

        // Устанавливаем даты по умолчанию
        StartDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);
        EndDatePicker.SelectedDate = DateTime.Now;

        LoadStatistics();
    }

    private void LoadStatistics()
    {
        try
        {
            // Обновляем фильтр с выбранными датами
            _currentFilter = new RecipeFilter
            {
                StartDate = StartDatePicker.SelectedDate,
                EndDate = EndDatePicker.SelectedDate
            };

            // Загружаем статистику
            LoadCategoryStatistics();
            LoadDifficultyStatistics();
            LoadMonthStatistics();
            LoadGeneralStatistics();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadCategoryStatistics()
    {
        try
        {
            var categoryStats = _statisticsService.GetRecipesByCategory(_currentFilter);

            var plotModel = new PlotModel
            {
                Title = "Распределение рецептов по категориям",
                Background = OxyColors.White
            };

            var series = new BarSeries
            {
                Title = "Количество рецептов",
                FillColor = OxyColors.SteelBlue,
                StrokeColor = OxyColors.Black,
                StrokeThickness = 1
            };

            var categories = new List<string>();
            int index = 0;
            foreach (var stat in categoryStats)
            {
                series.Items.Add(new BarItem(stat.RecipeCount, index));
                categories.Add(stat.CategoryName);
                index++;
            }

            plotModel.Series.Add(series);

            // Настраиваем оси
            plotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                ItemsSource = categories,
                Key = "Categories"
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                MinimumPadding = 0,
                MaximumPadding = 0.06,
                AbsoluteMinimum = 0,
                Key = "Value",
                Title = "Количество рецептов"
            });

            CategoryPlotView.Model = plotModel;
        }
        catch (Exception ex)
        {
            CategoryPlotView.Model = CreateErrorPlotModel("Ошибка загрузки статистики по категориям");
        }
    }

    private void LoadDifficultyStatistics()
    {
        try
        {
            var difficultyStats = _statisticsService.GetRecipesByDifficulty(_currentFilter);

            var plotModel = new PlotModel
            {
                Title = "Распределение рецептов по сложности",
                Background = OxyColors.White
            };

            var series = new PieSeries
            {
                StrokeThickness = 2,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0
            };

            // Цвета для разных уровней сложности
            var colors = new Dictionary<string, OxyColor>
            {
                { "Легкий", OxyColors.Green },
                { "Средний", OxyColors.Orange },
                { "Сложный", OxyColors.Red }
            };

            foreach (var stat in difficultyStats)
            {
                var color = colors.ContainsKey(stat.DifficultyLevel)
                    ? colors[stat.DifficultyLevel]
                    : OxyColors.Gray;

                series.Slices.Add(new PieSlice(
                    stat.DifficultyLevel,
                    stat.RecipeCount)
                {
                    Fill = color
                });
            }

            plotModel.Series.Add(series);
            DifficultyPlotView.Model = plotModel;
        }
        catch (Exception ex)
        {
            DifficultyPlotView.Model = CreateErrorPlotModel("Ошибка загрузки статистики по сложности");
        }
    }

    private void LoadMonthStatistics()
    {
        try
        {
            var monthStats = _statisticsService.GetRecipesByMonth(_currentFilter);

            var plotModel = new PlotModel
            {
                Title = "Динамика добавления рецептов по месяцам",
                Background = OxyColors.White
            };

            var lineSeries = new LineSeries
            {
                Title = "Количество рецептов",
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                MarkerStroke = OxyColors.Blue,
                MarkerFill = OxyColors.LightBlue
            };

            var categories = new List<string>();
            int pointIndex = 0;
            foreach (var stat in monthStats.OrderBy(m => m.Year).ThenBy(m => m.Month))
            {
                lineSeries.Points.Add(new DataPoint(pointIndex, stat.RecipeCount));
                categories.Add(stat.GetMonthName());
                pointIndex++;
            }

            plotModel.Series.Add(lineSeries);

            // Настраиваем оси
            if (categories.Any())
            {
                plotModel.Axes.Add(new CategoryAxis
                {
                    Position = AxisPosition.Bottom,
                    ItemsSource = categories,
                    Key = "Months",
                    Angle = 45
                });
            }

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Key = "Value",
                Title = "Количество рецептов"
            });

            MonthPlotView.Model = plotModel;
        }
        catch (Exception ex)
        {
            MonthPlotView.Model = CreateErrorPlotModel("Ошибка загрузки статистики по месяцам");
        }
    }

    private void LoadGeneralStatistics()
    {
        try
        {
            var stats = _statisticsService.GetRecipeStatistics(_currentFilter);

            TotalRecipesText.Text = stats.TotalRecipes.ToString();
            TotalTimeText.Text = $"{stats.TotalCookingTime} минут";
            AverageTimeText.Text = $"{stats.AverageCookingTime} минут";
            FavoriteRecipesText.Text = stats.FavoriteRecipes.ToString();
            PremiumRecipesText.Text = stats.PremiumRecipes.ToString();
        }
        catch (Exception ex)
        {
            TotalRecipesText.Text = "Ошибка";
            TotalTimeText.Text = "Ошибка";
            AverageTimeText.Text = "Ошибка";
            FavoriteRecipesText.Text = "Ошибка";
            PremiumRecipesText.Text = "Ошибка";
        }
    }

    private PlotModel CreateErrorPlotModel(string errorMessage)
    {
        var plotModel = new PlotModel
        {
            Title = errorMessage,
            Background = OxyColors.White
        };

        var textAnnotation = new OxyPlot.Annotations.TextAnnotation
        {
            Text = errorMessage,
            TextPosition = new DataPoint(0.5, 0.5),
            TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Center,
            TextVerticalAlignment = OxyPlot.VerticalAlignment.Middle,
            TextColor = OxyColors.Red
        };

        plotModel.Annotations.Add(textAnnotation);
        return plotModel;
    }

    private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
    {
        if (StartDatePicker.SelectedDate > EndDatePicker.SelectedDate)
        {
            MessageBox.Show("Дата начала не может быть позже даты окончания",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        LoadStatistics();
    }

    private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
    {
        StartDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);
        EndDatePicker.SelectedDate = DateTime.Now;

        LoadStatistics();
    }
}