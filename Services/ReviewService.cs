using CampRide.Data;
using CampRide.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampRide.Services
{
    public class ReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewEntity>> GetByCaravanAsync(int caravanId)
        {
            return await _context.Reviews
                .Where(r => r.CaravanId == caravanId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task AddReviewAsync(ReviewEntity review)
        {
            review.CreatedAt = DateTime.UtcNow;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<double> GetAverageRatingAsync(int caravanId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.CaravanId == caravanId)
                .Select(r => r.Rating)
                .ToListAsync();

            if (!ratings.Any()) return 0;
            return ratings.Average();
        }

        public async Task<bool> HasUserReviewedAsync(int caravanId, string userId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.CaravanId == caravanId && r.UserId == userId);
        }
    }
}
