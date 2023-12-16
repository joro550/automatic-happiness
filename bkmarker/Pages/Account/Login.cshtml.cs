using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bkmarker.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty]
    public LoginRequest? LoginRequest { get; set; }

    private readonly ILogger<LoginModel> _logger;
    private readonly IRepository _repository;

    public LoginModel(ILogger<LoginModel> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<IActionResult> OnPostAsync([FromQuery] string returnUrl)
    {
        var users = await _repository.WithConnection(async con =>
        {
            return await con.QueryAsync<User>(@"SELECT email, password, is_admin from users
                    where email = @email", new { email = LoginRequest!.Username });
        });

        if (!users.Any() || users.Count() > 1)
            return Page();

        var user = users.First();
        var passwordCheck = BCrypt.Net.BCrypt.Verify(LoginRequest!.Password, user.Password);
        if (!passwordCheck)
            return Page();

        _logger.LogInformation("User is logging in");
        var claims = new List<Claim>()
        {
            new Claim("user", LoginRequest!.Username),
            new Claim("role", "member")
        };

        await HttpContext.SignInAsync(
                new ClaimsPrincipal(
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "user", "role")));

        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);

        }
        return Redirect("./Index");
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class User
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
}
