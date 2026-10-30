using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Events;
using GameNewsHub.Contracts.Games;
using GameNewsHub.Contracts.Genres;
using GameNewsHub.Contracts.Platforms;
using GameNewsHub.Contracts.User;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Mappings;

public static class ProfileMappingExtension
{
    public static ProfileDto ToDto(this AppUser user, bool isAdmin, SettingsDto settings)
    {
        return new ProfileDto
        {
            Id = user.Id,
            Username = user.UserName!,
            CreatedAt = user.CreatedAt,
            Settings = settings,
            IsAdmin = isAdmin,
            SteamId = user.SteamId
        };
    }

    public static FullProfileDto ToFullDto(
        this AppUser user,
        List<EventListNameDto> events,
        List<GameListItemDto> games,
        List<GenreDto> genres,
        List<PlatformGroupDto> platformGroups,
        SettingsDto settings)
    {
        return new FullProfileDto
        {
            Id = user.Id,
            CreatedAt = user.CreatedAt,
            Events = events,
            Games = games,
            Genres = genres,
            PlatformGroups = platformGroups,
            Settings = settings,
            SteamId = user.SteamId
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