using Dapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bkmarker.Pages;

public class CreateModel : PageModel
{
    [BindProperty]
    public CreateBookmarkModel? Bookmark { get; set; }

    private readonly IRepository _repository;
    private readonly HttpClient _httpClient;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(ILogger<CreateModel> logger,
        IRepository repository,
        IHttpClientFactory httpClient)
    {
        _logger = logger;
        _repository = repository;
        _httpClient = httpClient.CreateClient();
    }

    public void OnGet()
    {
        _logger.LogInformation("OnGet");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Bookmark == null)
            return Page();

        if (!Uri.TryCreate(Bookmark.Url, UriKind.Absolute, out var uri))
            return Page();

        var response = await _httpClient.GetAsync(Bookmark!.Url);
        if (!response.IsSuccessStatusCode)
            return Page();

        var doc = new HtmlDocument();
        doc.LoadHtml(await response.Content.ReadAsStringAsync());

        var titleNode = doc.DocumentNode.SelectSingleNode("//title");
        var article = doc.DocumentNode.SelectSingleNode("//html/head/meta[@name='description']")
            .GetAttributeValue("content", "");

        var favicons = doc.DocumentNode.SelectNodes("//html/head/link[@rel='icon']")
          .Select(Image.Parse)
          .MaxBy(x => x.Size);


        _logger.LogInformation("Has Favicon [{Favicon}]", favicons != null);

        await _repository.WithConnection(async con =>
        {
            await con.ExecuteAsync(
               @"insert into bookmarks (url, content, image_url, image_alt_text, title)
               values (@url, @content, @imageUrl, @title)",
               new
               {
                   url = Bookmark!.Url,
                   content = article ?? "",
                   imageUrl = favicons?.Url ?? "",
                   title = titleNode.InnerText
               });
        });

        return RedirectToPage("./Index");
    }
}
