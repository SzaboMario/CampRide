using CampRide.Data;
using CampRide.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampRide.Services
{
    public class CaravanService
    {
        private readonly AppDbContext _context;

        public CaravanService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CaravanEntity>> GetAllAsync()
        {
            return await _context.Caravans
                .Include(c => c.Images)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<CaravanEntity>> GetAvailableAsync(DateTime? from, DateTime? to, int? minCapacity)
        {
            var query = _context.Caravans
                .Include(c => c.Images)
                .AsQueryable();

            if (minCapacity.HasValue && minCapacity > 0)
                query = query.Where(c => c.Capacity >= minCapacity.Value);

            var all = await query.OrderBy(c => c.Name).ToListAsync();

            if (!from.HasValue || !to.HasValue)
                return all;

            // Foglalt lakóautók ID-jai ebben az időszakban
            var bookedIds = await _context.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled &&
                            b.FromDate <= to.Value && b.ToDate >= from.Value)
                .Select(b => b.CaravanId)
                .Distinct()
                .ToListAsync();

            return all.Where(c => !bookedIds.Contains(c.Id)).ToList();
        }


        public async Task<CaravanEntity?> GetByIdAsync(int id)
        {
            return await _context.Caravans
               .Include(c => c.Images)
               .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateAsync(CaravanEntity caravan)
        {
            _context.Caravans.Add(caravan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CaravanEntity caravan)
        {
            var existing = await _context.Caravans.FindAsync(caravan.Id);
            if(existing == null)
                return;

            existing.Name = caravan.Name;
            existing.Description = caravan.Description;
            existing.Capacity = caravan.Capacity;
            existing.PricePerDay = caravan.PricePerDay;

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var caravan = await _context.Caravans.FindAsync(id);
            if(caravan == null)
                return;

            caravan.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task AddImageAsync(CaravanImageEntity image)
        {
            _context.CaravanImages.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task SetPrimaryImageAsync(int imageId)
        {
            var image = await _context.CaravanImages.FindAsync(imageId);
            if(image == null)
                return;

            var allImages = await _context.CaravanImages
                .Where(i => i.CaravanId == image.CaravanId)
                .ToListAsync();

            foreach(var img in allImages)
                img.IsPrimary = false;

            image.IsPrimary = true;

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteImageAsync(int imageId)
        {
            var image = await _context.CaravanImages.FindAsync(imageId);
            if(image == null)
                return;

            image.IsDeleted = true;
            await _context.SaveChangesAsync();
        }



    }

}
