using GameNewsHub.Contracts.Events;
using GameNewsHub.Contracts.Games;
using GameNewsHub.Contracts.Genres;
using GameNewsHub.Contracts.Platforms;
using GameNewsHub.Contracts.User;
using GameNewsHub.Data.Entities;
using Riok.Mapperly.Abstractions;
using GameNewsHub.Api.Features.Events;
using GameNewsHub.Api.Features.Games;
using GameNewsHub.Api.Features.Genres;
using GameNewsHub.Api.Features.Platforms;

namespace GameNewsHub.Api.Features.Users.Profile;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(EventMapper))]
[UseStaticMapper(typeof(GameMapper))]
[UseStaticMapper(typeof(GenreMapper))]
[UseStaticMapper(typeof(PlatformMapper))]
public static partial class ProfileMapper
{
    [MapProperty(nameof(AppUser.UserName), nameof(ProfileDto.Username))]
    [MapperIgnoreTarget(nameof(ProfileDto.Settings))]
    [MapperIgnoreTarget(nameof(ProfileDto.IsAdmin))]
    public static partial ProfileDto ToDto(AppUser source);

    [MapProperty(nameof(AppUser.UserName), nameof(FullProfileDto.Username))]
    [MapProperty(nameof(AppUser.FollowedEvents), nameof(FullProfileDto.Events))]
    [MapProperty(nameof(AppUser.FollowedGames), nameof(FullProfileDto.Games))]
    [MapProperty(nameof(AppUser.FollowedGenres), nameof(FullProfileDto.Genres))]
    [MapProperty(nameof(AppUser.FollowedPlatformGroups), nameof(FullProfileDto.PlatformGroups))]
    [MapperIgnoreTarget(nameof(FullProfileDto.Settings))]
    public static partial FullProfileDto ToFullDto(AppUser source);

    public static partial SettingsDto ToSettingsDto(AppUser source);
}