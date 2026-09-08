using GameNewsHub.Api.Features.Platforms;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Mappings;

public static class ProfileMappingExtension
{
    public static ProfileDto ToDto(this AppUser user)
    {
        return new ProfileDto
        {
            Id = user.Id,
            CreatedAt = user.CreatedAt,
            Events = user.FollowedEvents.Select(e => e.ToListNameDto()).ToList(),
            Games = user.FollowedGames.Select(g => g.ToListItemDto()).ToList(),
            Genres = user.FollowedGenres.Select(g => g.ToDto()).ToList(),
            PlatformGroups = user.FollowedPlatformGroups.Select(pg => pg.ToDto()).ToList(),
            Settings = user.ToSettingsDto()
        };
    }

    public static SettingsDto ToSettingsDto(this AppUser user)
    {
        return new SettingsDto
        {
            ShowFutureRecommendations = user.ShowFutureRecommendations
        };
    }
}