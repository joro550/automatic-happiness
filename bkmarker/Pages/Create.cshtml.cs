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
        var article = doc.DocumentNode.SelectSingleNode("//article");
        var picture = doc.DocumentNode.SelectNodes("//picture")
          .FirstOrDefault();

        _logger.LogInformation("Time for information [Do we have the picture: {Picture}]", picture?.InnerHtml);
        _logger.LogInformation("Time for information [Do we have the article: {Article}]", article != null);

        var source = picture?.GetAttributeValue("src", "");
        var altText = picture?.GetAttributeValue("alt", "");

        _logger.LogInformation("Time for information [Do we have the source: {Source}]", source);
        _logger.LogInformation("Time for information [Do we have the AltText: {AltText}]", altText);

        await _repository.WithConnection(async con =>
        {
            await con.ExecuteAsync(
               @"insert into bookmarks (url, content, image_url, image_alt_text, title)
               values (@url, @content, @imageUrl, @imageAltText, @title)",
               new
               {
                   url = Bookmark!.Url,
                   content = article?.InnerHtml ?? "",
                   imageUrl = source ?? "",
                   imageAltText = altText ?? "",
                   title = titleNode.InnerText
               });
        });

        return RedirectToPage("./Index");
    }
}

public class CreateBookmarkModel
{
    public string Url { get; set; } = string.Empty;
}
