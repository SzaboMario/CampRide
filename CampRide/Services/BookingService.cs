using CampRide.Data;
using CampRide.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampRide.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCaravanAvailableAsync(
            int caravanId,
            DateTime from,
            DateTime to)
        {
            return !await _context.Bookings.AnyAsync(b =>
                b.CaravanId == caravanId &&
                b.Status != BookingStatus.Cancelled &&
                b.FromDate < to &&
                b.ToDate > from
            );
        }

        public async Task<BookingEntity> CreateBookingAsync(BookingEntity booking)
        {
            var available = await IsCaravanAvailableAsync(
            booking.CaravanId,
            booking.FromDate,
            booking.ToDate);

            if(!available)
                throw new InvalidOperationException("A lakóautó ebben az időszakban már foglalt.");

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }
    }

}
