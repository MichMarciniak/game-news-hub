namespace GameNewsHub.Contracts;

public class PlatformGroupWithPlatformsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<PlatformDto> Platforms { get; set; } = new();
}

public class PlatformGroupDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}

public class PlatformDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}