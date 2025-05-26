using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SubscriptionManager.Models;

namespace SubscriptionManager.Models
{
    /// <summary>
    /// Data model for invoice printing and PDF generation
    /// </summary>
    public class InvoiceData : INotifyPropertyChanged
    {
        private string _companyName = string.Empty;
        private string _companyAddress = string.Empty;
        private string _companyContact = string.Empty;
        private string _companyPhone = string.Empty;
        private string _companyEmail = string.Empty;
        private string _invoiceNumber = string.Empty;
        private DateTime _invoiceDate = DateTime.Now;
        private DateTime _dueDate = DateTime.Now.AddDays(30);
        private string _customerName = string.Empty;
        private string _customerPhone = string.Empty;
        private string _customerAddress = string.Empty;
        private string _accountNumber = string.Empty;
        private DateTime _servicePeriodStart;
        private DateTime _servicePeriodEnd;
        private string _subscriptionType = string.Empty;
        private decimal _previousReading;
        private DateTime _previousReadingDate;
        private decimal _currentReading;
        private DateTime _currentReadingDate;
        private decimal _usageAmount;
        private decimal _pricePerUnit;
        private decimal _usageCharges;
        private decimal _monthlyFee;
        private decimal _subtotal;
        private decimal _taxAmount;
        private decimal _totalAmount;
        private bool _showDetailedHistory;
        private ObservableCollection<CounterHistory> _readingHistory;

        public InvoiceData()
        {
            _readingHistory = new ObservableCollection<CounterHistory>();
        }

        #region Company Information

        public string CompanyName
        {
            get => _companyName;
            set => SetProperty(ref _companyName, value);
        }

        public string CompanyAddress
        {
            get => _companyAddress;
            set => SetProperty(ref _companyAddress, value);
        }

        public string CompanyContact
        {
            get => _companyContact;
            set => SetProperty(ref _companyContact, value);
        }

        public string CompanyPhone
        {
            get => _companyPhone;
            set => SetProperty(ref _companyPhone, value);
        }

        public string CompanyEmail
        {
            get => _companyEmail;
            set => SetProperty(ref _companyEmail, value);
        }

        #endregion

        #region Invoice Details

        public string InvoiceNumber
        {
            get => _invoiceNumber;
            set => SetProperty(ref _invoiceNumber, value);
        }

        public DateTime InvoiceDate
        {
            get => _invoiceDate;
            set => SetProperty(ref _invoiceDate, value);
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        #endregion

        #region Customer Information

        public string CustomerName
        {
            get => _customerName;
            set => SetProperty(ref _customerName, value);
        }

        public string CustomerPhone
        {
            get => _customerPhone;
            set => SetProperty(ref _customerPhone, value);
        }

        public string CustomerAddress
        {
            get => _customerAddress;
            set => SetProperty(ref _customerAddress, value);
        }

        public string AccountNumber
        {
            get => _accountNumber;
            set => SetProperty(ref _accountNumber, value);
        }

        #endregion

        #region Service Information

        public DateTime ServicePeriodStart
        {
            get => _servicePeriodStart;
            set => SetProperty(ref _servicePeriodStart, value);
        }

        public DateTime ServicePeriodEnd
        {
            get => _servicePeriodEnd;
            set => SetProperty(ref _servicePeriodEnd, value);
        }

        public string SubscriptionType
        {
            get => _subscriptionType;
            set => SetProperty(ref _subscriptionType, value);
        }

        #endregion

        #region Meter Reading Information

        public decimal PreviousReading
        {
            get => _previousReading;
            set => SetProperty(ref _previousReading, value);
        }

        public DateTime PreviousReadingDate
        {
            get => _previousReadingDate;
            set => SetProperty(ref _previousReadingDate, value);
        }

        public decimal CurrentReading
        {
            get => _currentReading;
            set => SetProperty(ref _currentReading, value);
        }

        public DateTime CurrentReadingDate
        {
            get => _currentReadingDate;
            set => SetProperty(ref _currentReadingDate, value);
        }

        public decimal UsageAmount
        {
            get => _usageAmount;
            set => SetProperty(ref _usageAmount, value);
        }

        public decimal PricePerUnit
        {
            get => _pricePerUnit;
            set => SetProperty(ref _pricePerUnit, value);
        }

        #endregion

        #region Billing Information

        public decimal UsageCharges
        {
            get => _usageCharges;
            set => SetProperty(ref _usageCharges, value);
        }

        public decimal MonthlyFee
        {
            get => _monthlyFee;
            set => SetProperty(ref _monthlyFee, value);
        }

        public decimal Subtotal
        {
            get => _subtotal;
            set => SetProperty(ref _subtotal, value);
        }

        public decimal TaxAmount
        {
            get => _taxAmount;
            set => SetProperty(ref _taxAmount, value);
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
        }

        #endregion

        #region Display Options

        public bool ShowDetailedHistory
        {
            get => _showDetailedHistory;
            set => SetProperty(ref _showDetailedHistory, value);
        }

        public ObservableCollection<CounterHistory> ReadingHistory
        {
            get => _readingHistory;
            set => SetProperty(ref _readingHistory, value);
        }

        #endregion

        #region Calculated Properties

        /// <summary>
        /// Calculates all billing totals based on usage and monthly fees
        /// </summary>
        public void CalculateTotals()
        {
            UsageCharges = UsageAmount * PricePerUnit;
            Subtotal = UsageCharges + MonthlyFee;

            // Apply tax if applicable (you can make this configurable)
            TaxAmount = Subtotal * 0.00m; // No tax for now, but can be configured

            TotalAmount = Subtotal + TaxAmount;
        }

        /// <summary>
        /// Generates a unique invoice number based on date and customer ID
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Generated invoice number</returns>
        public string GenerateInvoiceNumber(int customerId)
        {
            var datePrefix = InvoiceDate.ToString("yyyyMMdd");
            InvoiceNumber = $"INV-{datePrefix}-{customerId:D4}";
            return InvoiceNumber;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

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

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates invoice data from customer and settings
        /// </summary>
        /// <param name="customer">Customer subscription information</param>
        /// <param name="settings">Application settings</param>
        /// <param name="readingHistory">Customer's reading history</param>
        /// <param name="showDetailedHistory">Whether to show detailed reading history</param>
        /// <returns>Populated invoice data</returns>
        public static InvoiceData CreateFromCustomer(
            CustomerSubscription customer,
            Settings settings,
            List<CounterHistory> readingHistory,
            bool showDetailedHistory = true)
        {
            var invoiceData = new InvoiceData
            {
                // Company Information
                CompanyName = settings.CompanyName,
                CompanyAddress = "123 Business Street, Business City, BC 12345", // Make this configurable
                CompanyContact = "Customer Service Department",
                CompanyPhone = "(555) 123-4567", // Make this configurable
                CompanyEmail = settings.AdminEmail,

                // Customer Information
                CustomerName = customer.Name,
                CustomerPhone = customer.PhoneNumber,
                CustomerAddress = "Customer Address Here", // Add address field to CustomerSubscription if needed
                AccountNumber = customer.Id.ToString("D6"),

                // Service Information
                SubscriptionType = customer.SubscriptionTypeName,
                MonthlyFee = customer.MonthlySubscriptionFee,

                // Display Options
                ShowDetailedHistory = showDetailedHistory,
                ReadingHistory = new ObservableCollection<CounterHistory>(readingHistory)
            };

            // Set service period based on latest reading
            if (readingHistory.Any())
            {
                var latestReading = readingHistory.OrderByDescending(r => r.RecordDate).First();
                var previousReading = readingHistory.OrderByDescending(r => r.RecordDate).Skip(1).FirstOrDefault();

                invoiceData.CurrentReading = latestReading.NewCounter;
                invoiceData.CurrentReadingDate = latestReading.RecordDate;
                invoiceData.PricePerUnit = latestReading.PricePerUnit;

                if (previousReading != null)
                {
                    invoiceData.PreviousReading = previousReading.NewCounter;
                    invoiceData.PreviousReadingDate = previousReading.RecordDate;
                    invoiceData.ServicePeriodStart = previousReading.RecordDate;
                }
                else
                {
                    invoiceData.PreviousReading = latestReading.OldCounter;
                    invoiceData.PreviousReadingDate = latestReading.RecordDate.AddMonths(-1);
                    invoiceData.ServicePeriodStart = latestReading.RecordDate.AddMonths(-1);
                }

                invoiceData.ServicePeriodEnd = latestReading.RecordDate;
                invoiceData.UsageAmount = latestReading.UnitsUsed;
            }

            // Generate invoice number and calculate totals
            invoiceData.GenerateInvoiceNumber(customer.Id);
            invoiceData.CalculateTotals();

            return invoiceData;
        }

        /// <summary>
        /// Creates invoice data for a single reading
        /// </summary>
        /// <param name="customer">Customer subscription information</param>
        /// <param name="settings">Application settings</param>
        /// <param name="reading">Specific counter history reading</param>
        /// <returns>Populated invoice data for single reading</returns>
        public static InvoiceData CreateFromSingleReading(
            CustomerSubscription customer,
            Settings settings,
            CounterHistory reading)
        {
            var invoiceData = CreateFromCustomer(customer, settings, new List<CounterHistory> { reading }, false);

            // Override with single reading data
            invoiceData.PreviousReading = reading.OldCounter;
            invoiceData.CurrentReading = reading.NewCounter;
            invoiceData.CurrentReadingDate = reading.RecordDate;
            invoiceData.PreviousReadingDate = reading.RecordDate.AddDays(-30); // Approximate
            invoiceData.UsageAmount = reading.UnitsUsed;
            invoiceData.PricePerUnit = reading.PricePerUnit;
            invoiceData.ServicePeriodStart = reading.RecordDate.AddDays(-30);
            invoiceData.ServicePeriodEnd = reading.RecordDate;

            // Recalculate totals
            invoiceData.CalculateTotals();

            return invoiceData;
        }

        #endregion
    }
}