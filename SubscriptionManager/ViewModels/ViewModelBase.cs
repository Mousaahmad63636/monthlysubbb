using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SubscriptionManager.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected async Task ShowErrorAsync(string message)
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        protected async Task ShowSuccessAsync(string message)
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }
    }
}