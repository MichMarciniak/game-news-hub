using backend.Services.Background;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{

    private readonly IgdbAuthService _authService;

    public TestController(IgdbAuthService service)
    {
        _authService = service;
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult Test()
    {
        return Ok();
    }

    [Authorize]
    [HttpGet("twitch")]
    public async Task<IActionResult> GetTwitchToken()
    {
        var token = await _authService.GetAccessTokenAsync();
        return Ok(new
        {
            message = "This is just for testing. Delete it later",
            token = token,
        });
    }
    
}