namespace BookStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public Payment Payment { get; set; }
        public Shipment Shipment { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
