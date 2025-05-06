// MonthlySubscriptionManager/MainWindow.xaml.cs
using MonthlySubscriptionManager.ViewModels;
using QuickTechSystems.WPF.ViewModels;
using System.Windows;

namespace MonthlySubscriptionManager.WPF
{
    public partial class MainWindow : Window
    {
        private readonly MonthlySubscriptionViewModel _viewModel;

        public MainWindow(MonthlySubscriptionViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}