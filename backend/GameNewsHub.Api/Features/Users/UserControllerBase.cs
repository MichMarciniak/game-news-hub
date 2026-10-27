using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Users;

[ApiController]
[Route("users/me")]
[Authorize]
public abstract class UserControllerBase : ControllerBase
{
    
}