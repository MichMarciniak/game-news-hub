namespace GameNewsHub.External.Igdb.Client;

public static class IgdbFields
{
    public const string DiscoveryEvents = "name, start_time, end_time, description, live_stream_url";
    public const string FullEvents = "name, start_time, end_time, description, live_stream_url, games.id";

    public const string FullGames =
        "name, summary, cover.url, genres.name, platforms.name, category, parent_game, game_type.type, release_dates.date"; //websites = 13 (steam app id)
}