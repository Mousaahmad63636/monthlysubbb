using System.Windows.Input;
using SubscriptionManager.Commands;
using System.Windows;

namespace SubscriptionManager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private int _selectedTabIndex;

        public MainViewModel(SubscriptionViewModel subscriptionViewModel, ExpenseViewModel expenseViewModel)
        {
            SubscriptionViewModel = subscriptionViewModel;
            ExpenseViewModel = expenseViewModel;
            _currentViewModel = subscriptionViewModel;

            SwitchToSubscriptionsCommand = new RelayCommand(_ => SwitchToSubscriptions());
            SwitchToExpensesCommand = new RelayCommand(_ => SwitchToExpenses());
        }

        public SubscriptionViewModel SubscriptionViewModel { get; }
        public ExpenseViewModel ExpenseViewModel { get; }

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
                    CurrentViewModel = value == 0 ? SubscriptionViewModel : ExpenseViewModel;
                }
            }
        }

        public ICommand SwitchToSubscriptionsCommand { get; }
        public ICommand SwitchToExpensesCommand { get; }

        private void SwitchToSubscriptions()
        {
            SelectedTabIndex = 0;
        }

        private void SwitchToExpenses()
        {
            SelectedTabIndex = 1;
        }
    }
}