namespace backend.Services.Background;

public class IgdbClient
{
    private readonly HttpClient _httpClient;
    private readonly IgdbAuthService _authService;

    public IgdbClient(HttpClient httpClient, IgdbAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }
    
}