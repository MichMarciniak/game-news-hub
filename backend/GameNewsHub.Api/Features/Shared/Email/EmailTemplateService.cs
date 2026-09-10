using backend.Configuration;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Auth;

public class EmailTemplateService
{

    private readonly EmailTemplateOptions _options;


    public EmailTemplateService(IOptions<EmailTemplateOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> RenderAsync(string templatePath, object model)
    {
        var path = Path.Combine(AppContext.BaseDirectory, _options.BasePath, templatePath);
        var raw = await File.ReadAllTextAsync(path);
        var template = Scriban.Template.Parse(raw);
        return await template.RenderAsync(model);
    }

}