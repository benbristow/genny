using System.Text;
using System.Xml.Linq;

namespace Genny.Services;

public static class SitemapGenerator
{
    private const string NamespaceName = "http://www.sitemaps.org/schemas/sitemap/0.9";

    public static async Task<string?> GenerateSitemapAsync(List<string> pagePaths, string pagesDirectory, string? baseUrl = null)
    {
        if (pagePaths.Count == 0)
        {
            return null;
        }

        var sitemap = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement(XName.Get("urlset", NamespaceName),
                pagePaths.Select(pagePath =>
                {
                    var fullUrl = PageUrlHelper.CalculatePageUrl(pagePath, pagesDirectory, baseUrl);
                    
                    // Calculate urlPath for priority determination
                    var relativePath = Path.GetRelativePath(pagesDirectory, pagePath);
                    string urlPath;
                    if (Path.GetDirectoryName(relativePath) == "." || string.IsNullOrEmpty(Path.GetDirectoryName(relativePath)))
                    {
                        var fileName = Path.GetFileName(pagePath);
                        urlPath = fileName == "index.html" ? "" : fileName;
                    }
                    else
                    {
                        urlPath = relativePath.Replace('\\', '/');
                    }

                    // Get last modified time from file
                    var lastModified = File.GetLastWriteTimeUtc(pagePath);

                    return new XElement(XName.Get("url", NamespaceName),
                        new XElement(XName.Get("loc", NamespaceName), fullUrl),
                        new XElement(XName.Get("lastmod", NamespaceName), lastModified.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                        new XElement(XName.Get("changefreq", NamespaceName), "monthly"),
                        new XElement(XName.Get("priority", NamespaceName), urlPath == "" ? "1.0" : "0.8")
                    );
                })
            )
        );

        var settings = new System.Xml.XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            Encoding = Encoding.UTF8,
            Async = true
        };

        await using var memoryStream = new MemoryStream();
        await using var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(false));
        await using var writer = System.Xml.XmlWriter.Create(streamWriter, settings);
        await sitemap.SaveAsync(writer, CancellationToken.None);
        await writer.FlushAsync();
        await streamWriter.FlushAsync();
        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}
