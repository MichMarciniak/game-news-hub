using System.Runtime.CompilerServices;
using backend.Data;
using GameNewsHub.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.UserInterest;

public class UserGenreWeightCalculator
{
   
    public async Task<Dictionary<int, double>> CalculateWeight(AppUser user)
    {
        /*
         * pobierz genres z usera i gier
         * dodaj z gier +1
         * dodaj followed +3
         */
        var followedGenres = user.FollowedGenres;

        var genresFromGames = user.FollowedGames
            .SelectMany(g => g.Genres);

        // obliczanie
        var weights = new Dictionary<int, double>();

        foreach (var genre in genresFromGames)
        {
            if (!weights.ContainsKey(genre.Id)) weights[genre.Id] = 0;
            weights[genre.Id] += 1.0;
        }

        foreach (var genre in followedGenres)
        {
            if (!weights.ContainsKey(genre.Id)) weights[genre.Id] = 0;
            weights[genre.Id] += 3.0;
        }

        if (!weights.Any()) return weights;

        double totalPoints = weights.Values.Sum();

        return weights.ToDictionary(
            x => x.Key,
            x => x.Value / totalPoints);
    }
}