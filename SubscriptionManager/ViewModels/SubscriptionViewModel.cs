using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using SubscriptionManager.Commands;
using SubscriptionManager.Models;
using SubscriptionManager.Services;

namespace SubscriptionManager.ViewModels
{
    public class SubscriptionViewModel : ViewModelBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private ObservableCollection<CustomerSubscription> _customers;
        private CustomerSubscription? _selectedCustomer;
        private CustomerSubscription _newCustomer;
        private ObservableCollection<CounterHistory> _counterHistory;
        private decimal _newReading;
        private decimal _pricePerUnit = 1.0m;
        private string _searchText = string.Empty;
        private bool _isNewCustomerDialogOpen;
        private bool _isInitialized;
        private ObservableCollection<SubscriptionType> _availableSubscriptionTypes;

        public SubscriptionViewModel(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
            _customers = new ObservableCollection<CustomerSubscription>();
            _counterHistory = new ObservableCollection<CounterHistory>();
            _newCustomer = new CustomerSubscription();
            _availableSubscriptionTypes = new ObservableCollection<SubscriptionType>();

            InitializeCommands();
     
        }

        public ObservableCollection<CustomerSubscription> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public CustomerSubscription? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (SetProperty(ref _selectedCustomer, value))
                {
                    _ = LoadCustomerHistoryAsync();
                }
            }
        }

        public CustomerSubscription NewCustomer
        {
            get => _newCustomer;
            set => SetProperty(ref _newCustomer, value);
        }

        public ObservableCollection<CounterHistory> CounterHistory
        {
            get => _counterHistory;
            set => SetProperty(ref _counterHistory, value);
        }

        public decimal NewReading
        {
            get => _newReading;
            set => SetProperty(ref _newReading, value);
        }

        public decimal PricePerUnit
        {
            get => _pricePerUnit;
            set => SetProperty(ref _pricePerUnit, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = FilterCustomersAsync();
                }
            }
        }

        public bool IsNewCustomerDialogOpen
        {
            get => _isNewCustomerDialogOpen;
            set => SetProperty(ref _isNewCustomerDialogOpen, value);
        }

        public bool IsInitialized
        {
            get => _isInitialized;
            private set => SetProperty(ref _isInitialized, value);
        }

        public ObservableCollection<SubscriptionType> AvailableSubscriptionTypes
        {
            get => _availableSubscriptionTypes;
            set => SetProperty(ref _availableSubscriptionTypes, value);
        }

        // Commands
        public ICommand AddCustomerCommand { get; private set; } = null!;
        public ICommand SaveCustomerCommand { get; private set; } = null!;
        public ICommand DeleteCustomerCommand { get; private set; } = null!;
        public ICommand SaveReadingCommand { get; private set; } = null!;
        public ICommand OpenNewCustomerDialogCommand { get; private set; } = null!;
        public ICommand CloseNewCustomerDialogCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;

    
        public async Task InitializeAsync()
        {
            if (IsInitialized) return;

            try
            {
                await LoadDataAsync();
                await LoadSubscriptionTypesAsync();
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Failed to initialize subscription data: {ex.Message}");
            }
        }

        private void InitializeCommands()
        {
            AddCustomerCommand = new AsyncRelayCommand(AddCustomerAsync);
            SaveCustomerCommand = new AsyncRelayCommand(SaveCustomerAsync);
            DeleteCustomerCommand = new AsyncRelayCommand(DeleteCustomerAsync);
            SaveReadingCommand = new AsyncRelayCommand(SaveReadingAsync);
            OpenNewCustomerDialogCommand = new RelayCommand(_ => OpenNewCustomerDialog());
            CloseNewCustomerDialogCommand = new RelayCommand(_ => CloseNewCustomerDialog());
            RefreshCommand = new AsyncRelayCommand(LoadDataAsync);
        }

        private async Task LoadDataAsync(object? parameter = null)
        {
            try
            {
                var customers = await _subscriptionService.GetAllCustomersAsync();


                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Customers = new ObservableCollection<CustomerSubscription>(customers);
                });
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error loading data: {ex.Message}");
            }
        }

        private async Task LoadCustomerHistoryAsync()
        {
            if (SelectedCustomer == null) return;

            try
            {
                var history = await _subscriptionService.GetCustomerHistoryAsync(SelectedCustomer.Id);

          
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    CounterHistory = new ObservableCollection<CounterHistory>(history);
                });
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error loading customer history: {ex.Message}");
            }
        }

        private async Task FilterCustomersAsync()
        {
            try
            {
                var allCustomers = await _subscriptionService.GetAllCustomersAsync();

                var filtered = string.IsNullOrWhiteSpace(SearchText)
                    ? allCustomers
                    : allCustomers.Where(c =>
                        c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        c.PhoneNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

     
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Customers = new ObservableCollection<CustomerSubscription>(filtered);
                });
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error filtering customers: {ex.Message}");
            }
        }

        private async Task LoadSubscriptionTypesAsync()
        {
            try
            {
                var subscriptionTypes = await _subscriptionService.GetActiveSubscriptionTypesAsync();

           
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    AvailableSubscriptionTypes = new ObservableCollection<SubscriptionType>(subscriptionTypes);
                });
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error loading subscription types: {ex.Message}");
            }
        }

        private void OpenNewCustomerDialog()
        {
            NewCustomer = new CustomerSubscription { PricePerUnit = PricePerUnit };
            IsNewCustomerDialogOpen = true;
        }

        private void CloseNewCustomerDialog()
        {
            IsNewCustomerDialogOpen = false;
            NewCustomer = new CustomerSubscription();
        }

        private async Task AddCustomerAsync(object? parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewCustomer.Name))
                {
                    await ShowErrorAsync("Customer name is required.");
                    return;
                }

                await _subscriptionService.AddCustomerAsync(NewCustomer);
                await ShowSuccessAsync("Customer added successfully!");

                CloseNewCustomerDialog();
                await LoadDataAsync();
                await LoadSubscriptionTypesAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error adding customer: {ex.Message}");
            }
        }

        private async Task SaveCustomerAsync(object? parameter)
        {
            try
            {
                if (SelectedCustomer == null) return;

                await _subscriptionService.UpdateCustomerAsync(SelectedCustomer);
                await ShowSuccessAsync("Customer updated successfully!");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error updating customer: {ex.Message}");
            }
        }

        private async Task DeleteCustomerAsync(object? parameter)
        {
            try
            {
                if (SelectedCustomer == null) return;

                var result = MessageBox.Show(
                    $"Are you sure you want to delete {SelectedCustomer.Name}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _subscriptionService.DeleteCustomerAsync(SelectedCustomer.Id);
                    await ShowSuccessAsync("Customer deleted successfully!");
                    await LoadDataAsync();
                    SelectedCustomer = null;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error deleting customer: {ex.Message}");
            }
        }

        private async Task SaveReadingAsync(object? parameter)
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    await ShowErrorAsync("Please select a customer.");
                    return;
                }

                if (NewReading <= SelectedCustomer.NewCounter)
                {
                    await ShowErrorAsync("New reading must be greater than current reading.");
                    return;
                }

                await _subscriptionService.SaveReadingAsync(SelectedCustomer.Id, NewReading, PricePerUnit);
                await ShowSuccessAsync("Reading saved successfully!");

                NewReading = 0;
                await LoadDataAsync();
                await LoadCustomerHistoryAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error saving reading: {ex.Message}");
            }
        }
    }
}