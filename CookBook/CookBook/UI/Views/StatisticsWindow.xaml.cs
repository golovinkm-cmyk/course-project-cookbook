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
    /// Логика взаимодействия для StatisticsWindow.xaml
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow(Services.StatisticsService _statisticsService, bool _isPremiumMode)
        {
            InitializeComponent();
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для применения файла
            MessageBox.Show("Применение файла");
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для сброса файла
            MessageBox.Show("Сброс файла");
        }
    }
}
