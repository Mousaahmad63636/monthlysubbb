using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SubscriptionManager.Models
{
    public class CustomerSubscription : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _phoneNumber = string.Empty;
        private decimal _oldCounter;
        private decimal _newCounter;
        private decimal _billAmount;
        private DateTime _lastBillDate;
        private decimal _pricePerUnit = 1;
        private bool _isActive = true;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public decimal OldCounter
        {
            get => _oldCounter;
            set => SetProperty(ref _oldCounter, value);
        }

        public decimal NewCounter
        {
            get => _newCounter;
            set => SetProperty(ref _newCounter, value);
        }

        public decimal BillAmount
        {
            get => _billAmount;
            set => SetProperty(ref _billAmount, value);
        }

        public DateTime LastBillDate
        {
            get => _lastBillDate;
            set => SetProperty(ref _lastBillDate, value);
        }

        public decimal PricePerUnit
        {
            get => _pricePerUnit;
            set => SetProperty(ref _pricePerUnit, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

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