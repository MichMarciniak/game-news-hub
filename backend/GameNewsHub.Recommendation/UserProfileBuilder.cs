using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Recommendation;

public class UserProfileBuilder
{
    private readonly AppDbContext _context;

    // sila sygnalu dla samej gry
    private const float FollowedGameWeight = 0.8f;
    private const float WishlistedGameWeight = 1.0f;
    private const float FollowedGenreWeight = 0.5f;
    private const float FollowedPlatformGroupWeight = 0.4f;
    private const float FollowedEventWeight = 0.3f;

    // sila sygnalu, z jaka gra przekazuje swoje gatunki i platformy do profilu
    // (wishlista = intencja bez zaangazowania, wiec nizej niz follow)
    private const float FollowedGameFeatureWeight = 0.8f;
    private const float WishlistedGameFeatureWeight = 0.5f;

    // jaka czesc sily sygnalu trafia do danego faseta
    private const float GenreFromGameFactor = 0.5f;
    private const float PlatformFromGameFactor = 0.3f;

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
            .AsSplitQuery()
            .Include(u => u.FollowedGames).ThenInclude(g => g.Genres)
            .Include(u => u.FollowedGames).ThenInclude(g => g.Platforms)
            .Include(u => u.WishlistedGames).ThenInclude(g => g.Genres)
            .Include(u => u.WishlistedGames).ThenInclude(g => g.Platforms)
            .Include(u => u.FollowedGenres)
            .Include(u => u.FollowedPlatformGroups)
            .Include(u => u.FollowedEvents)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            return new UserProfileVector { UserId = userId, Alpha = 0.0f };
        }

        var profile = new UserProfileVector { UserId = userId };
        int totalSignalsCount = 0;

        totalSignalsCount += AddGames(profile, user.FollowedGames, FollowedGameWeight, FollowedGameFeatureWeight);
        totalSignalsCount += AddGames(profile, user.WishlistedGames, WishlistedGameWeight, WishlistedGameFeatureWeight);
        totalSignalsCount += AddGenreWeight(profile, user.FollowedGenres);
        totalSignalsCount += AddPlatformGroupWeight(profile, user.FollowedPlatformGroups);
        totalSignalsCount += AddSeriesWeight(profile, user.FollowedEvents);

        profile.Alpha = Math.Min(1.0f, totalSignalsCount / SignalSaturationThreshold);

        return profile;
    }

    /// <summary>
    /// Dodaje gry do profilu: wage samej gry oraz (z mniejsza sila) jej gatunki i grupy platform.
    /// Uzywane zarowno dla gier sledzonych, jak i z wishlisty.
    /// </summary>
    /// <returns>Liczba sygnalow (jedna na gre)</returns>
    private static int AddGames(
        UserProfileVector profile,
        ICollection<Game> games,
        float gameWeight,
        float featureWeight)
    {
        var signals = 0;
        foreach (var game in games)
        {
            AddWeight(profile.GameWeights, game.Id, gameWeight);
            AddGameFeatures(profile, game, featureWeight);
            signals++;
        }

        return signals;
    }

    private static void AddGameFeatures(UserProfileVector profile, Game game, float featureWeight)
    {
        foreach (var genre in game.Genres)
        {
            AddWeight(profile.GenreWeights, genre.Id, featureWeight * GenreFromGameFactor);
        }

        // Distinct: gra na PS4 i PS5 to jedna grupa platform, nie dwie
        foreach (var platformGroupId in game.Platforms.Select(p => p.PlatformGroupId).Distinct())
        {
            AddWeight(profile.PlatformGroupWeights, platformGroupId, featureWeight * PlatformFromGameFactor);
        }
    }

    private static int AddGenreWeight(UserProfileVector profile, ICollection<Genre> genres)
    {
        var signal = 0;
        foreach (var genre in genres)
        {
            AddWeight(profile.GenreWeights, genre.Id, FollowedGenreWeight);
            signal++;
        }

        return signal;
    }

    private static int AddPlatformGroupWeight(UserProfileVector profile, ICollection<PlatformGroup> groups)
    {
        var signal = 0;
        foreach (var platformGroup in groups)
        {
            AddWeight(profile.PlatformGroupWeights, platformGroup.Id, FollowedPlatformGroupWeight);
            signal++;
        }

        return signal;
    }

    private static int AddSeriesWeight(UserProfileVector profile, ICollection<Event> events)
    {
        var signal = 0;
        foreach (var evt in events)
        {
            AddWeight(profile.SeriesWeights, evt.EventSeriesId, FollowedEventWeight);
            signal++;
        }

        return signal;
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