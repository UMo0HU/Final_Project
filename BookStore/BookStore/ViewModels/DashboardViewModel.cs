namespace BookStore.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int Users { get; set; }
        public decimal Sales { get; set; }
        public int TotalOrders { get; set; }
        public int CanceledOrders { get; set; }
        public int DeliveredOrders {get; set; }
        public int PendingOrders { get; set; }
    }
}
