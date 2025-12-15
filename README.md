# Genny

[![Test and Build](https://github.com/benbristow/genny/actions/workflows/ci.yml/badge.svg)](https://github.com/benbristow/genny/actions/workflows/ci.yml)

A static site generator built with .NET that transforms your HTML pages and assets into a production-ready website.

For people that want to generate websites and don't want a headache of configuring a complex static site generator. Plays nicely with JavaScript asset management tools like Vite/Parcel/Webpack if that's what you want.

## Features

- 🚀 **Simple and Fast** - Build static sites quickly with minimal configuration
- 📄 **Layout System** - Reusable layouts with content placeholders
- 🧩 **Partials Support** - Include reusable HTML snippets in layouts and pages
- 📁 **Organized Structure** - Clean separation of pages, layouts, and assets
- 🎨 **Asset Management** - Automatic copying of public assets
- 🧹 **Smart Filtering** - Automatically ignores common development files
- ⚙️ **TOML Configuration** - Simple configuration file format

## Installation

### Prerequisites

- .NET 10.0 SDK or later

### Build from Source

```bash
git clone <repository-url>
cd Genny
dotnet build
```

## Getting Started

### 1. Create a Project Structure

Genny expects the following directory structure:

```
your-site/
├── genny.toml          # Site configuration
├── pages/              # Your HTML pages
│   ├── index.html
│   └── about.html
├── layouts/            # Layout templates (optional)
│   └── default.html
├── partials/           # Reusable HTML snippets (optional)
│   ├── header.html
│   └── footer.html
└── public/             # Static assets (optional)
    ├── style.css
    └── images/
```

### 2. Create Configuration File

Create a `genny.toml` file in your project root:

```toml
name = "My Awesome Site"
description = "A static site built with Genny"
```

### 3. Create Your First Page

Create `pages/index.html`:

```html
<body>
    <h1>Welcome to My Site</h1>
    <p>This is my homepage.</p>
</body>
```

### 4. Build Your Site

Run the build command from your project root:

```bash
dotnet run --project Genny/Genny.csproj -- build
```

Or if you've installed Genny globally:

```bash
genny build
```

Your site will be generated in the `build/` directory.

## Project Structure

### Pages Directory (`pages/`)

Place all your HTML pages in the `pages/` directory. Pages can be organized in subdirectories:

```
pages/
├── index.html          # Becomes build/index.html
├── about.html          # Becomes build/about.html
└── blog/
    └── post.html       # Becomes build/blog/post.html
```

**Note:** Root-level pages are flattened to the build root, while subdirectory pages preserve their structure.

### Layouts Directory (`layouts/`)

Layouts are HTML templates that wrap your page content. They use the `{{content}}` placeholder to inject page content.

**Default Layout** (`layouts/default.html`):

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="{{ site.description }}">
    <title>{{ site.name }} - {{ title }}</title>
    <link rel="stylesheet" href="/style.css">
</head>
<body>
    <header>
        <h1>{{ site.name }}</h1>
        <nav>
            <a href="/">Home</a>
            <a href="/about.html">About</a>
        </nav>
    </header>
    <main>
        {{ content }}
    </main>
    <footer>
        <p>&copy; {{ year }} {{ site.name }}</p>
    </footer>
</body>
</html>
```

**Available Placeholders:**
- `{{content}}` or `{{ content }}` - The page body content
- `{{title}}` or `{{ title }}` - The page title (extracted from page)
- `{{site.name}}` or `{{ site.name }}` - Site name from `genny.toml`
- `{{site.description}}` or `{{ site.description }}` - Site description from `genny.toml`
- `{{year}}` or `{{ year }}` - Current year (e.g., 2025)
- `{{epoch}}` or `{{ epoch }}` - Current Unix epoch timestamp in seconds (e.g., 1734201600)
- `{{permalink}}` or `{{ permalink }}` - The full URL of the current page

**Note:** Spaces around placeholder names are optional. Both `{{title}}` and `{{ title }}` work the same way.

The `{{epoch}}` placeholder is useful for cache busting:
```html
<link rel="stylesheet" href="/style.css?v={{ epoch }}">
<script src="/app.js?t={{ epoch }}"></script>
```

**Using a Custom Layout:**

Add a comment at the top of your page to specify a custom layout:

```html
<!-- layout: custom.html -->
<body>
    <h1>Custom Page</h1>
    <p>This page uses a custom layout.</p>
</body>
```

**Note:** Layout comments are automatically removed from the final output.

**Specifying Page Title:**

You can specify a page title in two ways:

1. **Using a comment** (recommended):
```html
<!-- title: My Page Title -->
<body>
    <h1>My Page</h1>
</body>
```

2. **Using a `<title>` tag**:
```html
<html>
<head>
    <title>My Page Title</title>
</head>
<body>
    <h1>My Page</h1>
</body>
</html>
```

The title will be extracted and available as `{{title}}` in your layout. If no title is specified, `{{title}}` will be replaced with an empty string.

**Note:** Title comments (`<!-- title: ... -->`) are automatically removed from the final output. The `<title>` tag method will also extract the title, but the tag itself will remain in the output if no layout is used.

**Layout Behavior:**
- If no layout is specified, Genny looks for `default.html`
- If no layout exists, the page content is used as-is
- Layouts are not copied to the build directory (they're templates only)

### Partials Directory (`partials/`)

Partials are reusable HTML snippets that can be included in layouts, pages, and other partials. They're perfect for components like headers, footers, navigation menus, or any repeated content.

**Syntax:**
```html
{{ partial: filename.html }}
```

Spaces around the colon are optional: `{{ partial : filename.html }}` works the same way.

**Example Partial** (`partials/header.html`):
```html
<header>
    <h1>{{ site.name }}</h1>
    <nav>
        <a href="/">Home</a>
        <a href="/about.html">About</a>
    </nav>
</header>
```

**Using Partials in a Layout:**
```html
<!DOCTYPE html>
<html>
<head>
    <title>{{ site.name }} - {{ title }}</title>
</head>
<body>
    {{ partial: header.html }}
    <main>
        {{ content }}
    </main>
    {{ partial: footer.html }}
</body>
</html>
```

**Using Partials in a Page:**
```html
<body>
    <h1>Welcome</h1>
    <p>Check out our latest news:</p>
    {{ partial: news-section.html }}
</body>
```

**Nested Partials:**
Partials can include other partials. For example, `header.html` can include `nav.html`:

**partials/header.html:**
```html
<header>
    <h1>{{ site.name }}</h1>
    {{ partial: nav.html }}
</header>
```

**partials/nav.html:**
```html
<nav>
    <a href="/">Home</a>
    <a href="/about.html">About</a>
</nav>
```

**Circular Reference Prevention:**
Genny automatically prevents circular references (e.g., partial A includes partial B which includes partial A). If a circular reference is detected, the placeholder is removed to prevent infinite loops.

**Missing Partials:**
If a partial file doesn't exist, the placeholder is automatically removed from the output.

**Note:** Partials are not copied to the build directory (they're templates only).

### Public Directory (`public/`)

Static assets like CSS, JavaScript, images, and other files go in the `public/` directory. Everything in `public/` is copied to the build root:

```
public/
├── style.css           # Copied to build/style.css
├── script.js           # Copied to build/script.js
└── images/
    └── logo.png        # Copied to build/images/logo.png
```

## Configuration

### genny.toml

The configuration file supports the following options:

| Option | Description | Required | Default |
|--------|-------------|----------|---------|
| `name` | Site name | No | `""` |
| `description` | Site description | No | `""` |
| `base_url` | Base URL for the site (used in sitemap and permalinks) | No | `null` |
| `generate_sitemap` | Whether to generate sitemap.xml | No | `true` |
| `minify_output` | Whether to minify HTML output by removing unnecessary whitespace | No | `true` |

**Note:** Minification removes unnecessary whitespace, newlines, and collapses multiple spaces. Disable it (`minify_output = false`) if you want to preserve formatting for readability during development or debugging.

Example:

```toml
name = "My Blog"
description = "A personal blog about technology and life"
base_url = "https://example.com"
```

**Note:** If `base_url` is not specified, sitemap URLs will use relative paths starting with `/`.

## Commands

### Build

Build your static site:

```bash
genny build
```

Options:
- `-v, --verbose` - Enable verbose output (shows detailed build information including file paths and directory operations)

## Ignored Files and Directories

Genny automatically ignores common development files and directories:

**Ignored Files:**
- `.gitignore`, `.env`, `.env.local`, `.env.production`
- `package.json`, `package-lock.json`, `yarn.lock`, `pnpm-lock.yaml`
- `.git`, `.gitattributes`, `.gitkeep`
- `.DS_Store`, `Thumbs.db`

**Ignored Directories:**
- `node_modules`, `.git`, `.vscode`, `.idea`, `.vs`
- `.next`, `.nuxt`, `dist`, `build`, `.cache`
- `layouts` (templates, not copied to build)
- `partials` (templates, not copied to build)

## Examples

### Example 1: Simple Blog Post

**pages/blog/my-first-post.html:**
```html
<!-- layout: post.html -->
<!-- title: My First Post -->
<body>
    <article>
        <h1>My First Post</h1>
        <p>Published on January 1, 2025</p>
        <p>This is my first blog post!</p>
    </article>
</body>
```

**layouts/post.html:**
```html
<!DOCTYPE html>
<html>
<head>
    <title>{{site.name}} - Blog - {{title}}</title>
    <meta name="description" content="{{site.description}}">
    <link rel="stylesheet" href="/style.css">
</head>
<body>
    <header>
        <h1>{{site.name}}</h1>
        <nav>
            <a href="/">Home</a>
            <a href="/blog">Blog</a>
        </nav>
    </header>
    <main>
        {{content}}
    </main>
    <footer>
        <p>&copy; {{year}} {{site.name}}</p>
    </footer>
</body>
</html>
```

### Example 2: Page Without Layout

**pages/standalone.html:**
```html
<!DOCTYPE html>
<html>
<head>
    <title>Standalone Page</title>
</head>
<body>
    <h1>This page doesn't use a layout</h1>
    <p>It's a complete HTML document.</p>
</body>
</html>
```

### Example 3: Using Partials

**partials/header.html:**
```html
<header>
    <h1>{{ site.name }}</h1>
    <nav>{{ partial: nav.html }}</nav>
</header>
```

**partials/nav.html:**
```html
<a href="/">Home</a>
<a href="/about.html">About</a>
<a href="/blog.html">Blog</a>
```

**partials/footer.html:**
```html
<footer>
    <p>&copy; {{ year }} {{ site.name }}</p>
    <p><a href="{{ permalink }}">Permalink</a></p>
</footer>
```

**layouts/default.html:**
```html
<!DOCTYPE html>
<html>
<head>
    <title>{{ site.name }} - {{ title }}</title>
    <link rel="canonical" href="{{ permalink }}">
</head>
<body>
    {{ partial: header.html }}
    <main>
        {{ content }}
    </main>
    {{ partial: footer.html }}
</body>
</html>
```

**pages/index.html:**
```html
<!-- title: Home -->
<body>
    <h1>Welcome</h1>
    <p>This is the homepage.</p>
</body>
```

## Development

### Running Tests

```bash
cd Genny.Tests
dotnet test
```

### Building

```bash
dotnet build
```

### Running

```bash
dotnet run --project Genny/Genny.csproj -- build
```

## How It Works

1. **Configuration Parsing**: Genny looks for `genny.toml` in the current directory or parent directories
2. **Page Discovery**: Recursively finds all `.html` files in the `pages/` directory
3. **Layout Application**: Applies layouts from the `layouts/` directory if available
4. **Asset Copying**: Copies all files from `public/` to the build directory
5. **Output Generation**: Writes processed pages to the `build/` directory
6. **Sitemap Generation**: Automatically generates `sitemap.xml` with all pages

## Sitemap

Genny automatically generates a `sitemap.xml` file in the build directory containing all pages from your site. The sitemap includes:

- **URLs**: All pages with proper paths (index.html maps to root URL)
- **Last Modified**: File modification dates
- **Change Frequency**: Set to "monthly" by default
- **Priority**: Root page (index.html) gets priority 1.0, other pages get 0.8

**Example sitemap.xml:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
  <url>
    <loc>https://example.com</loc>
    <lastmod>2025-01-15T10:30:00Z</lastmod>
    <changefreq>monthly</changefreq>
    <priority>1.0</priority>
  </url>
  <url>
    <loc>https://example.com/about.html</loc>
    <lastmod>2025-01-14T09:20:00Z</lastmod>
    <changefreq>monthly</changefreq>
    <priority>0.8</priority>
  </url>
</urlset>
```

To use a custom base URL in the sitemap, add `base_url` to your `genny.toml`:

```toml
base_url = "https://example.com"
```

To disable sitemap generation, set `generate_sitemap = false`:

```toml
generate_sitemap = false
```

# TODO?

- Blog/articles support
- RSS feed support
- Some sort of support for 'objects' (e.g. portfolio items)

## License

This project is licensed under the GNU General Public License v3.0 or later. See the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a pull request.
