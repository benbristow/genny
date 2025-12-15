using Genny.Services;

namespace Genny.Tests;

public class PageUrlHelperTests
{
    [Fact]
    public void CalculatePageUrl_WithRootIndexHtml_WithoutBaseUrl_ReturnsRootPath()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/index.html";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir);

        // Assert
        result.ShouldBe("/");
    }

    [Fact]
    public void CalculatePageUrl_WithRootIndexHtml_WithBaseUrl_ReturnsBaseUrl()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/index.html";
        const string baseUrl = "https://example.com";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir, baseUrl);

        // Assert
        result.ShouldBe("https://example.com");
    }

    [Fact]
    public void CalculatePageUrl_WithRootPage_WithoutBaseUrl_ReturnsPagePath()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/about.html";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir);

        // Assert
        result.ShouldBe("/about.html");
    }

    [Fact]
    public void CalculatePageUrl_WithRootPage_WithBaseUrl_ReturnsFullUrl()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/about.html";
        const string baseUrl = "https://example.com";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir, baseUrl);

        // Assert
        result.ShouldBe("https://example.com/about.html");
    }

    [Fact]
    public void CalculatePageUrl_WithSubdirectoryPage_WithoutBaseUrl_ReturnsSubdirectoryPath()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/blog/post.html";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir);

        // Assert
        result.ShouldBe("/blog/post.html");
    }

    [Fact]
    public void CalculatePageUrl_WithSubdirectoryPage_WithBaseUrl_ReturnsFullUrl()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/blog/post.html";
        const string baseUrl = "https://example.com";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir, baseUrl);

        // Assert
        result.ShouldBe("https://example.com/blog/post.html");
    }

    [Fact]
    public void CalculatePageUrl_WithSubdirectoryIndexHtml_WithoutBaseUrl_ReturnsSubdirectoryPath()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/blog/index.html";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir);

        // Assert
        result.ShouldBe("/blog/index.html");
    }

    [Fact]
    public void CalculatePageUrl_WithSubdirectoryIndexHtml_WithBaseUrl_ReturnsFullUrl()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/blog/index.html";
        const string baseUrl = "https://example.com";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir, baseUrl);

        // Assert
        result.ShouldBe("https://example.com/blog/index.html");
    }

    [Fact]
    public void CalculatePageUrl_WithBaseUrlWithTrailingSlash_TrimsTrailingSlash()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/about.html";
        const string baseUrl = "https://example.com/";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir, baseUrl);

        // Assert
        result.ShouldBe("https://example.com/about.html");
    }

    [Fact]
    public void CalculatePageUrl_WithNestedSubdirectories_WithoutBaseUrl_ReturnsNestedPath()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/docs/api/reference.html";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir);

        // Assert
        result.ShouldBe("/docs/api/reference.html");
    }

    [Fact]
    public void CalculatePageUrl_WithNestedSubdirectories_WithBaseUrl_ReturnsFullUrl()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/docs/api/reference.html";
        const string baseUrl = "https://example.com";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir, baseUrl);

        // Assert
        result.ShouldBe("https://example.com/docs/api/reference.html");
    }

    [Fact]
    public void CalculatePageUrl_WithSubdirectory_AlwaysUsesForwardSlashes()
    {
        // Arrange
        var pagesDir = Path.Combine("/", "site", "pages");
        var pagePath = Path.Combine("/", "site", "pages", "blog", "post.html");
        
        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir);

        // Assert
        // The result should always use forward slashes regardless of platform path separators
        result.ShouldBe("/blog/post.html");
        result.ShouldNotContain("\\");
    }

    [Fact]
    public void CalculatePageUrl_WithRootIndexHtml_WithBaseUrlWithTrailingSlash_ReturnsBaseUrl()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/index.html";
        const string baseUrl = "https://example.com/";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir, baseUrl);

        // Assert
        result.ShouldBe("https://example.com");
    }

    [Fact]
    public void CalculatePageUrl_WithMultipleRootPages_ReturnsCorrectPaths()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string page1 = "/site/pages/contact.html";
        const string page2 = "/site/pages/services.html";
        const string page3 = "/site/pages/faq.html";

        // Act
        var result1 = PageUrlHelper.CalculatePageUrl(page1, pagesDir);
        var result2 = PageUrlHelper.CalculatePageUrl(page2, pagesDir);
        var result3 = PageUrlHelper.CalculatePageUrl(page3, pagesDir);

        // Assert
        result1.ShouldBe("/contact.html");
        result2.ShouldBe("/services.html");
        result3.ShouldBe("/faq.html");
    }

    [Fact]
    public void CalculatePageUrl_WithBaseUrl_HandlesDifferentProtocols()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/about.html";
        const string httpUrl = "http://example.com";
        const string httpsUrl = "https://example.com";

        // Act
        var httpResult = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir, httpUrl);
        var httpsResult = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir, httpsUrl);

        // Assert
        httpResult.ShouldBe("http://example.com/about.html");
        httpsResult.ShouldBe("https://example.com/about.html");
    }

    [Fact]
    public void CalculatePageUrl_WithBaseUrlContainingPath_HandlesCorrectly()
    {
        // Arrange
        const string pagesDir = "/site/pages";
        const string pagePath = "/site/pages/about.html";
        const string baseUrl = "https://example.com/site";

        // Act
        var result = PageUrlHelper.CalculatePageUrl(pagePath, pagesDir, baseUrl);

        // Assert
        result.ShouldBe("https://example.com/site/about.html");
    }
}
