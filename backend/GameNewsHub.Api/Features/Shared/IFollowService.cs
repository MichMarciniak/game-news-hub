using ErrorOr;

namespace GameNewsHub.Api.Features.Shared;

public interface IFollowService<TEntity> where TEntity : class
{
    Task<ErrorOr<Success>> Follow(int userId, int entityId);
    Task<ErrorOr<Success>> Unfollow(int userId, int entityId);
}