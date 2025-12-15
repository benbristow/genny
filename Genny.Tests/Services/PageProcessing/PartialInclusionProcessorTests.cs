using Genny.Models;
using Genny.Services.PageProcessing;

namespace Genny.Tests.Services.PageProcessing;

public class PartialInclusionProcessorTests
{
    private static PageProcessingContext CreateContext(string content, string rootDirectory)
    {
        return new PageProcessingContext
        {
            Content = content,
            PageBody = "",
            PageTitle = "Test Title",
            SiteConfig = new SiteConfig
            {
                Name = "Test Site",
                Description = "Test Description"
            },
            CurrentYear = "2025",
            CurrentEpoch = "1734201600",
            Permalink = "/test.html",
            RootDirectory = rootDirectory,
            IncludedPartials = []
        };
    }

    [Fact]
    public async Task ProcessAsync_WithPartialPlaceholder_IncludesPartialContent()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var partialsDir = Path.Combine(tempDir, "partials");
        Directory.CreateDirectory(partialsDir);
        var partialPath = Path.Combine(partialsDir, "header.html");
        await File.WriteAllTextAsync(partialPath, "<header>Header Content</header>");
        
        var processor = new PartialInclusionProcessor();
        var context = CreateContext("{{ partial: header.html }}", tempDir);

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldBe("<header>Header Content</header>");
            result.Content.ShouldNotContain("{{ partial:");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithNestedPartials_IncludesAllPartials()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var partialsDir = Path.Combine(tempDir, "partials");
        Directory.CreateDirectory(partialsDir);
        
        var headerPath = Path.Combine(partialsDir, "header.html");
        await File.WriteAllTextAsync(headerPath, "<header>{{ partial: nav.html }}</header>");
        
        var navPath = Path.Combine(partialsDir, "nav.html");
        await File.WriteAllTextAsync(navPath, "<nav>Navigation</nav>");
        
        var processor = new PartialInclusionProcessor();
        var context = CreateContext("{{ partial: header.html }}", tempDir);

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldContain("<nav>Navigation</nav>");
            result.Content.ShouldContain("<header>");
            result.Content.ShouldNotContain("{{ partial:");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithCircularReference_PreventsInfiniteLoop()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var partialsDir = Path.Combine(tempDir, "partials");
        Directory.CreateDirectory(partialsDir);
        
        var partialAPath = Path.Combine(partialsDir, "partialA.html");
        await File.WriteAllTextAsync(partialAPath, "A: {{ partial: partialB.html }}");
        
        var partialBPath = Path.Combine(partialsDir, "partialB.html");
        await File.WriteAllTextAsync(partialBPath, "B: {{ partial: partialA.html }}");
        
        var processor = new PartialInclusionProcessor();
        var context = CreateContext("{{ partial: partialA.html }}", tempDir);

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldContain("A:");
            result.Content.ShouldContain("B:");
            // Circular reference should be removed (the second {{ partial: partialA.html }} should be gone)
            var partialACount = (result.Content.Length - result.Content.Replace("partialA.html", "").Length) / "partialA.html".Length;
            partialACount.ShouldBeLessThanOrEqualTo(1);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithMissingPartial_RemovesPlaceholder()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var processor = new PartialInclusionProcessor();
        var context = CreateContext("Content {{ partial: missing.html }} more content", tempDir);

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldBe("Content  more content");
            result.Content.ShouldNotContain("{{ partial: missing.html }}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithPartialWithPlaceholders_ReplacesPlaceholdersInPartial()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var partialsDir = Path.Combine(tempDir, "partials");
        Directory.CreateDirectory(partialsDir);
        var partialPath = Path.Combine(partialsDir, "header.html");
        await File.WriteAllTextAsync(partialPath, "<h1>{{ site.name }}</h1><p>{{ year }}</p>");
        
        var processor = new PartialInclusionProcessor();
        var context = CreateContext("{{ partial: header.html }}", tempDir);

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldContain("<h1>Test Site</h1>");
            result.Content.ShouldContain("<p>2025</p>");
            result.Content.ShouldNotContain("{{ site.name }}");
            result.Content.ShouldNotContain("{{ year }}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithMultiplePartials_IncludesAllPartials()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var partialsDir = Path.Combine(tempDir, "partials");
        Directory.CreateDirectory(partialsDir);
        
        var headerPath = Path.Combine(partialsDir, "header.html");
        await File.WriteAllTextAsync(headerPath, "<header>Header</header>");
        
        var footerPath = Path.Combine(partialsDir, "footer.html");
        await File.WriteAllTextAsync(footerPath, "<footer>Footer</footer>");
        
        var processor = new PartialInclusionProcessor();
        var context = CreateContext("{{ partial: header.html }}Content{{ partial: footer.html }}", tempDir);

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldContain("<header>Header</header>");
            result.Content.ShouldContain("Content");
            result.Content.ShouldContain("<footer>Footer</footer>");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithSpacesInPartialPlaceholder_HandlesCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var partialsDir = Path.Combine(tempDir, "partials");
        Directory.CreateDirectory(partialsDir);
        var partialPath = Path.Combine(partialsDir, "header.html");
        await File.WriteAllTextAsync(partialPath, "<header>Header</header>");
        
        var processor = new PartialInclusionProcessor();
        var context = CreateContext("{{ partial : header.html }}", tempDir);

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldBe("<header>Header</header>");
            result.Content.ShouldNotContain("{{ partial");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_WithNoPartialsDirectory_RemovesPlaceholders()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var processor = new PartialInclusionProcessor();
        var context = CreateContext("Content {{ partial: header.html }} more", tempDir);

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Content.ShouldBe("Content  more");
            result.Content.ShouldNotContain("{{ partial:");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessAsync_ReturnsSameContextInstance()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var partialsDir = Path.Combine(tempDir, "partials");
        Directory.CreateDirectory(partialsDir);
        var partialPath = Path.Combine(partialsDir, "header.html");
        await File.WriteAllTextAsync(partialPath, "<header>Header</header>");
        
        var processor = new PartialInclusionProcessor();
        var context = CreateContext("{{ partial: header.html }}", tempDir);

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.ShouldBeSameAs(context);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }
}
