using ErrorOr;
using GameNewsHub.Api.Features.Games;
using GameNewsHub.Api.Mappings;
using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Games;
using GameNewsHub.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Users.Wishlist;

public class UserWishlistService
{
    private readonly AppDbContext _context;

    public UserWishlistService(
        AppDbContext context
    )
    {
        _context = context;
    }
    
    public async Task<ErrorOr<List<GameListItemDto>>> GetWishlist(int userId)
    {
        var wishlistedGames = await _context.Users
            .SelectMany(u => u.WishlistedGames)
            .Select(g => g.ToListItemDto())
            .ToListAsync();

        return wishlistedGames;
    }

    public async Task<ErrorOr<Success>> AddGameToWishlist(int userId, int gameId)
    {
        var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);
        if (game == null)
        {
            return Error.NotFound("Game.NotFound", "Game not found");
        }

        var user = await _context.Users
            .Include(u => u.WishlistedGames)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Error.NotFound("User.NotFound", "User not found");
        }
        
        user.WishlistedGames.Add(game);
        await _context.SaveChangesAsync();
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> RemoveGameFromWishlist(int userId, int gameId)
    {
        var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);
        if (game == null)
        {
            return Error.NotFound("Game.NotFound", "Game not found");
        }

        var user = await _context.Users
            .Include(u => u.WishlistedGames)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Error.NotFound("User.NotFound", "User not found");
        }

        user.WishlistedGames.Remove(game);
        await _context.SaveChangesAsync();
        return Result.Success;
    }
}