
using BCrypt.Net;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bkmarker.Pages.Account;

public class RegisterModel : PageModel
{
    [BindProperty]
    public RegisterRequest? RegisterRequest { get; set; }

    private readonly IRepository _repository;

    public RegisterModel(IRepository repository) => _repository = repository;

    public async Task<IActionResult> OnPostAsync()
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(RegisterRequest?.Password);
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
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
