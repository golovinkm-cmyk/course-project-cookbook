using System;
using System.Text.RegularExpressions;
using System.Windows;
using Services;

namespace UI.Views;

public partial class LicenseWindow : Window
{
    private readonly LicenseService _licenseService;
    private readonly bool _isPremiumMode;

    public LicenseWindow(LicenseService licenseService, bool isPremiumMode)
    {
        InitializeComponent();

        _licenseService = licenseService;
        _isPremiumMode = isPremiumMode;

        InitializeUI();
    }

    private void InitializeUI()
    {
        LicenseTypeComboBox.SelectionChanged += LicenseTypeComboBox_SelectionChanged;
        LicenseTypeComboBox_SelectionChanged(null, null);

        if (_isPremiumMode)
        {
            // Показываем информацию об активной лицензии
            ActivationStatusText.Text = "✅ Лицензия активна";
            ActivationStatusText.Foreground = System.Windows.Media.Brushes.Green;
        }
    }

    private void LicenseTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (LicenseTypeComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item &&
            item.Tag != null)
        {
            PriceTextBlock.Text = $"{item.Tag} руб.";
        }
    }

    private void BuyButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Валидация полей
            if (!ValidatePurchaseFields())
                return;

            // Извлекаем данные
            var selectedItem = LicenseTypeComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem;
            string licenseType = selectedItem?.Content.ToString()?.Split('-')[0].Trim() ?? "Месячная";
            decimal amount = decimal.Parse(selectedItem?.Tag?.ToString() ?? "0");
            string customerName = CustomerNameTextBox.Text.Trim();
            string customerEmail = CustomerEmailTextBox.Text.Trim();

            // Берем последние 4 цифры номера карты
            string cardLastFour = CardNumberTextBox.Text.Replace(" ", "");
            if (cardLastFour.Length >= 4)
                cardLastFour = cardLastFour.Substring(cardLastFour.Length - 4);
            else
                cardLastFour = "0000";

            // Проверка согласия
            if (!AgreementCheckBox.IsChecked ?? false)
            {
                MessageBox.Show("Необходимо согласиться с лицензионным соглашением",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Покупка лицензии
            var license = _licenseService.PurchaseLicense(
                licenseType, customerName, customerEmail, amount, cardLastFour);

            if (license != null)
            {
                MessageBox.Show($"Лицензия успешно приобретена!\nКлюч: {license.LicenseKey}\nСохраните ключ для активации на других устройствах.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при покупке лицензии: {ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidatePurchaseFields()
    {
        // Проверка имени
        if (string.IsNullOrWhiteSpace(CustomerNameTextBox.Text))
        {
            MessageBox.Show("Введите ваше имя", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Проверка email
        if (string.IsNullOrWhiteSpace(CustomerEmailTextBox.Text) ||
            !IsValidEmail(CustomerEmailTextBox.Text))
        {
            MessageBox.Show("Введите корректный email", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Проверка номера карты - упрощаем: только 16 цифр
        string cardNumber = CardNumberTextBox.Text.Replace(" ", "");
        if (string.IsNullOrWhiteSpace(cardNumber) ||
            !Regex.IsMatch(cardNumber, @"^\d{16}$"))
        {
            MessageBox.Show("Введите корректный номер карты (16 цифр)", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Проверка срока действия
        if (string.IsNullOrWhiteSpace(CardExpiryTextBox.Text) ||
            !IsValidExpiryDate(CardExpiryTextBox.Text))
        {
            MessageBox.Show("Введите корректный срок действия карты (ММ/ГГ)", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Проверка CVV
        if (string.IsNullOrWhiteSpace(CardCvvTextBox.Text) ||
            !IsValidCvv(CardCvvTextBox.Text))
        {
            MessageBox.Show("Введите корректный CVV код (3 цифры)", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Проверка имени владельца
        if (string.IsNullOrWhiteSpace(CardHolderTextBox.Text))
        {
            MessageBox.Show("Введите имя владельца карты", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private bool ValidateCard()
    {
        // Проверка срока действия карты
        if (!string.IsNullOrWhiteSpace(CardExpiryTextBox.Text))
        {
            var parts = CardExpiryTextBox.Text.Split('/');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int month) &&
                int.TryParse(parts[1], out int year))
            {
                // Добавляем 2000 к году
                year += 2000;

                var expiryDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
                if (expiryDate < DateTime.Now)
                {
                    MessageBox.Show("Срок действия карты истек", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
        }

        return true;
    }

    private void ActivateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string licenseKey = LicenseKeyTextBox.Text.Trim();
            string customerName = ActivationNameTextBox.Text.Trim();
            string customerEmail = ActivationEmailTextBox.Text.Trim();

            // Валидация
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                MessageBox.Show("Введите лицензионный ключ", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(customerName))
            {
                MessageBox.Show("Введите ваше имя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(customerEmail) || !IsValidEmail(customerEmail))
            {
                MessageBox.Show("Введите корректный email", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Активация лицензии
            var license = _licenseService.ActivateLicense(licenseKey, customerName, customerEmail);

            if (license != null)
            {
                ActivationStatusText.Text = "✅ Лицензия успешно активирована!";
                ActivationStatusText.Foreground = System.Windows.Media.Brushes.Green;

                MessageBox.Show("Лицензия успешно активирована!\nПриложение будет перезапущено для применения изменений.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            else
            {
                ActivationStatusText.Text = "❌ Неверный или уже использованный ключ";
                ActivationStatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при активации лицензии: {ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    // Вспомогательные методы валидации
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

    private bool IsValidExpiryDate(string expiryDate)
    {
        return Regex.IsMatch(expiryDate, @"^(0[1-9]|1[0-2])\/?([0-9]{2})$");
    }

    private bool IsValidCvv(string cvv)
    {
        return Regex.IsMatch(cvv, @"^\d{3,4}$");
    }
}