using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bkmarker.Pages.Account;

public class RegisterModel : PageModel
{
    [BindProperty]
    public RegisterRequest? RegisterRequest { get; set; }

    private readonly IRepository _repository;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IRepository repository, ILogger<RegisterModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (RegisterRequest == null || string.IsNullOrWhiteSpace(RegisterRequest.Username))
            return Page();

        if (await DoesAccountExist(RegisterRequest.Username))
            return Page();

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(RegisterRequest.Password);
        await _repository.WithConnection(async con =>
        {
            await con.ExecuteAsync(@"insert into users (email, password, is_admin)
                values (@email, @password, @isAdmin)",
                new
                {
                    email = RegisterRequest!.Username,
                    password = passwordHash,
                    isAdmin = false
                });
        });

        return Redirect("./Login");
    }

    private async Task<bool> DoesAccountExist(string username)
    {
        var result = await _repository.WithConnection(async con =>
            await con.ExecuteScalarAsync<int>("select count(id) from users where email = @email", new { email = username }));
        _logger.LogInformation("Found [{UserCount}] users", result);

        return result > 0;
    }
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
