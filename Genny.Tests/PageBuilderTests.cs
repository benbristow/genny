using Genny.Models;
using Genny.Services;

namespace Genny.Tests;

public class PageBuilderTests
{
    private static SiteConfig CreateTestSiteConfig(string rootDir) => new SiteConfig
    {
        Name = "Test Site",
        Description = "A test site",
        RootDirectory = rootDir,
        OutputDirectory = Path.Combine(rootDir, "build")
    };

    [Fact]
    public async Task BuildPageAsync_WithDefaultLayout_AppliesLayout()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><title>Default</title></head><body>{{content}}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<body>Page Body</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("Page Body");
            result.ShouldContain("<title>Default</title>");
            result.ShouldNotContain("{{content}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithCustomLayout_AppliesSpecifiedLayout()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var customLayoutPath = Path.Combine(layoutsDir, "custom.html");
        await File.WriteAllTextAsync(customLayoutPath, "<html><body class=\"custom\">{{content}}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<!-- layout: custom.html --><body>Custom Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("Custom Content");
            result.ShouldContain("class=\"custom\"");
            result.ShouldNotContain("{{content}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithNoLayout_ReturnsPageAsIs()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagePath = Path.Combine(tempDir, "page.html");
        var pageContent = "<html><body>No Layout</body></html>";
        await File.WriteAllTextAsync(pagePath, pageContent);

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldBe(pageContent);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithLayoutButNoBodyTag_ReturnsFullContentInLayout()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><body>{{content}}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        var pageContent = "<div>Just a div</div>";
        await File.WriteAllTextAsync(pagePath, pageContent);

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("<div>Just a div</div>");
            result.ShouldNotContain("{{content}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithLayoutCommentCaseInsensitive_AppliesLayout()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "custom.html");
        await File.WriteAllTextAsync(layoutPath, "<html><body>{{content}}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<!-- LAYOUT: CUSTOM.HTML --><body>Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("Content");
            result.ShouldNotContain("{{content}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithPageTitleComment_ReplacesTitlePlaceholder()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><title>{{title}}</title></head><body>{{content}}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<!-- title: My Page Title --><body>Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("<title>My Page Title</title>");
            result.ShouldNotContain("{{title}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithTitleTag_ExtractsTitleFromTag()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><title>{{title}}</title></head><body>{{content}}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<html><head><title>Page Title</title></head><body>Content</body></html>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("<title>Page Title</title>");
            result.ShouldNotContain("{{title}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithSiteConfig_ReplacesSitePlaceholders()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><title>{{site.name}} - {{title}}</title><meta name=\"description\" content=\"{{site.description}}\"></head><body>{{content}}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<!-- title: Home --><body>Content</body>");

        var siteConfig = new SiteConfig
        {
            Name = "My Awesome Site",
            Description = "A great website",
            RootDirectory = tempDir,
            OutputDirectory = Path.Combine(tempDir, "build")
        };

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("My Awesome Site");
            result.ShouldContain("A great website");
            result.ShouldContain("Home");
            result.ShouldNotContain("{{site.name}}");
            result.ShouldNotContain("{{site.description}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithNoTitle_ReplacesWithEmptyString()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><title>{{site.name}} - {{title}}</title></head><body>{{content}}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<body>Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("<title>Test Site - </title>");
            result.ShouldNotContain("{{title}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithYearPlaceholder_ReplacesWithCurrentYear()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><body><footer>&copy; {{year}} {{site.name}}</footer>{{content}}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<body>Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);
        var expectedYear = DateTime.Now.Year.ToString();

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain($"&copy; {expectedYear} Test Site");
            result.ShouldNotContain("{{year}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithSpacesInPlaceholders_ReplacesCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><title>{{ site.name }} - {{ title }}</title></head><body>{{ content }}</body><footer>&copy; {{ year }} {{ site.name }}</footer></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<!-- title: My Page --><body>Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);
        var expectedYear = DateTime.Now.Year.ToString();

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("Test Site");
            result.ShouldContain("My Page");
            result.ShouldContain("Content");
            result.ShouldContain(expectedYear);
            result.ShouldNotContain("{{");
            result.ShouldNotContain("}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithSpacesInSitePlaceholders_ReplacesCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><meta name=\"description\" content=\"{{ site.description }}\"></head><body>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<body>Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("A test site");
            result.ShouldNotContain("{{ site.description }}");
            result.ShouldNotContain("{{site.description}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithTitleComment_RemovesCommentFromOutput()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><title>{{ title }}</title></head><body>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<!-- title: My Page --><body>Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("My Page");
            result.ShouldContain("Content");
            result.ShouldNotContain("<!-- title:");
            result.ShouldNotContain("title: My Page");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithLayoutComment_RemovesCommentFromOutput()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "custom.html");
        await File.WriteAllTextAsync(layoutPath, "<html><body>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<!-- layout: custom.html --><body>Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("Content");
            result.ShouldNotContain("<!-- layout:");
            result.ShouldNotContain("layout: custom.html");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithBothComments_RemovesBothFromOutput()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><title>{{ title }}</title></head><body>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<!-- layout: default.html --><!-- title: My Page --><body>Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("My Page");
            result.ShouldContain("Content");
            result.ShouldNotContain("<!-- layout:");
            result.ShouldNotContain("<!-- title:");
            result.ShouldNotContain("layout: default.html");
            result.ShouldNotContain("title: My Page");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithNoLayout_RemovesCommentsFromOutput()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<!-- title: My Page --><html><body>Content</body></html>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("Content");
            result.ShouldNotContain("<!-- title:");
            result.ShouldNotContain("title: My Page");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithCommentsAndWhitespace_RemovesCommentsAndWhitespace()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><body>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        // Comments with newlines and spaces around them
        var pageContent = """
            <!-- layout: default.html -->
            
            <!-- title: My Page -->
            
            <body>
                Content
            </body>
            """;
        await File.WriteAllTextAsync(pagePath, pageContent);

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("Content");
            result.ShouldNotContain("<!-- layout:");
            result.ShouldNotContain("<!-- title:");
            // Verify no extra blank lines from removed comments
            result.ShouldNotContain("\n\n\n");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithEpochPlaceholder_ReplacesWithUnixEpoch()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><link rel=\"stylesheet\" href=\"/style.css?v={{ epoch }}\"></head><body>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<body>Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);
        var beforeEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            var afterEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            result.ShouldContain("href=\"/style.css?v=");
            result.ShouldNotContain("{{ epoch }}");
            result.ShouldNotContain("{{epoch}}");
            
            // Verify the epoch is a valid Unix timestamp (between before and after)
            var epochMatch = System.Text.RegularExpressions.Regex.Match(result, @"style\.css\?v=(\d+)");
            epochMatch.Success.ShouldBeTrue();
            var epochValue = long.Parse(epochMatch.Groups[1].Value);
            epochValue.ShouldBeGreaterThanOrEqualTo(beforeEpoch);
            epochValue.ShouldBeLessThanOrEqualTo(afterEpoch + 1); // Allow 1 second tolerance
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithEpochPlaceholder_ReplacesWithCurrentUnixTimestamp()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><body><script src=\"/app.js?t={{epoch}}\"></script>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(tempDir, "page.html");
        await File.WriteAllTextAsync(pagePath, "<body>Content</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);
        var beforeEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            var afterEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            result.ShouldContain("app.js?t=");
            result.ShouldNotContain("{{epoch}}");
            
            // Verify the epoch is a valid Unix timestamp (between before and after)
            var epochMatch = System.Text.RegularExpressions.Regex.Match(result, @"app\.js\?t=(\d+)");
            epochMatch.Success.ShouldBeTrue();
            var epochValue = long.Parse(epochMatch.Groups[1].Value);
            epochValue.ShouldBeGreaterThanOrEqualTo(beforeEpoch);
            epochValue.ShouldBeLessThanOrEqualTo(afterEpoch + 1); // Allow 1 second tolerance
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithPermalinkPlaceholder_ReplacesWithPageUrl()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><link rel=\"canonical\" href=\"{{ permalink }}\"></head><body>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(pagesDir, "about.html");
        await File.WriteAllTextAsync(pagePath, "<body>About Page</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("href=\"/about.html\"");
            result.ShouldNotContain("{{ permalink }}");
            result.ShouldNotContain("{{permalink}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithPermalinkPlaceholder_IndexPage_ReplacesWithRootUrl()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><link rel=\"canonical\" href=\"{{permalink}}\"></head><body>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<body>Home Page</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("href=\"/\"");
            result.ShouldNotContain("{{permalink}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithPermalinkPlaceholder_SubdirectoryPage_ReplacesWithSubdirectoryUrl()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var blogDir = Path.Combine(pagesDir, "blog");
        Directory.CreateDirectory(blogDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><link rel=\"canonical\" href=\"{{ permalink }}\"></head><body>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(blogDir, "post.html");
        await File.WriteAllTextAsync(pagePath, "<body>Blog Post</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("href=\"/blog/post.html\"");
            result.ShouldNotContain("{{ permalink }}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithPermalinkPlaceholder_WithBaseUrl_ReplacesWithFullUrl()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><link rel=\"canonical\" href=\"{{permalink}}\"></head><body>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(pagesDir, "about.html");
        await File.WriteAllTextAsync(pagePath, "<body>About Page</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);
        siteConfig.BaseUrl = "https://example.com";

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("href=\"https://example.com/about.html\"");
            result.ShouldNotContain("{{permalink}}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task BuildPageAsync_WithPermalinkPlaceholder_IndexPageWithBaseUrl_ReplacesWithBaseUrl()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        
        var layoutsDir = Path.Combine(tempDir, "layouts");
        Directory.CreateDirectory(layoutsDir);
        var layoutPath = Path.Combine(layoutsDir, "default.html");
        await File.WriteAllTextAsync(layoutPath, "<html><head><link rel=\"canonical\" href=\"{{ permalink }}\"></head><body>{{ content }}</body></html>");
        
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<body>Home Page</body>");

        var siteConfig = CreateTestSiteConfig(tempDir);
        siteConfig.BaseUrl = "https://example.com";

        try
        {
            // Act
            var result = await PageBuilder.BuildPageAsync(pagePath, tempDir, siteConfig);

            // Assert
            result.ShouldContain("href=\"https://example.com\"");
            result.ShouldNotContain("{{ permalink }}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }
}
