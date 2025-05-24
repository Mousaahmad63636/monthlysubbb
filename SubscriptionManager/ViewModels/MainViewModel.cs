using System.Windows.Input;
using SubscriptionManager.Commands;
using System.Windows;

namespace SubscriptionManager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private int _selectedTabIndex;
        private bool _isInitialized;

        public MainViewModel(SubscriptionViewModel subscriptionViewModel, ExpenseViewModel expenseViewModel, SettingsViewModel settingsViewModel)
        {
            SubscriptionViewModel = subscriptionViewModel;
            ExpenseViewModel = expenseViewModel;
            SettingsViewModel = settingsViewModel;
            _currentViewModel = subscriptionViewModel;

            SwitchToSubscriptionsCommand = new RelayCommand(_ => SwitchToSubscriptions());
            SwitchToExpensesCommand = new RelayCommand(_ => SwitchToExpenses());
            SwitchToSettingsCommand = new RelayCommand(_ => SwitchToSettings());
        }

        public SubscriptionViewModel SubscriptionViewModel { get; }
        public ExpenseViewModel ExpenseViewModel { get; }
        public SettingsViewModel SettingsViewModel { get; }

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (SetProperty(ref _selectedTabIndex, value))
                {
                    CurrentViewModel = value switch
                    {
                        0 => SubscriptionViewModel,
                        1 => ExpenseViewModel,
                        2 => SettingsViewModel,
                        _ => SubscriptionViewModel
                    };
                }
            }
        }

        public bool IsInitialized
        {
            get => _isInitialized;
            private set => SetProperty(ref _isInitialized, value);
        }

        public ICommand SwitchToSubscriptionsCommand { get; }
        public ICommand SwitchToExpensesCommand { get; }
        public ICommand SwitchToSettingsCommand { get; }

        public async Task InitializeAsync()
        {
            if (IsInitialized) return;

            try
            {

                await SubscriptionViewModel.InitializeAsync();
                await ExpenseViewModel.InitializeAsync();
                await SettingsViewModel.InitializeAsync();

                IsInitialized = true;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Failed to initialize application: {ex.Message}");
            }
        }

        private void SwitchToSubscriptions()
        {
            SelectedTabIndex = 0;
        }

        private void SwitchToExpenses()
        {
            SelectedTabIndex = 1;
        }

        private void SwitchToSettings()
        {
            SelectedTabIndex = 2;
        }
    }
}