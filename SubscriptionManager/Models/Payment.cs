using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SubscriptionManager.Models
{
    public class Payment : INotifyPropertyChanged
    {
        private int _id;
        private int _customerSubscriptionId;
        private decimal _usageBillAmount;
        private decimal _monthlySubscriptionAmount;
        private decimal _totalBillAmount;
        private decimal _amountPaid;
        private decimal _remainingBalance;
        private DateTime _paymentDate = DateTime.Now;
        private DateTime _billDate;
        private string _paymentMethod = "Cash";
        private string _notes = string.Empty;
        private string _receiptNumber = string.Empty;
        private bool _isFullyPaid;
        private PaymentStatus _status = PaymentStatus.Pending;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public int CustomerSubscriptionId
        {
            get => _customerSubscriptionId;
            set => SetProperty(ref _customerSubscriptionId, value);
        }

        public decimal UsageBillAmount
        {
            get => _usageBillAmount;
            set => SetProperty(ref _usageBillAmount, value);
        }

        public decimal MonthlySubscriptionAmount
        {
            get => _monthlySubscriptionAmount;
            set => SetProperty(ref _monthlySubscriptionAmount, value);
        }

        public decimal TotalBillAmount
        {
            get => _totalBillAmount;
            set
            {
                if (SetProperty(ref _totalBillAmount, value))
                {
                    CalculateRemainingBalance();
                }
            }
        }

        public decimal AmountPaid
        {
            get => _amountPaid;
            set
            {
                if (SetProperty(ref _amountPaid, value))
                {
                    CalculateRemainingBalance();
                }
            }
        }

        public decimal RemainingBalance
        {
            get => _remainingBalance;
            set => SetProperty(ref _remainingBalance, value);
        }

        public DateTime PaymentDate
        {
            get => _paymentDate;
            set => SetProperty(ref _paymentDate, value);
        }

        public DateTime BillDate
        {
            get => _billDate;
            set => SetProperty(ref _billDate, value);
        }

        public string PaymentMethod
        {
            get => _paymentMethod;
            set => SetProperty(ref _paymentMethod, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public string ReceiptNumber
        {
            get => _receiptNumber;
            set => SetProperty(ref _receiptNumber, value);
        }

        public bool IsFullyPaid
        {
            get => _isFullyPaid;
            set => SetProperty(ref _isFullyPaid, value);
        }

        public PaymentStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual CustomerSubscription? CustomerSubscription { get; set; }

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

        private void CalculateRemainingBalance()
        {
            RemainingBalance = TotalBillAmount - AmountPaid;
            IsFullyPaid = RemainingBalance <= 0;

            // Update status based on payment
            if (IsFullyPaid)
            {
                Status = PaymentStatus.Paid;
            }
            else if (AmountPaid > 0)
            {
                Status = PaymentStatus.PartiallyPaid;
            }
            else
            {
                Status = PaymentStatus.Pending;
            }
        }

        public void CalculateTotalBill()
        {
            TotalBillAmount = UsageBillAmount + MonthlySubscriptionAmount;
        }
    }

    public enum PaymentStatus
    {
        Pending,
        PartiallyPaid,
        Paid,
        Overdue
    }
}