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
        private decimal _totalExpenses;
        private bool _isNewExpenseDialogOpen;
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
            _ = LoadDataAsync();
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
                if (SetProperty(ref _startDate, value))
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
                if (SetProperty(ref _endDate, value))
                {
                    _ = FilterExpensesAsync();
                }
            }
        }

        public decimal TotalExpenses
        {
            get => _totalExpenses;
            set => SetProperty(ref _totalExpenses, value);
        }

        public bool IsNewExpenseDialogOpen
        {
            get => _isNewExpenseDialogOpen;
            set => SetProperty(ref _isNewExpenseDialogOpen, value);
        }

        public List<string> Categories => _categories;

        // Commands
        public ICommand AddExpenseCommand { get; private set; } = null!;
        public ICommand SaveExpenseCommand { get; private set; } = null!;
        public ICommand DeleteExpenseCommand { get; private set; } = null!;
        public ICommand OpenNewExpenseDialogCommand { get; private set; } = null!;
        public ICommand CloseNewExpenseDialogCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand FilterCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            AddExpenseCommand = new AsyncRelayCommand(AddExpenseAsync);
            SaveExpenseCommand = new AsyncRelayCommand(SaveExpenseAsync);
            DeleteExpenseCommand = new AsyncRelayCommand(DeleteExpenseAsync);
            OpenNewExpenseDialogCommand = new RelayCommand(_ => OpenNewExpenseDialog());
            CloseNewExpenseDialogCommand = new RelayCommand(_ => CloseNewExpenseDialog());
            RefreshCommand = new AsyncRelayCommand(LoadDataAsync);
            FilterCommand = new AsyncRelayCommand(FilterExpensesAsync);
        }

        private async Task LoadDataAsync(object? parameter = null)
        {
            try
            {
                var expenses = await _expenseService.GetAllExpensesAsync();
                Expenses = new ObservableCollection<Expense>(expenses);
                await CalculateTotalAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error loading expenses: {ex.Message}");
            }
        }

        private async Task FilterExpensesAsync(object? parameter = null)
        {
            try
            {
                var expenses = await _expenseService.GetExpensesByDateRangeAsync(StartDate, EndDate);
                Expenses = new ObservableCollection<Expense>(expenses);
                await CalculateTotalAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error filtering expenses: {ex.Message}");
            }
        }

        private async Task CalculateTotalAsync()
        {
            try
            {
                TotalExpenses = await _expenseService.GetTotalExpensesAsync(StartDate, EndDate);
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error calculating total: {ex.Message}");
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
                await LoadDataAsync();
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
                await LoadDataAsync();
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
                    await LoadDataAsync();
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