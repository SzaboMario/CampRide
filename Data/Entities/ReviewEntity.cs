namespace CampRide.Data.Entities
{
    public class ReviewEntity
    {
        public int Id { get; set; }
        public int CaravanId { get; set; }
        public CaravanEntity? Caravan { get; set; }
        public string UserId { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public int Rating { get; set; } // 1–5
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
