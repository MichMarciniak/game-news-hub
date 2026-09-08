using backend.Data;
using GameNewsHub.Contracts;

namespace GameNewsHub.Api.Features.Series;

public class SeriesService
{
    private readonly AppDbContext _context;
    
    public SeriesService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SeriesDto>> Get()
    {
        throw new NotImplementedException();
    }
}