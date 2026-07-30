using backend.Data;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.UserInterest.Calculator;

public class UserWeightService : IUserWeightService
{
    private readonly AppDbContext _context;
    private readonly UserGenreWeightCalculator _calculator;

    public UserWeightService(AppDbContext context, UserGenreWeightCalculator calculator)
    {
        _context = context;
        _calculator = calculator;
    }

    public async Task UpdateUserWeightsAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedGenres)
            .Include(u => u.FollowedGames)
            .ThenInclude(g => g.Genres)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return;

        var weightDict = await _calculator.CalculateWeight(user);

        var oldWeights = await _context.UserGenreWeights
            .Where(w => w.UserId == userId)
            .ToListAsync();
        
        _context.UserGenreWeights.RemoveRange(oldWeights);

        var newWeights = weightDict.Select(x => new UserGenreWeight
        {
            UserId = userId,
            GenreId = x.Key,
            Weight = x.Value
        });

        await _context.UserGenreWeights.AddRangeAsync(newWeights);
        await _context.SaveChangesAsync();
    }
}