using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bkmarker.Pages;

[Authorize]
public class SecretPageModel : PageModel
{
    private readonly ILogger<SecretPageModel> _loggger;

    public SecretPageModel(ILogger<SecretPageModel> loggger) => _loggger = loggger;

    public void OnGet()
    {
        var user = HttpContext.User;
        _loggger.LogInformation("Got to the secret page [User == null {User}]", user == null);
    }

}
