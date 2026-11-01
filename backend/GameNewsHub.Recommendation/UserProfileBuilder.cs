using Data.Migrations;
using GameNewsHub.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Recommendation;

public class UserProfileBuilder
{
    private readonly AppDbContext _context;

    private const float FollowedGameWeight = 0.8f;
    private const float WishlistedGameWeight = 1.0f;
    private const float FollowedGenreWeight = 0.5f;
    private const float FollowedPlatformGroupWeight = 0.4f;

    // ile sygnalow trzeba zeby alpha = 1
    private const float SignalSaturationThreshold = 20.0f;

    public UserProfileBuilder(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<UserProfileVector> BuildProfileAsync(int userId, CancellationToken ct)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.FollowedGames)
            .ThenInclude(g => g.Genres)
            .Include(u => u.FollowedGames)
            .ThenInclude(g => g.Platforms)
            .Include(u => u.WishlistedGames)
            .ThenInclude(g => g.Genres)
            .Include(u => u.FollowedGenres)
            .Include(u => u.FollowedPlatformGroups)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);


        if (user == null)
        {
            return new UserProfileVector { UserId = userId, Alpha = 0.0f };
        }

        var profile = new UserProfileVector { UserId = userId };
        int totalSignalsCount = 0;

        foreach (var game in user.FollowedGames)
        {
            AddWeight(profile.GameWeights, game.Id, FollowedGameWeight);
            totalSignalsCount++;

            foreach (var genre in game.Genres)
            {
                AddWeight(profile.GenreWeights, genre.Id, FollowedGameWeight * 0.5f);
            }

            foreach (var platform in game.Platforms)
            {
                AddWeight(profile.PlatformGroupWeights, platform.PlatformGroupId, FollowedGameWeight * 0.3f);
            }
        }

        foreach (var game in user.WishlistedGames)
        {
            AddWeight(profile.GameWeights, game.Id, WishlistedGameWeight);
            totalSignalsCount++;
        }

        foreach (var genre in user.FollowedGenres)
        {
            AddWeight(profile.GenreWeights, genre.Id, FollowedGenreWeight);
            totalSignalsCount++;
        }

        foreach (var platformGroup in user.FollowedPlatformGroups)
        {
            AddWeight(profile.PlatformGroupWeights, platformGroup.Id, FollowedPlatformGroupWeight);
            totalSignalsCount++;
        }

        profile.Alpha = Math.Min(1.0f, totalSignalsCount / SignalSaturationThreshold);

        return profile;
    }

    private static void AddWeight(Dictionary<int, float> dict, int key, float weight)
    {
        if (dict.TryGetValue(key, out var currWeight))
        {
            dict[key] = currWeight + weight;
        }
        else
        {
            dict[key] = weight;
        }
    }
}