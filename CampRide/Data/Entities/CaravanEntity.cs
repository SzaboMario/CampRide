namespace CampRide.Data.Entities
{
    public class CaravanEntity : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Capacity { get; set; }
        public List<CaravanImageEntity> Images { get; set; } = new();

    }
}
