using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using SubscriptionManager.Commands;
using SubscriptionManager.Models;
using SubscriptionManager.Services;

namespace SubscriptionManager.ViewModels
{
    public class ExpenseViewModel : ViewModelBase
    {
        private readonly IExpenseService _expenseService;
        private ObservableCollection<Expense> _expenses;
        private Expense? _selectedExpense;
        private Expense _newExpense;
        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;
        private int _selectedYear = DateTime.Now.Year;
        private int _selectedMonth = DateTime.Now.Month;
        private decimal _totalExpenses;
        private decimal _totalConsumption;
        private decimal _totalRevenue;
        private decimal _totalProfit;
        private bool _isNewExpenseDialogOpen;
        private bool _isInitialized;
        private bool _isMonthlyView = true;
        private readonly List<string> _categories = new()
        {
            "Motor Expenses",
            "Motor Salaries",
            "Office Supplies",
            "Utilities",
            "Maintenance",
            "Other"
        };

        public ExpenseViewModel(IExpenseService expenseService)
        {
            _expenseService = expenseService;
            _expenses = new ObservableCollection<Expense>();
            _newExpense = new Expense();

            InitializeCommands();
        }

        public ObservableCollection<Expense> Expenses
        {
            get => _expenses;
            set => SetProperty(ref _expenses, value);
        }

        public Expense? SelectedExpense
        {
            get => _selectedExpense;
            set => SetProperty(ref _selectedExpense, value);
        }

        public Expense NewExpense
        {
            get => _newExpense;
            set => SetProperty(ref _newExpense, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value) && !IsMonthlyView)
                {
                    _ = FilterExpensesAsync();
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value) && !IsMonthlyView)
                {
                    _ = FilterExpensesAsync();
                }
            }
        }

        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (SetProperty(ref _selectedYear, value))
                {
                    _ = LoadMonthlyDataAsync();
                }
            }
        }

        public int SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (SetProperty(ref _selectedMonth, value))
                {
                    _ = LoadMonthlyDataAsync();
                }
            }
        }

        public bool IsMonthlyView
        {
            get => _isMonthlyView;
            set
            {
                if (SetProperty(ref _isMonthlyView, value))
                {
                    if (value)
                        _ = LoadMonthlyDataAsync();
                    else
                        _ = FilterExpensesAsync();
                }
            }
        }

        public decimal TotalExpenses
        {
            get => _totalExpenses;
            set => SetProperty(ref _totalExpenses, value);
        }

        public decimal TotalConsumption
        {
            get => _totalConsumption;
            set => SetProperty(ref _totalConsumption, value);
        }

        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set => SetProperty(ref _totalRevenue, value);
        }

        public decimal TotalProfit
        {
            get => _totalProfit;
            set => SetProperty(ref _totalProfit, value);
        }

        public bool IsNewExpenseDialogOpen
        {
            get => _isNewExpenseDialogOpen;
            set => SetProperty(ref _isNewExpenseDialogOpen, value);
        }

        public bool IsInitialized
        {
            get => _isInitialized;
            private set => SetProperty(ref _isInitialized, value);
        }

        public List<string> Categories => _categories;

        public List<int> AvailableYears => Enumerable.Range(DateTime.Now.Year - 5, 11).Reverse().ToList();

        public List<int> AvailableMonths => Enumerable.Range(1, 12).ToList();

        public string GetMonthName(int month) => new DateTime(2000, month, 1).ToString("MMMM");

        public ICommand AddExpenseCommand { get; private set; } = null!;
        public ICommand SaveExpenseCommand { get; private set; } = null!;
        public ICommand DeleteExpenseCommand { get; private set; } = null!;
        public ICommand OpenNewExpenseDialogCommand { get; private set; } = null!;
        public ICommand CloseNewExpenseDialogCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand FilterCommand { get; private set; } = null!;
        public ICommand SwitchToMonthlyViewCommand { get; private set; } = null!;
        public ICommand SwitchToDateRangeViewCommand { get; private set; } = null!;
        public ICommand DebugConsumptionCommand { get; private set; } = null!;


        public async Task InitializeAsync()
        {
            if (IsInitialized) return;

            try
            {
                await LoadMonthlyDataAsync();
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Failed to initialize expense data: {ex.Message}");
            }
        }

        private void InitializeCommands()
        {
            AddExpenseCommand = new AsyncRelayCommand(AddExpenseAsync);
            SaveExpenseCommand = new AsyncRelayCommand(SaveExpenseAsync);
            DeleteExpenseCommand = new AsyncRelayCommand(DeleteExpenseAsync);
            OpenNewExpenseDialogCommand = new RelayCommand(_ => OpenNewExpenseDialog());
            CloseNewExpenseDialogCommand = new RelayCommand(_ => CloseNewExpenseDialog());
            RefreshCommand = new AsyncRelayCommand(LoadCurrentDataAsync);
            FilterCommand = new AsyncRelayCommand(FilterExpensesAsync);
            DebugConsumptionCommand = new AsyncRelayCommand(DebugConsumptionAsync);
            SwitchToMonthlyViewCommand = new RelayCommand(_ => IsMonthlyView = true);
            SwitchToDateRangeViewCommand = new RelayCommand(_ => IsMonthlyView = false);
        }

        private async Task LoadCurrentDataAsync(object? parameter = null)
        {
            if (IsMonthlyView)
                await LoadMonthlyDataAsync();
            else
                await FilterExpensesAsync();
        }

        private async Task LoadMonthlyDataAsync()
        {
            try
            {
                var expenses = await _expenseService.GetExpensesByMonthAsync(SelectedYear, SelectedMonth);
                var totalExpenses = await _expenseService.GetTotalExpensesForMonthAsync(SelectedYear, SelectedMonth);
                var totalConsumption = await _expenseService.GetTotalConsumptionForMonthAsync(SelectedYear, SelectedMonth);
                var totalRevenue = await _expenseService.GetTotalRevenueForMonthAsync(SelectedYear, SelectedMonth);
                var totalProfit = await _expenseService.GetTotalProfitForMonthAsync(SelectedYear, SelectedMonth);

                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Expenses = new ObservableCollection<Expense>(expenses);
                    TotalExpenses = totalExpenses;
                    TotalConsumption = totalConsumption;
                    TotalRevenue = totalRevenue;
                    TotalProfit = totalProfit;
                });
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error loading monthly data: {ex.Message}");
            }
        }
        private async Task DebugConsumptionAsync(object? parameter)
        {
            try
            {
                var debugInfo = await _expenseService.DebugConsumptionDataAsync(SelectedYear, SelectedMonth);
                MessageBox.Show(debugInfo, "Consumption Debug Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Debug error: {ex.Message}");
            }
        }

        private async Task FilterExpensesAsync(object? parameter = null)
        {
            try
            {
                var expenses = await _expenseService.GetExpensesByDateRangeAsync(StartDate, EndDate);
                var totalExpenses = await _expenseService.GetTotalExpensesAsync(StartDate, EndDate);

                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Expenses = new ObservableCollection<Expense>(expenses);
                    TotalExpenses = totalExpenses;
                    TotalConsumption = 0;
                    TotalRevenue = 0;
                    TotalProfit = 0;
                });
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error filtering expenses: {ex.Message}");
            }
        }

        private void OpenNewExpenseDialog()
        {
            NewExpense = new Expense { Date = DateTime.Now };
            IsNewExpenseDialogOpen = true;
        }

        private void CloseNewExpenseDialog()
        {
            IsNewExpenseDialogOpen = false;
            NewExpense = new Expense();
        }

        private async Task AddExpenseAsync(object? parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewExpense.Reason))
                {
                    await ShowErrorAsync("Expense reason is required.");
                    return;
                }

                if (NewExpense.Amount <= 0)
                {
                    await ShowErrorAsync("Expense amount must be greater than zero.");
                    return;
                }

                await _expenseService.AddExpenseAsync(NewExpense);
                await ShowSuccessAsync("Expense added successfully!");

                CloseNewExpenseDialog();
                await LoadCurrentDataAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error adding expense: {ex.Message}");
            }
        }

        private async Task SaveExpenseAsync(object? parameter)
        {
            try
            {
                if (SelectedExpense == null) return;

                await _expenseService.UpdateExpenseAsync(SelectedExpense);
                await ShowSuccessAsync("Expense updated successfully!");
                await LoadCurrentDataAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error updating expense: {ex.Message}");
            }
        }

        private async Task DeleteExpenseAsync(object? parameter)
        {
            try
            {
                if (SelectedExpense == null) return;

                var result = MessageBox.Show(
                    $"Are you sure you want to delete this expense?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _expenseService.DeleteExpenseAsync(SelectedExpense.Id);
                    await ShowSuccessAsync("Expense deleted successfully!");
                    await LoadCurrentDataAsync();
                    SelectedExpense = null;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error deleting expense: {ex.Message}");
            }
        }
    }
}