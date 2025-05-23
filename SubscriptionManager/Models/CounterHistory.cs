namespace SubscriptionManager.Models;
    public class CounterHistory
    {
        public int Id { get; set; }
        public int CustomerSubscriptionId { get; set; }
        public decimal OldCounter { get; set; }
        public decimal NewCounter { get; set; }
        public decimal BillAmount { get; set; }
        public DateTime RecordDate { get; set; }
        public decimal PricePerUnit { get; set; }

        // Calculated property
        public decimal UnitsUsed => NewCounter - OldCounter;

        // Navigation property
        public CustomerSubscription? CustomerSubscription { get; set; }
    }
