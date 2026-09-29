using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Api.Options;

public interface IConfigSection
{
    static abstract string SectionName { get; }
}

public static class OptionsExtensions
{
    public static T AddValidatedOptions<T>(this IServiceCollection services, IConfiguration config)
        where T : class, IConfigSection
    {
        var section = config.GetSection(T.SectionName);

        var options = section.Get<T>()
          ?? throw new InvalidOperationException($"Missing configuration section '{T.SectionName}'");
        
        Validator.ValidateObject(options, new ValidationContext(options), validateAllProperties: true);

        services.Configure<T>(section);
        return options;
    }
}