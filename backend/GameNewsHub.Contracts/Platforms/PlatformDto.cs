namespace GameNewsHub.Contracts.Platforms;

public class PlatformGroupWithPlatformsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<PlatformDto> Platforms { get; set; } = [];
}

public class PlatformGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PlatformDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}