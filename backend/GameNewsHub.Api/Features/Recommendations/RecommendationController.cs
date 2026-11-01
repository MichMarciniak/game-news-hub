using GameNewsHub.Recommendation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Recommendations;

[ApiController]
[Route("[controller]")]
[Authorize]
public class RecommendationController : ControllerBase
{


}