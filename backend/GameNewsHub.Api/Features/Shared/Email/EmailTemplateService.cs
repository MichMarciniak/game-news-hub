using GameNewsHub.Api.Options;
using Microsoft.Extensions.Options;
using Scriban;

namespace GameNewsHub.Api.Features.Shared.Email;

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
        var template = Template.Parse(raw);
        return await template.RenderAsync(model);
    }
}