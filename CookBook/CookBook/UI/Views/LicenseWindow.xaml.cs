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
    /// Логика взаимодействия для LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window
    {
        public LicenseWindow(Services.LicenseService _licenseService, bool _isPremiumMode)
        {
            InitializeComponent();
        }

        // Добавьте эти методы в класс LicenseWindow
        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для кнопки покупки лицензии
            MessageBox.Show("Функционал покупки лицензии");
        }

        private void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для активации лицензии
            MessageBox.Show("Функционал активации лицензии");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Закрытие окна
            this.Close();
        }
    }
}
