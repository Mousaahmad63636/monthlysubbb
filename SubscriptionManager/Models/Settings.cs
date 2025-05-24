using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SubscriptionManager.Models
{
    public class Settings : INotifyPropertyChanged
    {
        private int _id;
        private decimal _defaultPricePerUnit = 1.0m;
        private string _companyName = "Subscription Manager";
        private string _adminEmail = string.Empty;
        private bool _autoCalculateMonthlyFees = true;
        private int _billingDay = 1; 

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public decimal DefaultPricePerUnit
        {
            get => _defaultPricePerUnit;
            set => SetProperty(ref _defaultPricePerUnit, value);
        }

        public string CompanyName
        {
            get => _companyName;
            set => SetProperty(ref _companyName, value);
        }

        public string AdminEmail
        {
            get => _adminEmail;
            set => SetProperty(ref _adminEmail, value);
        }

        public bool AutoCalculateMonthlyFees
        {
            get => _autoCalculateMonthlyFees;
            set => SetProperty(ref _autoCalculateMonthlyFees, value);
        }

        public int BillingDay
        {
            get => _billingDay;
            set => SetProperty(ref _billingDay, value);
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