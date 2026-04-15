using GameNewsHub.Api.Services.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class GameController
{
    private readonly IGameService _service;

    public GameController(IGameService service)
    {
        _service = service;
    }

}