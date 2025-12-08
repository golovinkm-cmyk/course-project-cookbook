using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class LicenseWindow : Window
    {
        private readonly ILicenseRepository _licenseRepository; // ТОЛЬКО ОДИН РАЗ

        // ОДИН конструктор с Dependency Injection
        public LicenseWindow(ILicenseRepository licenseRepository)
        {
            _licenseRepository = licenseRepository;
            InitializeComponent();
            licenseTypeComboBox.SelectedIndex = 0;
            UpdatePrice();
        }

        private void LicenseTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            if (licenseTypeComboBox.SelectedItem == null) return;

            string licenseType = (licenseTypeComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();

            switch (licenseType)
            {
                case "Пожизненная":
                    priceTextBlock.Text = "999 ₽";
                    break;
                case "Годовая":
                    priceTextBlock.Text = "299 ₽";
                    break;
                case "Месячная":
                    priceTextBlock.Text = "49 ₽";
                    break;
                default:
                    priceTextBlock.Text = "0 ₽";
                    break;
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка введенных данных
            if (!ValidateForm())
                return;

            // Симуляция процесса оплаты
            var result = MessageBox.Show($"Подтвердить покупку {licenseTypeComboBox.Text} лицензии за {priceTextBlock.Text}?",
                "Подтверждение покупки",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Генерация лицензионного ключа
                string licenseKey = GenerateLicenseKey();

                MessageBox.Show($"Покупка успешно завершена!\n\nЛицензионный ключ: {licenseKey}\n\nКлюч отправлен на email: {customerEmailTextBox.Text}",
                    "Успешная покупка",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
        }

        private bool ValidateForm()
        {
            // Проверка имени
            if (string.IsNullOrWhiteSpace(customerNameTextBox.Text))
            {
                MessageBox.Show("Введите имя покупателя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка email
            if (string.IsNullOrWhiteSpace(customerEmailTextBox.Text) ||
                !IsValidEmail(customerEmailTextBox.Text))
            {
                MessageBox.Show("Введите корректный email", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка номера карты
            if (string.IsNullOrWhiteSpace(cardNumberTextBox.Text) ||
                cardNumberTextBox.Text.Length < 16)
            {
                MessageBox.Show("Введите корректный номер карты", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка срока действия
            if (cardExpiryTextBox.Text == "MM/YY" ||
                !Regex.IsMatch(cardExpiryTextBox.Text, @"^\d{2}/\d{2}$"))
            {
                MessageBox.Show("Введите корректный срок действия карты (MM/YY)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка CVV
            if (string.IsNullOrWhiteSpace(cardCvvTextBox.Text) ||
                !Regex.IsMatch(cardCvvTextBox.Text, @"^\d{3}$"))
            {
                MessageBox.Show("Введите корректный CVV код", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка имени владельца карты
            if (string.IsNullOrWhiteSpace(cardHolderTextBox.Text))
            {
                MessageBox.Show("Введите имя владельца карты", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка согласия с условиями
            if (agreementCheckBox.IsChecked != true)
            {
                MessageBox.Show("Необходимо согласиться с условиями лицензионного соглашения", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateLicenseKey()
        {
            var random = new Random();
            return $"{random.Next(1000, 9999):0000}-{random.Next(1000, 9999):0000}-{random.Next(1000, 9999):0000}-{random.Next(1000, 9999):0000}";
        }

        private void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            string licenseKey = licenseKeyTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(licenseKey) || licenseKey == "XXXX-XXXX-XXXX-XXXX")
            {
                activationStatusTextBlock.Text = "Ошибка: Введите лицензионный ключ";
                activationStatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            // Проверка формата ключа
            if (!Regex.IsMatch(licenseKey, @"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$", RegexOptions.IgnoreCase))
            {
                activationStatusTextBlock.Text = "Ошибка: Неверный формат ключа";
                activationStatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            // Симуляция проверки ключа
            if (licenseKey.ToUpper() == "ABCD-1234-EFGH-5678")
            {
                activationStatusTextBlock.Text = "Успешно: Лицензия активирована!";
                activationStatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                licenseInfoTextBlock.Text = "Тип: Пожизненная\nСрок действия: Бессрочно\nДата активации: Сегодня";
            }
            else
            {
                activationStatusTextBlock.Text = "Ошибка: Лицензионный ключ не найден";
                activationStatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
