using CampRide.Data;
using CampRide.Data.Entities;

namespace CampRide.Seeder
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if(context.Caravans.Any())
                return;

            context.Caravans.AddRange(
                new CaravanEntity
                {
                    Name = "VW California",
                    Description = "4 személyes, konyhával",
                    Capacity = 4
                },
                new CaravanEntity
                {
                    Name = "Fiat Ducato Camper",
                    Description = "Családi lakóautó",
                    Capacity = 5
                }
            );

            context.SaveChanges();
        }
    }

}
