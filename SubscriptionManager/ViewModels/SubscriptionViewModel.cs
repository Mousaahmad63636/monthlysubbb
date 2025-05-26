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
        private readonly ISettingsService _settingsService;
        private readonly IPaymentService _paymentService;
        private readonly IPrintService _printService;
        private ObservableCollection<CustomerSubscription> _customers;
        private CustomerSubscription? _selectedCustomer;
        private CustomerSubscription _newCustomer;
        private ObservableCollection<CounterHistory> _counterHistory;
        private ObservableCollection<Payment> _paymentHistory;
        private Payment? _selectedPayment;
        private CounterHistory? _selectedCounterHistory;
        private decimal _newReading;
        private decimal _pricePerUnit = 1.0m;
        private decimal _paymentAmount;
        private string _selectedPaymentMethod = "Cash";
        private string _searchText = string.Empty;
        private bool _isNewCustomerDialogOpen;
        private bool _isInitialized;
        private ObservableCollection<SubscriptionType> _availableSubscriptionTypes;

        private readonly List<string> _paymentMethods = new()
        {
            "Cash",
            "Credit Card",
            "Debit Card",
            "Bank Transfer",
            "Check",
            "Mobile Payment",
            "Other"
        };

        public SubscriptionViewModel(ISubscriptionService subscriptionService, ISettingsService settingsService,
            IPaymentService paymentService, IPrintService printService)
        {
            _subscriptionService = subscriptionService;
            _settingsService = settingsService;
            _paymentService = paymentService;
            _printService = printService;
            _customers = new ObservableCollection<CustomerSubscription>();
            _counterHistory = new ObservableCollection<CounterHistory>();
            _paymentHistory = new ObservableCollection<Payment>();
            _newCustomer = new CustomerSubscription();
            _availableSubscriptionTypes = new ObservableCollection<SubscriptionType>();

            InitializeCommands();
        }

        #region Properties

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
                    // Update the price per unit to match the selected customer's current rate
                    if (value != null)
                    {
                        PricePerUnit = value.PricePerUnit;
                        PaymentAmount = value.TotalMonthlyBill; // Default to full bill amount
                    }
                    _ = LoadCustomerHistoryAsync();
                    _ = LoadPaymentHistoryAsync();
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

        public ObservableCollection<Payment> PaymentHistory
        {
            get => _paymentHistory;
            set => SetProperty(ref _paymentHistory, value);
        }

        public Payment? SelectedPayment
        {
            get => _selectedPayment;
            set => SetProperty(ref _selectedPayment, value);
        }

        public CounterHistory? SelectedCounterHistory
        {
            get => _selectedCounterHistory;
            set => SetProperty(ref _selectedCounterHistory, value);
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

        public decimal PaymentAmount
        {
            get => _paymentAmount;
            set => SetProperty(ref _paymentAmount, value);
        }

        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set => SetProperty(ref _selectedPaymentMethod, value);
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

        public List<string> PaymentMethods => _paymentMethods;

        #endregion

        #region Commands

        public ICommand AddCustomerCommand { get; private set; } = null!;
        public ICommand SaveCustomerCommand { get; private set; } = null!;
        public ICommand DeleteCustomerCommand { get; private set; } = null!;
        public ICommand SaveReadingCommand { get; private set; } = null!;
        public ICommand OpenNewCustomerDialogCommand { get; private set; } = null!;
        public ICommand CloseNewCustomerDialogCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;

        // Payment Commands
        public ICommand SettleFullBillCommand { get; private set; } = null!;
        public ICommand RecordPaymentCommand { get; private set; } = null!;
        public ICommand RefreshPaymentsCommand { get; private set; } = null!;

        // Print Commands
        public ICommand PrintInvoiceCommand { get; private set; } = null!;
        public ICommand PrintSelectedReadingCommand { get; private set; } = null!;
        public ICommand PrintSingleReadingCommand { get; private set; } = null!;
        public ICommand ExportInvoiceToPdfCommand { get; private set; } = null!;
        public ICommand RefreshHistoryCommand { get; private set; } = null!;
        public ICommand ShowPrintPreviewCommand { get; private set; } = null!;

        #endregion

        public async Task InitializeAsync()
        {
            if (IsInitialized) return;

            try
            {
                await LoadDataAsync();
                await LoadSubscriptionTypesAsync();
                await LoadDefaultPricingAsync();
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

            // Payment Commands
            SettleFullBillCommand = new AsyncRelayCommand(SettleFullBillAsync);
            RecordPaymentCommand = new AsyncRelayCommand(RecordPaymentAsync);
            RefreshPaymentsCommand = new AsyncRelayCommand(LoadPaymentHistoryAsync);

            // Print Commands
            PrintInvoiceCommand = new AsyncRelayCommand(PrintInvoiceAsync);
            PrintSelectedReadingCommand = new AsyncRelayCommand(PrintSelectedReadingAsync);
            PrintSingleReadingCommand = new AsyncRelayCommand(PrintSingleReadingAsync);
            ExportInvoiceToPdfCommand = new AsyncRelayCommand(ExportInvoiceToPdfAsync);
            RefreshHistoryCommand = new AsyncRelayCommand(LoadCustomerHistoryAsync);
            ShowPrintPreviewCommand = new AsyncRelayCommand(ShowPrintPreviewAsync);
        }

        #region Data Loading Methods

        private async Task LoadDataAsync(object? parameter = null)
        {
            try
            {
                var customers = await _subscriptionService.GetAllCustomersAsync();

                // Update UI on main thread
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

        private async Task LoadCustomerHistoryAsync(object? parameter = null)
        {
            if (SelectedCustomer == null) return;

            try
            {
                var history = await _subscriptionService.GetCustomerHistoryAsync(SelectedCustomer.Id);

                // Update UI on main thread
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

        private async Task LoadPaymentHistoryAsync(object? parameter = null)
        {
            if (SelectedCustomer == null) return;

            try
            {
                var payments = await _paymentService.GetCustomerPaymentsAsync(SelectedCustomer.Id);

                // Update UI on main thread
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    PaymentHistory = new ObservableCollection<Payment>(payments);
                });
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error loading payment history: {ex.Message}");
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

                // Update UI on main thread
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

                // Update UI on main thread
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

        /// <summary>
        /// Loads the default price per unit from settings.
        /// This is used for new customers and as the default for meter readings.
        /// </summary>
        private async Task LoadDefaultPricingAsync()
        {
            try
            {
                var settings = await _settingsService.GetSettingsAsync();

                // Update the default price per unit if no customer is selected
                if (SelectedCustomer == null)
                {
                    PricePerUnit = settings.DefaultPricePerUnit;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error loading default pricing: {ex.Message}");
            }
        }

        #endregion

        #region Customer Management Methods

        private async void OpenNewCustomerDialog()
        {
            try
            {
                // Get the current default price from settings
                var settings = await _settingsService.GetSettingsAsync();

                NewCustomer = new CustomerSubscription
                {
                    PricePerUnit = settings.DefaultPricePerUnit
                };

                IsNewCustomerDialogOpen = true;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error preparing new customer dialog: {ex.Message}");
            }
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

                if (NewCustomer.PricePerUnit <= 0)
                {
                    await ShowErrorAsync("Price per unit must be greater than zero.");
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

                if (SelectedCustomer.PricePerUnit <= 0)
                {
                    await ShowErrorAsync("Price per unit must be greater than zero.");
                    return;
                }

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
                    $"Are you sure you want to delete {SelectedCustomer.Name}?\n\nThis will also delete all associated payment records.",
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

        #endregion

        #region Meter Reading Methods

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

                if (PricePerUnit <= 0)
                {
                    await ShowErrorAsync("Price per unit must be greater than zero.");
                    return;
                }

                await _subscriptionService.SaveReadingAsync(SelectedCustomer.Id, NewReading, PricePerUnit);
                await ShowSuccessAsync("Reading saved successfully!");

                NewReading = 0;
                await LoadDataAsync();
                await LoadCustomerHistoryAsync();

                // Update payment amount to reflect new bill
                if (SelectedCustomer != null)
                {
                    PaymentAmount = SelectedCustomer.TotalMonthlyBill;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error saving reading: {ex.Message}");
            }
        }

        #endregion

        #region Payment Methods

        private async Task SettleFullBillAsync(object? parameter)
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    await ShowErrorAsync("Please select a customer.");
                    return;
                }

                if (SelectedCustomer.TotalMonthlyBill <= 0)
                {
                    await ShowErrorAsync("No outstanding bill to settle.");
                    return;
                }

                var result = MessageBox.Show(
                    $"Settle the full bill of {SelectedCustomer.TotalMonthlyBill:C2} for {SelectedCustomer.Name}?",
                    "Confirm Full Bill Settlement",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var payment = await _paymentService.SettleCurrentBillAsync(
                        SelectedCustomer.Id,
                        SelectedCustomer.TotalMonthlyBill,
                        SelectedPaymentMethod,
                        "Full bill settlement");

                    await ShowSuccessAsync($"Full bill settled successfully! Receipt Number: {payment.ReceiptNumber}");

                    // Refresh data
                    await LoadDataAsync();
                    await LoadPaymentHistoryAsync();

                    // Reset payment amount
                    PaymentAmount = 0;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error settling full bill: {ex.Message}");
            }
        }

        private async Task RecordPaymentAsync(object? parameter)
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    await ShowErrorAsync("Please select a customer.");
                    return;
                }

                if (PaymentAmount <= 0)
                {
                    await ShowErrorAsync("Payment amount must be greater than zero.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedPaymentMethod))
                {
                    await ShowErrorAsync("Please select a payment method.");
                    return;
                }

                var payment = await _paymentService.SettleCurrentBillAsync(
                    SelectedCustomer.Id,
                    PaymentAmount,
                    SelectedPaymentMethod,
                    $"Partial payment - {SelectedPaymentMethod}");

                var message = payment.IsFullyPaid
                    ? $"Payment recorded successfully! Bill fully paid. Receipt Number: {payment.ReceiptNumber}"
                    : $"Payment recorded successfully! Remaining balance: {payment.RemainingBalance:C2}. Receipt Number: {payment.ReceiptNumber}";

                await ShowSuccessAsync(message);

                // Refresh data
                await LoadDataAsync();
                await LoadPaymentHistoryAsync();

                // Reset payment amount or set to remaining balance
                PaymentAmount = payment.IsFullyPaid ? 0 : payment.RemainingBalance;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error recording payment: {ex.Message}");
            }
        }

        #endregion

        #region Print Methods

        /// <summary>
        /// Prints complete invoice for the selected customer including all reading history
        /// </summary>
        private async Task PrintInvoiceAsync(object? parameter)
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    await ShowErrorAsync("Please select a customer to print invoice.");
                    return;
                }

                var settings = await _settingsService.GetSettingsAsync();
                var readingHistory = await _subscriptionService.GetCustomerHistoryAsync(SelectedCustomer.Id);

                if (!readingHistory.Any())
                {
                    await ShowErrorAsync("No reading history found for this customer.");
                    return;
                }

                var success = await _printService.PrintCustomerInvoiceAsync(SelectedCustomer, settings, readingHistory);

                if (success)
                {
                    await ShowSuccessAsync("Invoice printed successfully!");
                }
                else
                {
                    await ShowErrorAsync("Failed to print invoice. Please check your printer settings.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error printing invoice: {ex.Message}");
            }
        }

        /// <summary>
        /// Prints invoice for the currently selected reading in the history grid
        /// </summary>
        private async Task PrintSelectedReadingAsync(object? parameter)
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    await ShowErrorAsync("Please select a customer.");
                    return;
                }

                if (SelectedCounterHistory == null)
                {
                    await ShowErrorAsync("Please select a reading from the history to print.");
                    return;
                }

                var settings = await _settingsService.GetSettingsAsync();
                var success = await _printService.PrintSingleReadingInvoiceAsync(SelectedCustomer, settings, SelectedCounterHistory);

                if (success)
                {
                    await ShowSuccessAsync("Reading invoice printed successfully!");
                }
                else
                {
                    await ShowErrorAsync("Failed to print reading invoice. Please check your printer settings.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error printing selected reading: {ex.Message}");
            }
        }

        /// <summary>
        /// Prints invoice for a specific reading (used by DataGrid row buttons)
        /// </summary>
        private async Task PrintSingleReadingAsync(object? parameter)
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    await ShowErrorAsync("Please select a customer.");
                    return;
                }

                if (parameter is not CounterHistory reading)
                {
                    await ShowErrorAsync("Invalid reading data for printing.");
                    return;
                }

                var settings = await _settingsService.GetSettingsAsync();
                var success = await _printService.PrintSingleReadingInvoiceAsync(SelectedCustomer, settings, reading);

                if (success)
                {
                    await ShowSuccessAsync($"Invoice for reading on {reading.RecordDate:d} printed successfully!");
                }
                else
                {
                    await ShowErrorAsync("Failed to print reading invoice. Please check your printer settings.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error printing single reading: {ex.Message}");
            }
        }

        /// <summary>
        /// Exports customer invoice to PDF format
        /// </summary>
        private async Task ExportInvoiceToPdfAsync(object? parameter)
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    await ShowErrorAsync("Please select a customer to export invoice.");
                    return;
                }

                var settings = await _settingsService.GetSettingsAsync();
                var readingHistory = await _subscriptionService.GetCustomerHistoryAsync(SelectedCustomer.Id);

                if (!readingHistory.Any())
                {
                    await ShowErrorAsync("No reading history found for this customer.");
                    return;
                }

                var invoiceData = InvoiceData.CreateFromCustomer(SelectedCustomer, settings, readingHistory, true);
                var success = await _printService.ExportToPdfWithDialogAsync(invoiceData);

                if (success)
                {
                    await ShowSuccessAsync("Invoice exported successfully!");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error exporting invoice: {ex.Message}");
            }
        }

        /// <summary>
        /// Shows print preview dialog for the customer invoice
        /// </summary>
        private async Task ShowPrintPreviewAsync(object? parameter)
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    await ShowErrorAsync("Please select a customer to preview invoice.");
                    return;
                }

                var settings = await _settingsService.GetSettingsAsync();
                var readingHistory = await _subscriptionService.GetCustomerHistoryAsync(SelectedCustomer.Id);

                if (!readingHistory.Any())
                {
                    await ShowErrorAsync("No reading history found for this customer.");
                    return;
                }

                var invoiceData = InvoiceData.CreateFromCustomer(SelectedCustomer, settings, readingHistory, true);
                var printed = await _printService.ShowPrintPreviewAsync(invoiceData);

                if (printed)
                {
                    await ShowSuccessAsync("Invoice printed successfully!");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error showing print preview: {ex.Message}");
            }
        }

        #endregion
    }
}