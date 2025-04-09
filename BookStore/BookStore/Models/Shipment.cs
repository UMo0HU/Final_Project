namespace BookStore.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
    }
}
