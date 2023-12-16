using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bkmarker.Pages;

public class LoginModel : PageModel
{
    [BindProperty]
    public LoginRequest? LoginRequest { get; set; }

    private readonly ILogger<LoginModel> _logger;

    public LoginModel(ILogger<LoginModel> logger) => _logger = logger;

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("User is logging in");

        var claims = new List<Claim>()
        {
            new Claim("user", LoginRequest!.Username),
            new Claim("role", "member")
        };

        await HttpContext.SignInAsync(
                new ClaimsPrincipal(
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "user", "role")));
        return Redirect("./SecretPage");
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
