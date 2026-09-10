namespace backend.Configuration;

public class EmailTemplateOptions
{
    public string Section = "EmailTemplates";
    public string BasePath { get; set; } = "Resources/EmailTemplates";
}
