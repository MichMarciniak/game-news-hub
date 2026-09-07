namespace GameNewsHub.Contracts;

public class PlatformGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<PlatformDto> Platforms { get; set; } = new List<PlatformDto>();
}

public class PlatformDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}