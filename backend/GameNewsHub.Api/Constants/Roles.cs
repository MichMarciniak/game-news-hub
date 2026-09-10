namespace GameNewsHub.Api.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";


    public static string[] GetArray()
    {
        var roles =  new[] { Admin, User };
        return roles;
    }
}