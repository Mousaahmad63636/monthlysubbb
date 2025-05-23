using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SubscriptionManager.Models
{
    public class Expense : INotifyPropertyChanged
    {
        private int _id;
        private string _reason = string.Empty;
        private decimal _amount;
        private DateTime _date = DateTime.Now;
        private string _notes = string.Empty;
        private string _category = string.Empty;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

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
    }
}