using JwtAuthApi.Db;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthApi
{
    internal static class ApiEndpoints
    {
        public record AuthRequest(string email, string password);

        public static void Map(WebApplication app)
        {
            app.MapGet("/users/{email}", Get).RequireAuthorization();

            app.MapPost("/users", Create);

            app.MapPost("/authenticate", Authenticate);

        }

        public static async Task<IResult> Get(string email, AppDbContext context)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);

            return user == null ? Results.NotFound() : Results.Ok(user);
        }

        public static async Task<IResult> Create(User item, AppDbContext context)
        {
            context.Users.Add(item);

            await context.SaveChangesAsync();

            return Results.Created($"/user/{item.Id}", item);
        }

        public static async Task<IResult> Authenticate(AuthRequest request, AppDbContext context, JwtTokenProvider jwtTokenProvider)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == request.email);

            if (user is null) return Results.NotFound();

            var token = jwtTokenProvider.GetToken(user);

            return Results.Ok(token);
        }
    }
}
