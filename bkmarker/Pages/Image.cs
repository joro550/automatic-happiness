using HtmlAgilityPack;

namespace bkmarker.Pages;

public record Image(int Size, string Url)
{
  public static Image Parse(HtmlNode node)
  {
    var size = node.GetAttributeValue("sizes", "0")
      .Split("x")
      .Select(int.Parse)
      .Aggregate(0, (l, r) => l + r);

    var url = node.GetAttributeValue("href", "");

    return new Image(size, url);
  }
}

