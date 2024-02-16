namespace API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder builder)
    {
        var userGroup = builder.MapGroup("api/users")
            .WithTags("Users");
    }
}