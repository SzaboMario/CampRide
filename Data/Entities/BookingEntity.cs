namespace CampRide.Data.Entities
{
    public class BookingEntity : BaseEntity
    {
        public int Id { get; set; }
        public int CaravanId { get; set; }
        public CaravanEntity Caravan { get; set; } = null!;

        public string? UserId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string CustomerName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Notes { get; set; } = "";

        public BookingStatus Status { get; set; }
    }

    public enum BookingStatus
    {
        Pending,
        Approved,
        Cancelled
    }
}
