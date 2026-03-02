namespace CampRide.Data.Entities
{
    public class CaravanImageEntity : BaseEntity
    {
        public int Id { get; set; }

        public int CaravanId { get; set; }
        public CaravanEntity Caravan { get; set; } = null!;

        public string FileName { get; set; } = "";

        public bool IsPrimary { get; set; } = false;

        public int SortOrder { get; set; }
    }
}
