namespace GameNewsHub.Hosting.Configuration;

public interface IConfigSection
{
    static abstract string SectionName { get; }
}