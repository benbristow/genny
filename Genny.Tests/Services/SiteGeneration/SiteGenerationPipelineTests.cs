using Genny.Models;
using Genny.Services.SiteGeneration;

namespace Genny.Tests.Services.SiteGeneration;

public class SiteGenerationPipelineTests
{
    [Fact]
    public async Task ProcessAsync_WithSingleProcessor_ExecutesProcessor()
    {
        // Arrange
        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                OutputDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())
            },
            Verbose = false
        };

        var processor = new BuildDirectoryCreationProcessor();
        var pipeline = new SiteGenerationPipeline()
            .AddProcessor(processor);

        try
        {
            // Act
            var result = await pipeline.ProcessAsync(context);

            // Assert
            Directory.Exists(context.SiteConfig.OutputDirectory).ShouldBeTrue();
            result.ShouldBeSameAs(context);
        }
        finally
        {
            if (Directory.Exists(context.SiteConfig.OutputDirectory))
            {
                Directory.Delete(context.SiteConfig.OutputDirectory, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithMultipleProcessors_ExecutesInOrder()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var buildDir = Path.Combine(tempDir, "build");
        Directory.CreateDirectory(buildDir);
        var testFile = Path.Combine(buildDir, "test.txt");
        await File.WriteAllTextAsync(testFile, "test");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                OutputDirectory = buildDir
            },
            Verbose = false
        };

        var pipeline = new SiteGenerationPipeline()
            .AddProcessor(new BuildDirectoryCleanupProcessor())
            .AddProcessor(new BuildDirectoryCreationProcessor());

        try
        {
            // Act
            var result = await pipeline.ProcessAsync(context);

            // Assert
            // Cleanup should remove the directory, creation should recreate it
            Directory.Exists(buildDir).ShouldBeTrue();
            File.Exists(testFile).ShouldBeFalse(); // File should be gone after cleanup
            result.ShouldBeSameAs(context);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithNoProcessors_ReturnsContextUnchanged()
    {
        // Arrange
        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig(),
            Verbose = false
        };

        var pipeline = new SiteGenerationPipeline();

        // Act
        var result = await pipeline.ProcessAsync(context);

        // Assert
        result.ShouldBeSameAs(context);
    }

    [Fact]
    public void AddProcessor_ReturnsPipelineForChaining()
    {
        // Arrange
        var pipeline = new SiteGenerationPipeline();

        // Act
        var result = pipeline
            .AddProcessor(new BuildDirectoryCleanupProcessor())
            .AddProcessor(new BuildDirectoryCreationProcessor());

        // Assert
        result.ShouldBeSameAs(pipeline);
    }

    [Fact]
    public async Task ProcessAsync_WithFullPipeline_ExecutesAllProcessors()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Test</body></html>");
        
        var publicDir = Path.Combine(tempDir, "public");
        Directory.CreateDirectory(publicDir);
        var assetPath = Path.Combine(publicDir, "style.css");
        await File.WriteAllTextAsync(assetPath, "body { color: red; }");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = Path.Combine(tempDir, "build"),
                GenerateSitemap = true
            },
            Verbose = false
        };

        var pipeline = new SiteGenerationPipeline()
            .AddProcessor(new ConfigurationLoggingProcessor())
            .AddProcessor(new BuildDirectoryCleanupProcessor())
            .AddProcessor(new BuildDirectoryCreationProcessor())
            .AddProcessor(new PageDiscoveryProcessor())
            .AddProcessor(new PublicAssetsCopyProcessor())
            .AddProcessor(new PageProcessingProcessor())
            .AddProcessor(new SitemapGenerationProcessor())
            .AddProcessor(new CompletionLoggingProcessor());

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                var result = await pipeline.ProcessAsync(context);
            }

            // Assert
            Directory.Exists(context.SiteConfig.OutputDirectory).ShouldBeTrue();
            context.Pages.Count.ShouldBe(1);
            context.CopiedPublicFiles.ShouldBe(1);
            File.Exists(Path.Combine(context.SiteConfig.OutputDirectory, "index.html")).ShouldBeTrue();
            File.Exists(Path.Combine(context.SiteConfig.OutputDirectory, "style.css")).ShouldBeTrue();
            File.Exists(Path.Combine(context.SiteConfig.OutputDirectory, "sitemap.xml")).ShouldBeTrue();
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithPageDiscoveryProcessor_DiscoverPages()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var page1Path = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(page1Path, "<html><body>Index</body></html>");
        
        var subDir = Path.Combine(pagesDir, "blog");
        Directory.CreateDirectory(subDir);
        var page2Path = Path.Combine(subDir, "post.html");
        await File.WriteAllTextAsync(page2Path, "<html><body>Post</body></html>");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir
            },
            Verbose = false
        };

        var processor = new PageDiscoveryProcessor();

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Pages.Count.ShouldBe(2);
            result.Pages.ShouldContain(page1Path);
            result.Pages.ShouldContain(page2Path);
            result.PagesDirectory.ShouldBe(pagesDir);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithPageDiscoveryProcessor_WithNoPagesDirectory_ReturnsEmptyList()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir
            },
            Verbose = false
        };

        var processor = new PageDiscoveryProcessor();

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.Pages.Count.ShouldBe(0);
            result.PagesDirectory.ShouldBe(Path.Combine(tempDir, "pages"));
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithPublicAssetsCopyProcessor_CopiesPublicFiles()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var publicDir = Path.Combine(tempDir, "public");
        Directory.CreateDirectory(publicDir);
        var assetPath = Path.Combine(publicDir, "style.css");
        await File.WriteAllTextAsync(assetPath, "body { color: red; }");
        
        var subDir = Path.Combine(publicDir, "images");
        Directory.CreateDirectory(subDir);
        var imagePath = Path.Combine(subDir, "logo.png");
        await File.WriteAllTextAsync(imagePath, "fake image");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = Path.Combine(tempDir, "build")
            },
            Verbose = false
        };

        Directory.CreateDirectory(context.SiteConfig.OutputDirectory);

        var processor = new PublicAssetsCopyProcessor();

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            result.CopiedPublicFiles.ShouldBe(2);
            File.Exists(Path.Combine(context.SiteConfig.OutputDirectory, "style.css")).ShouldBeTrue();
            File.Exists(Path.Combine(context.SiteConfig.OutputDirectory, "images", "logo.png")).ShouldBeTrue();
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithPublicAssetsCopyProcessor_WithNoPublicDirectory_DoesNotThrow()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = Path.Combine(tempDir, "build")
            },
            Verbose = false
        };

        var processor = new PublicAssetsCopyProcessor();

        try
        {
            // Act & Assert
            await Should.NotThrowAsync(async () => await processor.ProcessAsync(context));
            context.CopiedPublicFiles.ShouldBe(0);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithSitemapGenerationProcessor_WithSitemapEnabled_GeneratesSitemap()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Test</body></html>");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = Path.Combine(tempDir, "build"),
                GenerateSitemap = true,
                BaseUrl = "https://example.com"
            },
            Pages = [pagePath],
            PagesDirectory = pagesDir,
            Verbose = false
        };

        Directory.CreateDirectory(context.SiteConfig.OutputDirectory);

        var processor = new SitemapGenerationProcessor();

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            var sitemapPath = Path.Combine(context.SiteConfig.OutputDirectory, "sitemap.xml");
            File.Exists(sitemapPath).ShouldBeTrue();
            var sitemapContent = await File.ReadAllTextAsync(sitemapPath);
            sitemapContent.ShouldContain("https://example.com");
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithSitemapGenerationProcessor_WithSitemapDisabled_DoesNotGenerateSitemap()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = Path.Combine(tempDir, "build"),
                GenerateSitemap = false
            },
            Verbose = false
        };

        Directory.CreateDirectory(context.SiteConfig.OutputDirectory);

        var processor = new SitemapGenerationProcessor();

        try
        {
            // Act
            var result = await processor.ProcessAsync(context);

            // Assert
            var sitemapPath = Path.Combine(context.SiteConfig.OutputDirectory, "sitemap.xml");
            File.Exists(sitemapPath).ShouldBeFalse();
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithPageProcessingProcessor_ProcessesAndWritesPages()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var pagesDir = Path.Combine(tempDir, "pages");
        Directory.CreateDirectory(pagesDir);
        var pagePath = Path.Combine(pagesDir, "index.html");
        await File.WriteAllTextAsync(pagePath, "<html><body>Test</body></html>");

        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = tempDir,
                OutputDirectory = Path.Combine(tempDir, "build")
            },
            Pages = [pagePath],
            PagesDirectory = pagesDir,
            Verbose = false
        };

        Directory.CreateDirectory(context.SiteConfig.OutputDirectory);

        var processor = new PageProcessingProcessor();

        try
        {
            // Act
            using (TestHelpers.SuppressConsoleOutput())
            {
                var result = await processor.ProcessAsync(context);
            }

            // Assert
            var builtPage = Path.Combine(context.SiteConfig.OutputDirectory, "index.html");
            File.Exists(builtPage).ShouldBeTrue();
            context.ProcessedPages.ShouldBe(1);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithConfigurationLoggingProcessor_WithVerboseEnabled_LogsConfiguration()
    {
        // Arrange
        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                RootDirectory = "/test",
                OutputDirectory = "/test/build",
                Name = "Test Site",
                MinifyOutput = true,
                GenerateSitemap = true
            },
            Verbose = true
        };

        var processor = new ConfigurationLoggingProcessor();

        // Act & Assert - Should not throw
        using (TestHelpers.SuppressConsoleOutput())
        {
            await Should.NotThrowAsync(async () => await processor.ProcessAsync(context));
        }
    }

    [Fact]
    public async Task ProcessAsync_WithConfigurationLoggingProcessor_WithVerboseDisabled_DoesNotLog()
    {
        // Arrange
        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig(),
            Verbose = false
        };

        var processor = new ConfigurationLoggingProcessor();

        // Act & Assert - Should not throw
        await Should.NotThrowAsync(async () => await processor.ProcessAsync(context));
    }

    [Fact]
    public async Task ProcessAsync_WithCompletionLoggingProcessor_LogsCompletion()
    {
        // Arrange
        var context = new SiteGenerationContext
        {
            SiteConfig = new SiteConfig
            {
                OutputDirectory = "/test/build"
            },
            Verbose = false
        };

        var processor = new CompletionLoggingProcessor();

        // Act & Assert - Should not throw
        using (TestHelpers.SuppressConsoleOutput())
        {
            await Should.NotThrowAsync(async () => await processor.ProcessAsync(context));
        }
    }
}
