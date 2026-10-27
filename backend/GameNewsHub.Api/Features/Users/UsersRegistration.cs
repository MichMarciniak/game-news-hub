using GameNewsHub.Api.Features.Users.Preferences;
using GameNewsHub.Api.Features.Users.Profile;
using GameNewsHub.Api.Features.Users.Wishlist;

namespace GameNewsHub.Api.Features.Users;

public static class UsersRegistration
{
    public static IServiceCollection AddUsersServices(this IServiceCollection services)
    {
        services.AddScoped<UserProfileService>();
        services.AddScoped<UserWishlistService>();
        services.AddScoped<UserPreferencesService>();

        return services;
    }
}