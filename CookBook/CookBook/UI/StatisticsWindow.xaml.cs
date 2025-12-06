using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace UI
{
    public partial class StatisticsWindow : Window
    {
        // Модели для диаграмм
        public PlotModel PieModel { get; private set; }
        public PlotModel BarModel { get; private set; }
        public PlotModel LineModel { get; private set; }

        // Тестовые данные
        private List<RecipeStat> _recipes = new();

        public class RecipeStat
        {
            public string Title { get; set; } = string.Empty;
            public string Category { get; set; } = "Основные блюда";
            public int TotalTime { get; set; } = 45;
            public string Difficulty { get; set; } = "Средний";
            public bool IsFavorite { get; set; }
            public bool IsPremium { get; set; }
            public DateTime CreatedDate { get; set; } = DateTime.Now;
        }

        public StatisticsWindow()
        {
            InitializeComponent();
            DataContext = this;
            GenerateTestData();
            InitializePlotModels();
            UpdateStatistics();
        }

        private void GenerateTestData()
        {
            var random = new Random();
            var categories = new[] { "Основные блюда", "Закуски", "Десерты", "Завтраки", "Напитки" };
            var difficulties = new[] { "Легкий", "Средний", "Сложный" };

            for (int i = 1; i <= 50; i++)
            {
                _recipes.Add(new RecipeStat
                {
                    Title = $"Рецепт {i}",
                    Category = categories[random.Next(categories.Length)],
                    TotalTime = random.Next(15, 180),
                    Difficulty = difficulties[random.Next(difficulties.Length)],
                    IsFavorite = random.Next(0, 2) == 1,
                    IsPremium = random.Next(0, 3) == 1, // Меньше премиум рецептов
                    CreatedDate = DateTime.Now.AddDays(-random.Next(0, 365))
                });
            }
        }

        private void InitializePlotModels()
        {
            // Круговая диаграмма - распределение по сложности
            PieModel = new PlotModel { Title = "Распределение рецептов по сложности" };
            UpdatePieChart();

            // Столбчатая диаграмма - по категориям
            BarModel = new PlotModel { Title = "Количество рецептов по категориям" };
            UpdateBarChart();

            // Линейный график - по месяцам
            LineModel = new PlotModel { Title = "Динамика добавления рецептов" };
            UpdateLineChart();
        }

        private void UpdatePieChart()
        {
            var difficultyGroups = _recipes
                .GroupBy(r => r.Difficulty)
                .Select(g => new { Difficulty = g.Key, Count = g.Count() })
                .OrderBy(g => g.Difficulty)
                .ToList();

            var pieSeries = new PieSeries
            {
                StrokeThickness = 2,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0
            };

            foreach (var group in difficultyGroups)
            {
                pieSeries.Slices.Add(new PieSlice(group.Difficulty, group.Count));
            }

            PieModel.Series.Clear();
            PieModel.Series.Add(pieSeries);
        }

        private void UpdateBarChart()
        {
            var categoryGroups = _recipes
                .GroupBy(r => r.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            var barSeries = new BarSeries
            {
                Title = "Количество рецептов",
                FillColor = OxyColor.FromRgb(79, 129, 189),
                LabelPlacement = LabelPlacement.Inside,
                LabelFormatString = "{0}"
            };

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Left,
                Title = "Категории"
            };

            foreach (var group in categoryGroups)
            {
                barSeries.Items.Add(new BarItem { Value = group.Count });
                categoryAxis.Labels.Add(group.Category);
            }

            BarModel.Axes.Clear();
            BarModel.Series.Clear();

            BarModel.Axes.Add(categoryAxis);
            BarModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                MinimumPadding = 0,
                MaximumPadding = 0.06,
                AbsoluteMinimum = 0
            });

            BarModel.Series.Add(barSeries);
        }

        private void UpdateLineChart()
        {
            // Группировка по месяцам
            var monthlyData = _recipes
                .GroupBy(r => new { r.CreatedDate.Year, r.CreatedDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count(),
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy")
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            var lineSeries = new LineSeries
            {
                Title = "Рецептов добавлено",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStroke = OxyColors.White,
                MarkerFill = OxyColor.FromRgb(79, 129, 189),
                Color = OxyColor.FromRgb(79, 129, 189)
            };

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Месяц"
            };

            for (int i = 0; i < monthlyData.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i, monthlyData[i].Count));
                categoryAxis.Labels.Add(monthlyData[i].MonthName);
            }

            LineModel.Axes.Clear();
            LineModel.Series.Clear();

            LineModel.Axes.Add(categoryAxis);
            LineModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Количество рецептов",
                Minimum = 0
            });

            LineModel.Series.Add(lineSeries);
        }

        private void UpdateStatistics()
        {
            // Обновление текстовой статистики
            totalRecipesText.Text = _recipes.Count.ToString();
            favoriteRecipesText.Text = _recipes.Count(r => r.IsFavorite).ToString();
            premiumRecipesText.Text = _recipes.Count(r => r.IsPremium).ToString();

            if (_recipes.Any())
            {
                avgCookingTimeText.Text = $"{(int)_recipes.Average(r => r.TotalTime)} мин.";

                var mostPopularDifficulty = _recipes
                    .GroupBy(r => r.Difficulty)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();
                mostPopularDifficultyText.Text = mostPopularDifficulty?.Key ?? "-";

                var mostPopularCategory = _recipes
                    .GroupBy(r => r.Category)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();
                mostPopularCategoryText.Text = mostPopularCategory?.Key ?? "-";
            }
        }

        private void PeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Здесь можно фильтровать данные по периоду
            UpdateAllCharts();
        }

        private void StatTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Здесь можно менять тип отображаемой статистики
            UpdateAllCharts();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateAllCharts();
        }

        private void UpdateAllCharts()
        {
            UpdatePieChart();
            UpdateBarChart();
            UpdateLineChart();
            UpdateStatistics();

            // Обновление моделей для отображения
            PieModel.InvalidatePlot(true);
            BarModel.InvalidatePlot(true);
            LineModel.InvalidatePlot(true);
        }
    }
}
