using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bkmarker.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public List<Bookmark> Bookmarks { get; set; } = new();

    private readonly ILogger<IndexModel> _logger;
    private readonly IRepository _repository;

    public IndexModel(ILogger<IndexModel> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task OnGetAsync()
    {
        var bookmarks = await _repository.WithConnection(async con =>
        {
            return await con.QueryAsync<Bookmark>(@"select 
                  url, title,
                  content, image_url as ImageUrl, 
                  image_alt_text as ImageAltText 
                from bookmarks");
        });

        Bookmarks = bookmarks.ToList();
    }
}

public class Bookmark
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAltText { get; set; }
}
