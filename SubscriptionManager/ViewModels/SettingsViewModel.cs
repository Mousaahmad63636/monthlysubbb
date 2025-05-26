using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using SubscriptionManager.Commands;
using SubscriptionManager.Models;
using SubscriptionManager.Services;

namespace SubscriptionManager.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private Settings _settings;
        private Settings _originalSettings; // Keep track of original settings for comparison
        private ObservableCollection<SubscriptionType> _subscriptionTypes;
        private SubscriptionType? _selectedSubscriptionType;
        private SubscriptionType _newSubscriptionType;
        private bool _isNewSubscriptionTypeDialogOpen;
        private bool _isEditingSubscriptionType;
        private bool _isInitialized;

        private readonly List<string> _subscriptionCategories = new()
        {
            "Basic",
            "Standard",
            "Premium",
            "Enterprise",
            "Custom"
        };

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _settings = new Settings();
            _originalSettings = new Settings();
            _subscriptionTypes = new ObservableCollection<SubscriptionType>();
            _newSubscriptionType = new SubscriptionType();

            InitializeCommands();
        }

        #region Properties

        public Settings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public ObservableCollection<SubscriptionType> SubscriptionTypes
        {
            get => _subscriptionTypes;
            set => SetProperty(ref _subscriptionTypes, value);
        }

        public SubscriptionType? SelectedSubscriptionType
        {
            get => _selectedSubscriptionType;
            set => SetProperty(ref _selectedSubscriptionType, value);
        }

        public SubscriptionType NewSubscriptionType
        {
            get => _newSubscriptionType;
            set => SetProperty(ref _newSubscriptionType, value);
        }

        public bool IsNewSubscriptionTypeDialogOpen
        {
            get => _isNewSubscriptionTypeDialogOpen;
            set => SetProperty(ref _isNewSubscriptionTypeDialogOpen, value);
        }

        public bool IsEditingSubscriptionType
        {
            get => _isEditingSubscriptionType;
            set => SetProperty(ref _isEditingSubscriptionType, value);
        }

        public bool IsInitialized
        {
            get => _isInitialized;
            private set => SetProperty(ref _isInitialized, value);
        }

        public List<string> SubscriptionCategories => _subscriptionCategories;

        #endregion

        #region Commands

        public ICommand SaveSettingsCommand { get; private set; } = null!;
        public ICommand LoadDataCommand { get; private set; } = null!;
        public ICommand AddSubscriptionTypeCommand { get; private set; } = null!;
        public ICommand EditSubscriptionTypeCommand { get; private set; } = null!;
        public ICommand SaveSubscriptionTypeCommand { get; private set; } = null!;
        public ICommand DeleteSubscriptionTypeCommand { get; private set; } = null!;
        public ICommand OpenNewSubscriptionTypeDialogCommand { get; private set; } = null!;
        public ICommand CloseSubscriptionTypeDialogCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;

        #endregion

        public async Task InitializeAsync()
        {
            if (IsInitialized) return;

            try
            {
                await LoadDataAsync();
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Failed to initialize settings data: {ex.Message}");
            }
        }

        private void InitializeCommands()
        {
            SaveSettingsCommand = new AsyncRelayCommand(SaveSettingsAsync);
            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            AddSubscriptionTypeCommand = new AsyncRelayCommand(AddSubscriptionTypeAsync);
            EditSubscriptionTypeCommand = new RelayCommand(EditSubscriptionType);
            SaveSubscriptionTypeCommand = new AsyncRelayCommand(SaveSubscriptionTypeAsync);
            DeleteSubscriptionTypeCommand = new AsyncRelayCommand(DeleteSubscriptionTypeAsync);
            OpenNewSubscriptionTypeDialogCommand = new RelayCommand(_ => OpenNewSubscriptionTypeDialog());
            CloseSubscriptionTypeDialogCommand = new RelayCommand(_ => CloseSubscriptionTypeDialog());
            RefreshCommand = new AsyncRelayCommand(LoadDataAsync);
        }

        #region Private Methods

        private async Task LoadDataAsync(object? parameter = null)
        {
            try
            {
                // Load settings and subscription types
                var settings = await _settingsService.GetSettingsAsync();
                var subscriptionTypes = await _settingsService.GetAllSubscriptionTypesAsync();

                // Update UI on the main thread
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Settings = settings;
                    // Keep a copy of the original settings for comparison
                    _originalSettings = new Settings
                    {
                        Id = settings.Id,
                        DefaultPricePerUnit = settings.DefaultPricePerUnit,
                        CompanyName = settings.CompanyName,
                        AdminEmail = settings.AdminEmail,
                        AutoCalculateMonthlyFees = settings.AutoCalculateMonthlyFees,
                        BillingDay = settings.BillingDay,
                        CreatedAt = settings.CreatedAt,
                        UpdatedAt = settings.UpdatedAt
                    };
                    SubscriptionTypes = new ObservableCollection<SubscriptionType>(subscriptionTypes);
                });
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error loading settings data: {ex.Message}");
            }
        }

        private async Task SaveSettingsAsync(object? parameter)
        {
            try
            {
                // Basic validation
                if (Settings.DefaultPricePerUnit <= 0)
                {
                    await ShowErrorAsync("Default price per unit must be greater than zero.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Settings.CompanyName))
                {
                    await ShowErrorAsync("Company name is required.");
                    return;
                }

                if (Settings.BillingDay < 1 || Settings.BillingDay > 28)
                {
                    await ShowErrorAsync("Billing day must be between 1 and 28.");
                    return;
                }

                // Check if the price per unit has changed
                bool priceChanged = _originalSettings.DefaultPricePerUnit != Settings.DefaultPricePerUnit;

                if (priceChanged)
                {
                    // Get the count of customers that will be affected
                    var customerCount = await _settingsService.GetActiveCustomersCountAsync();

                    if (customerCount > 0)
                    {
                        var result = MessageBox.Show(
                            $"Changing the default price per unit from ${_originalSettings.DefaultPricePerUnit:F2} to ${Settings.DefaultPricePerUnit:F2} " +
                            $"will automatically update the pricing for all {customerCount} active customers.\n\n" +
                            "This change will apply to future meter readings only. Historical readings will retain their original pricing.\n\n" +
                            "Do you want to continue?",
                            "Confirm Price Change",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result != MessageBoxResult.Yes)
                        {
                            // User cancelled, reload original settings
                            await LoadDataAsync();
                            return;
                        }
                    }
                }

                // Save the settings (this will also update customer pricing if needed)
                await _settingsService.UpdateSettingsAsync(Settings);

                // Show appropriate success message
                if (priceChanged)
                {
                    var customerCount = await _settingsService.GetActiveCustomersCountAsync();
                    await ShowSuccessAsync($"Settings saved successfully! Price per unit updated for {customerCount} customers.");
                }
                else
                {
                    await ShowSuccessAsync("Settings saved successfully!");
                }

                // Reload data to ensure we have the latest information
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error saving settings: {ex.Message}");
                // Reload original settings on error
                await LoadDataAsync();
            }
        }

        private void OpenNewSubscriptionTypeDialog()
        {
            NewSubscriptionType = new SubscriptionType
            {
                Category = "Standard",
                IsActive = true
            };
            IsEditingSubscriptionType = false;
            IsNewSubscriptionTypeDialogOpen = true;
        }

        private void EditSubscriptionType(object? parameter)
        {
            if (SelectedSubscriptionType == null) return;

            // Create a copy for editing
            NewSubscriptionType = new SubscriptionType
            {
                Id = SelectedSubscriptionType.Id,
                Name = SelectedSubscriptionType.Name,
                Description = SelectedSubscriptionType.Description,
                MonthlyFee = SelectedSubscriptionType.MonthlyFee,
                Category = SelectedSubscriptionType.Category,
                IsActive = SelectedSubscriptionType.IsActive
            };

            IsEditingSubscriptionType = true;
            IsNewSubscriptionTypeDialogOpen = true;
        }

        private void CloseSubscriptionTypeDialog()
        {
            IsNewSubscriptionTypeDialogOpen = false;
            IsEditingSubscriptionType = false;
            NewSubscriptionType = new SubscriptionType();
        }

        private async Task AddSubscriptionTypeAsync(object? parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewSubscriptionType.Name))
                {
                    await ShowErrorAsync("Subscription type name is required.");
                    return;
                }

                if (NewSubscriptionType.MonthlyFee < 0)
                {
                    await ShowErrorAsync("Monthly fee cannot be negative.");
                    return;
                }

                if (IsEditingSubscriptionType)
                {
                    await _settingsService.UpdateSubscriptionTypeAsync(NewSubscriptionType);
                    await ShowSuccessAsync("Subscription type updated successfully!");
                }
                else
                {
                    await _settingsService.AddSubscriptionTypeAsync(NewSubscriptionType);
                    await ShowSuccessAsync("Subscription type added successfully!");
                }

                CloseSubscriptionTypeDialog();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error saving subscription type: {ex.Message}");
            }
        }

        private async Task SaveSubscriptionTypeAsync(object? parameter)
        {
            await AddSubscriptionTypeAsync(parameter);
        }

        private async Task DeleteSubscriptionTypeAsync(object? parameter)
        {
            try
            {
                if (SelectedSubscriptionType == null) return;

                var result = MessageBox.Show(
                    $"Are you sure you want to delete the subscription type '{SelectedSubscriptionType.Name}'?\n\n" +
                    "Note: If customers are using this subscription type, it will be deactivated instead of deleted.",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _settingsService.DeleteSubscriptionTypeAsync(SelectedSubscriptionType.Id);
                    await ShowSuccessAsync("Subscription type deleted successfully!");
                    await LoadDataAsync();
                    SelectedSubscriptionType = null;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error deleting subscription type: {ex.Message}");
            }
        }

        #endregion
    }
}