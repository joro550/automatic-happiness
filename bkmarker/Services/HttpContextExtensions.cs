using System.Security.Claims;
using bkmarker.Entities;
using Dapper;
using OneOf.Types;
namespace bkmarker.Services;


public static class HttpContextExtensions
{
    public static async Task<OneOf.OneOf<UserEntity, NotFound>> GetLoggedInUser(this HttpContext context, IRepository repository)
    {
        var claims = context.User?.Claims.ToArray() ?? Array.Empty<Claim>();
        var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        if (emailClaim == null)
            return new NotFound();

        var users = await repository.WithConnection(con =>
            con.QuerySingleAsync<UserEntity>(@"select id, email, is_admin
              where email = @email", new { email = emailClaim.Value })
          );
        return users == null ? new NotFound() : users;
    }
}

