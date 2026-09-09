using Microsoft.AspNetCore.Identity;

namespace backend.Configuration;

public class IdentityConfig
{
    //TODO zmienić przez prod albo wyrzucić
    public static void ConfigIdentity(IdentityOptions options)
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 2;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
        
    }
}