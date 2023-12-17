using Dapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
          await con.QueryAsync<User>(@"SELECT id, email, password, is_admin from users
            where email = @email", new { email = LoginRequest!.Username })
        );

        if (!users.Any() || users.Count() > 1)
        {
            _logger.LogInformation("No user was found [{UserCount}]", users.Count());
            return Page();
        }

        var user = users.First();
        var passwordCheck = BCrypt.Net.BCrypt.Verify(LoginRequest!.Password, user.Password);
        if (!passwordCheck)
        {
            _logger.LogInformation("Password is not correct");
            return Page();
        }

        _logger.LogInformation("User is logging in");
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, LoginRequest!.Username),
            new Claim(ClaimTypes.Role, "member")
        };

        await HttpContext.SignInAsync(
                new ClaimsPrincipal(
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "user", "role")));

        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return Redirect("~/Index");
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
}
