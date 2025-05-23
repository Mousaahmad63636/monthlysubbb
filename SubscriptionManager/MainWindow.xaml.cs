using System.Windows;
using SubscriptionManager.ViewModels;

namespace SubscriptionManager
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}