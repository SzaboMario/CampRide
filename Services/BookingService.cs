using CampRide.Data;
using CampRide.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampRide.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public BookingService(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<List<(DateTime From, DateTime To)>> GetBookedRangesAsync(int caravanId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.CaravanId == caravanId && b.Status != BookingStatus.Cancelled)
                .Select(b => new { b.FromDate, b.ToDate })
                .ToListAsync();

            return bookings.Select(b => (b.FromDate, b.ToDate)).ToList();
        }

        public async Task<bool> IsCaravanAvailableAsync(int caravanId, DateTime from, DateTime to)
        {
            return !await _context.Bookings.AnyAsync(b =>
                b.CaravanId == caravanId &&
                b.Status != BookingStatus.Cancelled &&
                b.FromDate <= to &&
                b.ToDate >= from
            );
        }

        public async Task<BookingEntity> CreateBookingAsync(BookingEntity booking)
        {
            var available = await IsCaravanAvailableAsync(
                booking.CaravanId,
                booking.FromDate,
                booking.ToDate);

            if (!available)
                throw new InvalidOperationException("A lakóautó ebben az időszakban már foglalt.");

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }

        public async Task<List<BookingEntity>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Caravan)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<BookingEntity>> GetUserBookingsAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Caravan)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task ApproveAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Caravan)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking != null)
            {
                booking.Status = BookingStatus.Approved;
                await _context.SaveChangesAsync();

                await _emailService.SendBookingApprovedAsync(
                    booking.Email,
                    booking.CustomerName,
                    booking.Caravan?.Name ?? "Lakóautó",
                    booking.FromDate,
                    booking.ToDate);
            }
        }

        public async Task CancelAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Caravan)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking != null)
            {
                booking.Status = BookingStatus.Cancelled;
                await _context.SaveChangesAsync();

                await _emailService.SendBookingCancelledAsync(
                    booking.Email,
                    booking.CustomerName,
                    booking.Caravan?.Name ?? "Lakóautó",
                    booking.FromDate,
                    booking.ToDate);
            }
        }

        /// <summary>
        /// Vendégként leadott foglalásokat hozzáköti a bejelentkezett felhasználóhoz email alapján.
        /// Meghívandó bejelentkezés után (pl. MyBookings oldal betöltésekor).
        /// </summary>
        public async Task LinkGuestBookingsAsync(string userId, string email)
        {
            var guestBookings = await _context.Bookings
                .Where(b => b.UserId == null && b.Email == email)
                .ToListAsync();

            if (guestBookings.Count == 0) return;

            foreach (var b in guestBookings)
                b.UserId = userId;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Ügyfél saját foglalásának lemondása – csak Pending státuszban, csak saját foglalás
        /// </summary>
        public async Task<bool> CancelByUserAsync(int id, string userId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Caravan)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return false;

            if (booking.Status != BookingStatus.Pending)
                return false;

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();

            await _emailService.SendBookingCancelledAsync(
                booking.Email,
                booking.CustomerName,
                booking.Caravan?.Name ?? "Lakóautó",
                booking.FromDate,
                booking.ToDate);

            return true;
        }
    }
}
