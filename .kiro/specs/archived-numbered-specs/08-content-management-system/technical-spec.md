# Content Management System - Technical Specification

## Component Overview
The Content Management System provides dynamic content creation and management capabilities including blog posts, educational articles, FAQs, landing pages, and SEO optimization tools, with multilingual support and AI-powered content suggestions.

## Database Schema

### CMS Tables

```sql
-- ContentTypes
CREATE TABLE [dbo].[ContentTypes] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL UNIQUE,
    [DisplayName] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Schema] NVARCHAR(MAX) NOT NULL, -- JSON schema definition
    [ValidationRules] NVARCHAR(MAX) NULL, -- JSON
    [DefaultTemplate] NVARCHAR(100) NULL,
    [Icon] NVARCHAR(50) NULL,
    [IsSystem] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- ContentItems
CREATE TABLE [dbo].[ContentItems] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ContentTypeId] INT NOT NULL,
    [Title] NVARCHAR(500) NOT NULL,
    [Slug] NVARCHAR(500) NOT NULL,
    [Status] INT NOT NULL, -- 0: Draft, 1: Published, 2: Scheduled, 3: Archived
    [Version] INT NOT NULL DEFAULT 1,
    [Language] NVARCHAR(10) NOT NULL DEFAULT 'en-GB',
    [ParentId] INT NULL, -- For translations
    [Content] NVARCHAR(MAX) NOT NULL, -- JSON structured content
    [Excerpt] NVARCHAR(1000) NULL,
    [FeaturedImageUrl] NVARCHAR(500) NULL,
    [Author] NVARCHAR(450) NOT NULL,
    [PublishedAt] DATETIME2 NULL,
    [ScheduledFor] DATETIME2 NULL,
    [ExpiresAt] DATETIME2 NULL,
    [ViewCount] INT NOT NULL DEFAULT 0,
    [LikeCount] INT NOT NULL DEFAULT 0,
    [ShareCount] INT NOT NULL DEFAULT 0,
    [CommentCount] INT NOT NULL DEFAULT 0,
    [ReadingTime] INT NULL, -- Minutes
    [SEOTitle] NVARCHAR(200) NULL,
    [SEODescription] NVARCHAR(500) NULL,
    [SEOKeywords] NVARCHAR(500) NULL,
    [OGImageUrl] NVARCHAR(500) NULL,
    [CanonicalUrl] NVARCHAR(500) NULL,
    [IsIndexable] BIT NOT NULL DEFAULT 1,
    [Tags] NVARCHAR(MAX) NULL, -- JSON array
    [Categories] NVARCHAR(MAX) NULL, -- JSON array of category IDs
    [CustomFields] NVARCHAR(MAX) NULL, -- JSON
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_ContentItems_Slug_Language] ([Slug], [Language]) UNIQUE,
    INDEX [IX_ContentItems_Status_PublishedAt] ([Status], [PublishedAt] DESC),
    INDEX [IX_ContentItems_ContentTypeId] ([ContentTypeId]),
    FULLTEXT INDEX [FT_ContentItems] ([Title], [Content], [Excerpt]) KEY INDEX [PK_ContentItems],
    CONSTRAINT [FK_ContentItems_ContentTypes] FOREIGN KEY ([ContentTypeId]) REFERENCES [ContentTypes]([Id]),
    CONSTRAINT [FK_ContentItems_Parent] FOREIGN KEY ([ParentId]) REFERENCES [ContentItems]([Id]),
    CONSTRAINT [FK_ContentItems_Author] FOREIGN KEY ([Author]) REFERENCES [AspNetUsers]([Id])
);

-- ContentRevisions
CREATE TABLE [dbo].[ContentRevisions] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ContentItemId] INT NOT NULL,
    [Version] INT NOT NULL,
    [Title] NVARCHAR(500) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [ChangeSummary] NVARCHAR(500) NULL,
    [Author] NVARCHAR(450) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_ContentRevisions_ContentItemId_Version] ([ContentItemId], [Version]),
    CONSTRAINT [FK_ContentRevisions_ContentItems] FOREIGN KEY ([ContentItemId]) REFERENCES [ContentItems]([Id]),
    CONSTRAINT [FK_ContentRevisions_Author] FOREIGN KEY ([Author]) REFERENCES [AspNetUsers]([Id])
);

-- Categories
CREATE TABLE [dbo].[Categories] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Slug] NVARCHAR(100) NOT NULL UNIQUE,
    [Description] NVARCHAR(500) NULL,
    [ParentCategoryId] INT NULL,
    [ImageUrl] NVARCHAR(500) NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [SEOTitle] NVARCHAR(200) NULL,
    [SEODescription] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Categories_Parent] FOREIGN KEY ([ParentCategoryId]) REFERENCES [Categories]([Id])
);

-- Tags
CREATE TABLE [dbo].[Tags] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL UNIQUE,
    [Slug] NVARCHAR(100) NOT NULL UNIQUE,
    [Description] NVARCHAR(500) NULL,
    [UsageCount] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- MediaLibrary
CREATE TABLE [dbo].[MediaLibrary] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [FileName] NVARCHAR(255) NOT NULL,
    [FileUrl] NVARCHAR(500) NOT NULL,
    [ThumbnailUrl] NVARCHAR(500) NULL,
    [FileType] NVARCHAR(50) NOT NULL,
    [MimeType] NVARCHAR(100) NOT NULL,
    [FileSize] BIGINT NOT NULL,
    [Width] INT NULL,
    [Height] INT NULL,
    [Duration] INT NULL, -- For videos/audio in seconds
    [Title] NVARCHAR(200) NULL,
    [AltText] NVARCHAR(500) NULL,
    [Caption] NVARCHAR(1000) NULL,
    [FolderId] INT NULL,
    [Tags] NVARCHAR(MAX) NULL, -- JSON array
    [Metadata] NVARCHAR(MAX) NULL, -- JSON
    [UploadedBy] NVARCHAR(450) NOT NULL,
    [UsageCount] INT NOT NULL DEFAULT 0,
    [LastUsedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_MediaLibrary_FileType] ([FileType]),
    INDEX [IX_MediaLibrary_FolderId] ([FolderId]),
    CONSTRAINT [FK_MediaLibrary_Folders] FOREIGN KEY ([FolderId]) REFERENCES [MediaFolders]([Id]),
    CONSTRAINT [FK_MediaLibrary_UploadedBy] FOREIGN KEY ([UploadedBy]) REFERENCES [AspNetUsers]([Id])
);

-- MediaFolders
CREATE TABLE [dbo].[MediaFolders] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [ParentId] INT NULL,
    [Path] NVARCHAR(500) NOT NULL,
    [CreatedBy] NVARCHAR(450) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_MediaFolders_Parent] FOREIGN KEY ([ParentId]) REFERENCES [MediaFolders]([Id]),
    CONSTRAINT [FK_MediaFolders_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [AspNetUsers]([Id])
);

-- ContentTemplates
CREATE TABLE [dbo].[ContentTemplates] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL UNIQUE,
    [DisplayName] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [ContentTypeId] INT NULL,
    [Template] NVARCHAR(MAX) NOT NULL, -- Razor/Liquid template
    [Styles] NVARCHAR(MAX) NULL, -- CSS
    [Scripts] NVARCHAR(MAX) NULL, -- JS
    [Fields] NVARCHAR(MAX) NULL, -- JSON schema
    [PreviewImageUrl] NVARCHAR(500) NULL,
    [IsDefault] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ContentTemplates_ContentTypes] FOREIGN KEY ([ContentTypeId]) REFERENCES [ContentTypes]([Id])
);

-- ContentBlocks
CREATE TABLE [dbo].[ContentBlocks] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL UNIQUE,
    [Type] NVARCHAR(50) NOT NULL, -- text, image, video, carousel, cta, etc.
    [Content] NVARCHAR(MAX) NOT NULL, -- JSON
    [IsReusable] BIT NOT NULL DEFAULT 1,
    [UsageCount] INT NOT NULL DEFAULT 0,
    [CreatedBy] NVARCHAR(450) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ContentBlocks_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [AspNetUsers]([Id])
);

-- ContentWorkflows
CREATE TABLE [dbo].[ContentWorkflows] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ContentItemId] INT NOT NULL,
    [Status] INT NOT NULL, -- 0: Draft, 1: Review, 2: Approved, 3: Published
    [AssignedTo] NVARCHAR(450) NULL,
    [Comments] NVARCHAR(MAX) NULL,
    [DueDate] DATETIME2 NULL,
    [CompletedAt] DATETIME2 NULL,
    [CreatedBy] NVARCHAR(450) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ContentWorkflows_ContentItems] FOREIGN KEY ([ContentItemId]) REFERENCES [ContentItems]([Id]),
    CONSTRAINT [FK_ContentWorkflows_AssignedTo] FOREIGN KEY ([AssignedTo]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_ContentWorkflows_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [AspNetUsers]([Id])
);

-- SEOAnalysis
CREATE TABLE [dbo].[SEOAnalysis] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ContentItemId] INT NOT NULL,
    [Score] INT NOT NULL, -- 0-100
    [Issues] NVARCHAR(MAX) NULL, -- JSON array
    [Suggestions] NVARCHAR(MAX) NULL, -- JSON array
    [Keywords] NVARCHAR(MAX) NULL, -- JSON with density
    [Readability] DECIMAL(5, 2) NULL,
    [LastAnalyzedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_SEOAnalysis_ContentItems] FOREIGN KEY ([ContentItemId]) REFERENCES [ContentItems]([Id])
);

-- ContentMetrics
CREATE TABLE [dbo].[ContentMetrics] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ContentItemId] INT NOT NULL,
    [Date] DATE NOT NULL,
    [Views] INT NOT NULL DEFAULT 0,
    [UniqueViews] INT NOT NULL DEFAULT 0,
    [TimeOnPage] INT NOT NULL DEFAULT 0, -- Seconds
    [BounceRate] DECIMAL(5, 2) NULL,
    [SharesTotal] INT NOT NULL DEFAULT 0,
    [SharesFacebook] INT NOT NULL DEFAULT 0,
    [SharesTwitter] INT NOT NULL DEFAULT 0,
    [SharesLinkedIn] INT NOT NULL DEFAULT 0,
    [ConversionCount] INT NOT NULL DEFAULT 0,
    [ConversionValue] DECIMAL(18, 2) NULL,
    INDEX [IX_ContentMetrics_ContentItemId_Date] ([ContentItemId], [Date]),
    CONSTRAINT [FK_ContentMetrics_ContentItems] FOREIGN KEY ([ContentItemId]) REFERENCES [ContentItems]([Id])
);

-- ContentComments
CREATE TABLE [dbo].[ContentComments] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ContentItemId] INT NOT NULL,
    [ParentCommentId] INT NULL,
    [AuthorId] NVARCHAR(450) NULL,
    [AuthorName] NVARCHAR(100) NOT NULL,
    [AuthorEmail] NVARCHAR(256) NOT NULL,
    [Comment] NVARCHAR(MAX) NOT NULL,
    [Status] INT NOT NULL, -- 0: Pending, 1: Approved, 2: Spam, 3: Deleted
    [IPAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [LikeCount] INT NOT NULL DEFAULT 0,
    [IsEdited] BIT NOT NULL DEFAULT 0,
    [EditedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_ContentComments_ContentItemId_Status] ([ContentItemId], [Status]),
    CONSTRAINT [FK_ContentComments_ContentItems] FOREIGN KEY ([ContentItemId]) REFERENCES [ContentItems]([Id]),
    CONSTRAINT [FK_ContentComments_Parent] FOREIGN KEY ([ParentCommentId]) REFERENCES [ContentComments]([Id]),
    CONSTRAINT [FK_ContentComments_Author] FOREIGN KEY ([AuthorId]) REFERENCES [AspNetUsers]([Id])
);
```

## API Endpoints

### Content Management

```yaml
/api/v1/cms/content:
  /:
    GET:
      description: Get content items
      parameters:
        type: string # Content type
        status: enum [all, draft, published, scheduled, archived]
        language: string
        category: string
        tags: array[string]
        search: string
        page: number
        pageSize: number
        sort: enum [newest, oldest, popular, trending]
      responses:
        200:
          items: array[ContentItem]
          totalCount: number
          facets: object

    POST:
      description: Create content item
      auth: required (editor)
      body:
        contentType: string
        title: string
        content: object # Structured content
        status: enum [draft, published, scheduled]
        scheduledFor: datetime # If scheduled
        language: string
        tags: array[string]
        categories: array[number]
        seo: object
      responses:
        201:
          contentId: number
          slug: string
          previewUrl: string

  /{contentId}:
    GET:
      description: Get content item
      parameters:
        version: number # Specific version
        preview: boolean
      responses:
        200:
          content: ContentItem
          revisions: array[Revision]
          analytics: ContentAnalytics

    PUT:
      description: Update content item
      auth: required (editor)
      body: ContentUpdateDto
      responses:
        200:
          content: UpdatedContent
          version: number

    DELETE:
      description: Delete content item
      auth: required (editor)
      responses:
        204: Deleted

  /{contentId}/publish:
    POST:
      description: Publish content
      auth: required (editor)
      body:
        publishAt: datetime # Optional
        expiresAt: datetime # Optional
      responses:
        200:
          published: boolean
          url: string

  /{contentId}/revisions:
    GET:
      description: Get content revisions
      auth: required (editor)
      responses:
        200:
          revisions: array[ContentRevision]

  /{contentId}/revert:
    POST:
      description: Revert to previous version
      auth: required (editor)
      body:
        version: number
      responses:
        200:
          content: ContentItem
          newVersion: number

  /{contentId}/translate:
    POST:
      description: Create translation
      auth: required (editor)
      body:
        targetLanguage: string
        autoTranslate: boolean
      responses:
        201:
          translationId: number
          status: enum [draft, translating, completed]

  /search:
    POST:
      description: Advanced content search
      body:
        query: string
        filters: object
        aggregations: array[string]
      responses:
        200:
          results: array[SearchResult]
          total: number
          aggregations: object
          suggestions: array[string]
```

### Media Management

```yaml
/api/v1/cms/media:
  /:
    GET:
      description: Browse media library
      auth: required
      parameters:
        folder: number
        type: enum [all, image, video, document, audio]
        search: string
        tags: array[string]
        page: number
        pageSize: number
      responses:
        200:
          items: array[MediaItem]
          folders: array[MediaFolder]
          totalCount: number

    POST:
      description: Upload media
      auth: required (editor)
      contentType: multipart/form-data
      body:
        files: array[file]
        folder: number
        metadata: object
      responses:
        201:
          uploaded: array[MediaItem]
          errors: array[UploadError]

  /{mediaId}:
    GET:
      description: Get media item
      auth: required
      responses:
        200:
          media: MediaItem
          usage: array[ContentUsage]

    PUT:
      description: Update media metadata
      auth: required (editor)
      body:
        title: string
        altText: string
        caption: string
        tags: array[string]
      responses:
        200: Updated media

    DELETE:
      description: Delete media
      auth: required (editor)
      responses:
        204: Deleted

  /process:
    POST:
      description: Process media (resize, crop, etc.)
      auth: required (editor)
      body:
        mediaId: number
        operations: array[{
          type: enum [resize, crop, rotate, filter]
          parameters: object
        }]
      responses:
        200:
          processedUrl: string
          variants: array[MediaVariant]

  /folders:
    GET:
      description: Get folder structure
      auth: required
      responses:
        200:
          folders: array[FolderTree]

    POST:
      description: Create folder
      auth: required (editor)
      body:
        name: string
        parentId: number
      responses:
        201:
          folderId: number
```

### Templates & Blocks

```yaml
/api/v1/cms/templates:
  /:
    GET:
      description: Get content templates
      auth: required (editor)
      parameters:
        contentType: string
      responses:
        200:
          templates: array[ContentTemplate]

    POST:
      description: Create template
      auth: required (admin)
      body:
        name: string
        contentType: string
        template: string
        fields: object
        styles: string
        scripts: string
      responses:
        201:
          templateId: number

  /{templateId}:
    GET:
      description: Get template details
      auth: required (editor)
      responses:
        200:
          template: TemplateDetail
          preview: string

    PUT:
      description: Update template
      auth: required (admin)
      body: TemplateUpdateDto
      responses:
        200: Updated template

  /preview:
    POST:
      description: Preview template with data
      auth: required (editor)
      body:
        templateId: number
        data: object
      responses:
        200:
          html: string
          css: string

/api/v1/cms/blocks:
  /:
    GET:
      description: Get content blocks
      auth: required (editor)
      parameters:
        type: string
        reusable: boolean
      responses:
        200:
          blocks: array[ContentBlock]

    POST:
      description: Create content block
      auth: required (editor)
      body:
        name: string
        type: string
        content: object
        isReusable: boolean
      responses:
        201:
          blockId: number
```

### SEO & Analytics

```yaml
/api/v1/cms/seo:
  /analyze:
    POST:
      description: Analyze content SEO
      auth: required (editor)
      body:
        contentId: number
        targetKeywords: array[string]
      responses:
        200:
          score: number
          issues: array[SEOIssue]
          suggestions: array[SEOSuggestion]
          keywordDensity: object
          readability: ReadabilityScore

  /sitemap:
    GET:
      description: Generate sitemap
      responses:
        200:
          contentType: application/xml
          body: XML sitemap

  /robots:
    GET:
      description: Get robots.txt
      responses:
        200:
          contentType: text/plain
          body: Robots.txt content

/api/v1/cms/analytics:
  /content/{contentId}:
    GET:
      description: Get content analytics
      auth: required (editor)
      parameters:
        startDate: date
        endDate: date
      responses:
        200:
          views: ViewAnalytics
          engagement: EngagementMetrics
          conversions: ConversionData
          traffic: TrafficSources

  /dashboard:
    GET:
      description: CMS analytics dashboard
      auth: required (editor)
      parameters:
        period: enum [day, week, month, year]
      responses:
        200:
          overview: AnalyticsOverview
          topContent: array[PopularContent]
          trends: TrendData
          authors: AuthorPerformance
```

## Frontend Components

### Content Editor Components (Vue.js)

```typescript
// ContentEditor.vue
interface ContentEditorProps {
  contentId?: number
  contentType: string
  mode: 'create' | 'edit'
}

// Features:
// - Rich text editor (TinyMCE/Quill)
// - Block-based editor
// - Media embedding
// - SEO preview
// - Version control
// - Autosave

// BlockEditor.vue
interface BlockEditorProps {
  blocks: ContentBlock[]
  availableBlockTypes: BlockType[]
  onChange: (blocks: ContentBlock[]) => void
}

// Block types:
// - TextBlock
// - ImageBlock
// - VideoBlock
// - GalleryBlock
// - CodeBlock
// - QuoteBlock
// - CTABlock
// - CustomBlock
```

### Media Library Components

```typescript
// MediaLibrary.vue
interface MediaLibraryProps {
  mode: 'browse' | 'select'
  accept: string[] // File types
  multiple: boolean
  onSelect: (media: MediaItem[]) => void
}

// Features:
// - Grid/list view toggle
// - Folder navigation
// - Drag-drop upload
// - Bulk operations
// - Search & filters
// - Image editing

// MediaUploader.vue
interface MediaUploaderProps {
  folderId?: number
  maxFiles: number
  maxSize: number
  onUpload: (files: MediaItem[]) => void
}

// Features:
// - Chunked upload
// - Progress tracking
// - Pause/resume
// - Metadata editing
```

### Template Builder Components

```typescript
// TemplateBuilder.vue
interface TemplateBuilderProps {
  template?: ContentTemplate
  contentType: string
}

// Features:
// - Visual editor
// - Code editor
// - Variable insertion
// - Live preview
// - Responsive preview

// ContentBlockLibrary.vue
interface ContentBlockLibraryProps {
  onInsert: (block: ContentBlock) => void
}

// Categories:
// - Layout blocks
// - Content blocks
// - Interactive blocks
// - Custom blocks
```

## Technical Implementation Details

### Content Rendering Engine

```csharp
public class ContentRenderingEngine
{
    private readonly ITemplateEngine _templateEngine;
    private readonly IContentRepository _repository;
    private readonly ICacheService _cache;
    
    public async Task<RenderedContent> RenderContent(
        int contentId, 
        RenderContext context)
    {
        var cacheKey = $"rendered:{contentId}:{context.Language}:{context.Device}";
        
        var cached = await _cache.GetAsync<RenderedContent>(cacheKey);
        if (cached != null) return cached;
        
        // Load content
        var content = await _repository.GetContent(contentId);
        
        // Select template
        var template = await SelectTemplate(content, context);
        
        // Prepare data
        var data = PrepareRenderData(content, context);
        
        // Render
        var rendered = await _templateEngine.RenderAsync(template, data);
        
        // Post-process
        rendered = await PostProcess(rendered, context);
        
        // Cache
        await _cache.SetAsync(cacheKey, rendered, GetCacheDuration(content));
        
        return rendered;
    }
    
    private async Task<string> PostProcess(string html, RenderContext context)
    {
        // Lazy load images
        html = AddLazyLoading(html);
        
        // Optimize images
        html = OptimizeImageUrls(html, context.Device);
        
        // Add structured data
        html = AddStructuredData(html, context);
        
        // Minify if production
        if (context.IsProduction)
        {
            html = MinifyHtml(html);
        }
        
        return html;
    }
    
    private string OptimizeImageUrls(string html, DeviceType device)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        var images = doc.DocumentNode.SelectNodes("//img[@src]");
        if (images == null) return html;
        
        foreach (var img in images)
        {
            var src = img.GetAttributeValue("src", "");
            if (string.IsNullOrEmpty(src)) continue;
            
            // Add responsive image parameters
            var optimizedSrc = _imageOptimizer.GetOptimizedUrl(src, new ImageOptions
            {
                Width = GetOptimalWidth(device),
                Format = "webp",
                Quality = 85
            });
            
            img.SetAttributeValue("src", optimizedSrc);
            
            // Add srcset for responsive images
            var srcset = GenerateSrcSet(src, device);
            img.SetAttributeValue("srcset", srcset);
        }
        
        return doc.DocumentNode.OuterHtml;
    }
}
```

### SEO Optimization Service

```csharp
public class SEOOptimizationService
{
    private readonly IKeywordAnalyzer _keywordAnalyzer;
    private readonly IReadabilityAnalyzer _readabilityAnalyzer;
    
    public async Task<SEOAnalysisResult> AnalyzeContent(ContentItem content, SEOTarget target)
    {
        var analysis = new SEOAnalysisResult();
        
        // Title analysis
        analysis.TitleScore = AnalyzeTitle(content.SEOTitle ?? content.Title, target);
        
        // Description analysis
        analysis.DescriptionScore = AnalyzeDescription(content.SEODescription, target);
        
        // Content analysis
        var contentAnalysis = await AnalyzeContentBody(content.Content, target);
        analysis.ContentScore = contentAnalysis.Score;
        analysis.KeywordDensity = contentAnalysis.KeywordDensity;
        
        // Readability
        analysis.Readability = _readabilityAnalyzer.Analyze(content.Content);
        
        // Technical SEO
        analysis.TechnicalIssues = CheckTechnicalSEO(content);
        
        // Calculate overall score
        analysis.OverallScore = CalculateOverallScore(analysis);
        
        // Generate suggestions
        analysis.Suggestions = GenerateSuggestions(analysis, target);
        
        return analysis;
    }
    
    private async Task<ContentAnalysis> AnalyzeContentBody(string content, SEOTarget target)
    {
        var text = StripHtml(content);
        var words = TokenizeText(text);
        
        // Keyword analysis
        var keywordOccurrences = CountKeywords(words, target.Keywords);
        var totalWords = words.Count;
        
        var density = keywordOccurrences
            .ToDictionary(k => k.Key, k => (decimal)k.Value / totalWords * 100);
        
        // Check keyword placement
        var score = 100;
        
        // Keywords in headings
        if (!HasKeywordsInHeadings(content, target.Keywords))
            score -= 10;
        
        // Keywords in first paragraph
        if (!HasKeywordsInFirstParagraph(text, target.Keywords))
            score -= 10;
        
        // Keyword density (2-3% is optimal)
        var primaryDensity = density.GetValueOrDefault(target.Keywords.First(), 0);
        if (primaryDensity < 1 || primaryDensity > 4)
            score -= 15;
        
        // Content length (minimum 300 words)
        if (totalWords < 300)
            score -= 20;
        
        return new ContentAnalysis
        {
            Score = Math.Max(0, score),
            KeywordDensity = density,
            WordCount = totalWords
        };
    }
    
    private List<SEOSuggestion> GenerateSuggestions(SEOAnalysisResult analysis, SEOTarget target)
    {
        var suggestions = new List<SEOSuggestion>();
        
        if (analysis.TitleScore < 80)
        {
            suggestions.Add(new SEOSuggestion
            {
                Type = "title",
                Priority = "high",
                Message = "Include your primary keyword in the title",
                Impact = 10
            });
        }
        
        if (analysis.ContentScore < 70)
        {
            if (analysis.KeywordDensity.GetValueOrDefault(target.Keywords.First(), 0) < 1)
            {
                suggestions.Add(new SEOSuggestion
                {
                    Type = "content",
                    Priority = "medium",
                    Message = "Increase keyword usage in your content",
                    Impact = 15
                });
            }
        }
        
        if (analysis.Readability.Score < 60)
        {
            suggestions.Add(new SEOSuggestion
            {
                Type = "readability",
                Priority = "medium",
                Message = "Simplify your content for better readability",
                Impact = 8,
                Details = new[]
                {
                    $"Average sentence length: {analysis.Readability.AverageSentenceLength} words (aim for 15-20)",
                    $"Complex words: {analysis.Readability.ComplexWordPercentage}% (aim for less than 20%)"
                }
            });
        }
        
        return suggestions.OrderByDescending(s => s.Impact).ToList();
    }
}
```

### Content Versioning System

```csharp
public class ContentVersioningService
{
    private readonly IContentRepository _repository;
    private readonly IDiffService _diffService;
    
    public async Task<ContentRevision> CreateRevision(
        ContentItem content, 
        string changeSummary, 
        string authorId)
    {
        // Get current version
        var currentVersion = await _repository.GetLatestRevision(content.Id);
        
        // Create diff
        var diff = _diffService.CreateDiff(
            currentVersion?.Content ?? "", 
            content.Content);
        
        // Create revision
        var revision = new ContentRevision
        {
            ContentItemId = content.Id,
            Version = (currentVersion?.Version ?? 0) + 1,
            Title = content.Title,
            Content = content.Content,
            ChangeSummary = changeSummary,
            Diff = diff,
            Author = authorId,
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.SaveRevision(revision);
        
        // Update content version
        content.Version = revision.Version;
        await _repository.UpdateContent(content);
        
        // Cleanup old revisions (keep last 50)
        await CleanupOldRevisions(content.Id, 50);
        
        return revision;
    }
    
    public async Task<ContentItem> RevertToVersion(int contentId, int version)
    {
        var revision = await _repository.GetRevision(contentId, version);
        if (revision == null)
            throw new NotFoundException($"Revision {version} not found");
        
        var content = await _repository.GetContent(contentId);
        
        // Create new revision for the revert
        await CreateRevision(
            content, 
            $"Reverted to version {version}", 
            GetCurrentUserId());
        
        // Apply revision content
        content.Title = revision.Title;
        content.Content = revision.Content;
        content.UpdatedAt = DateTime.UtcNow;
        
        await _repository.UpdateContent(content);
        
        return content;
    }
    
    public async Task<List<ContentDiff>> GetVersionDifferences(
        int contentId, 
        int fromVersion, 
        int toVersion)
    {
        var fromRevision = await _repository.GetRevision(contentId, fromVersion);
        var toRevision = await _repository.GetRevision(contentId, toVersion);
        
        return _diffService.GetDetailedDiff(
            fromRevision.Content, 
            toRevision.Content);
    }
}
```

### Content Publishing Pipeline

```csharp
public class ContentPublishingService
{
    private readonly IContentRepository _repository;
    private readonly ICacheService _cache;
    private readonly ISearchIndexer _searchIndexer;
    private readonly INotificationService _notifications;
    
    public async Task<PublishResult> PublishContent(
        int contentId, 
        PublishOptions options)
    {
        var content = await _repository.GetContent(contentId);
        
        // Validate content
        var validation = await ValidateForPublishing(content);
        if (!validation.IsValid)
        {
            return new PublishResult
            {
                Success = false,
                Errors = validation.Errors
            };
        }
        
        // Set publish metadata
        content.Status = ContentStatus.Published;
        content.PublishedAt = options.PublishAt ?? DateTime.UtcNow;
        
        if (options.ExpiresAt.HasValue)
        {
            content.ExpiresAt = options.ExpiresAt.Value;
            await ScheduleExpiration(content);
        }
        
        // Update content
        await _repository.UpdateContent(content);
        
        // Clear caches
        await InvalidateCaches(content);
        
        // Update search index
        await _searchIndexer.IndexContent(content);
        
        // Generate static pages if configured
        if (options.GenerateStaticPage)
        {
            await GenerateStaticPage(content);
        }
        
        // Send notifications
        await NotifySubscribers(content);
        
        // Update sitemap
        await UpdateSitemap();
        
        return new PublishResult
        {
            Success = true,
            PublishedUrl = GeneratePublishedUrl(content)
        };
    }
    
    private async Task<ValidationResult> ValidateForPublishing(ContentItem content)
    {
        var errors = new List<string>();
        
        // Required fields
        if (string.IsNullOrWhiteSpace(content.Title))
            errors.Add("Title is required");
        
        if (string.IsNullOrWhiteSpace(content.Content))
            errors.Add("Content is required");
        
        // SEO validation
        if (string.IsNullOrWhiteSpace(content.SEOTitle))
            errors.Add("SEO title is recommended");
        
        if (string.IsNullOrWhiteSpace(content.SEODescription))
            errors.Add("SEO description is recommended");
        
        // Content quality
        var wordCount = CountWords(content.Content);
        if (wordCount < 300)
            errors.Add("Content should be at least 300 words");
        
        // Check for broken links
        var brokenLinks = await CheckBrokenLinks(content.Content);
        if (brokenLinks.Any())
            errors.Add($"Found {brokenLinks.Count} broken links");
        
        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }
}
```

### AI Content Assistant

```csharp
public class AIContentAssistant
{
    private readonly IGeminiService _geminiService;
    
    public async Task<ContentSuggestions> GenerateSuggestions(
        string topic, 
        ContentType contentType)
    {
        var prompt = $@"
            Generate content suggestions for a {contentType} about '{topic}'.
            Include:
            1. 5 compelling titles
            2. An engaging meta description (150-160 chars)
            3. An outline with main sections
            4. 5 relevant keywords
            5. 3 call-to-action suggestions
            
            Format as JSON.
        ";
        
        var response = await _geminiService.GenerateContent(prompt);
        return JsonSerializer.Deserialize<ContentSuggestions>(response);
    }
    
    public async Task<string> ImproveContent(string content, ImprovementType type)
    {
        var prompt = type switch
        {
            ImprovementType.Readability => $@"
                Improve the readability of this content:
                - Use simpler words where possible
                - Break up long sentences
                - Add transition phrases
                - Maintain the original meaning
                
                Content: {content}
            ",
            ImprovementType.SEO => $@"
                Optimize this content for SEO:
                - Add relevant keywords naturally
                - Improve heading structure
                - Add internal linking suggestions
                - Enhance meta description
                
                Content: {content}
            ",
            ImprovementType.Engagement => $@"
                Make this content more engaging:
                - Add compelling hooks
                - Include questions to engage readers
                - Add storytelling elements
                - Improve call-to-actions
                
                Content: {content}
            ",
            _ => throw new ArgumentException("Invalid improvement type")
        };
        
        return await _geminiService.GenerateContent(prompt);
    }
    
    public async Task<TranslationResult> TranslateContent(
        ContentItem content, 
        string targetLanguage)
    {
        var prompt = $@"
            Translate this content to {targetLanguage}.
            Maintain:
            - SEO keywords relevance
            - Formatting and structure
            - Cultural appropriateness
            - Call-to-action effectiveness
            
            Content:
            Title: {content.Title}
            Body: {content.Content}
            SEO Description: {content.SEODescription}
        ";
        
        var response = await _geminiService.GenerateContent(prompt);
        return ParseTranslationResponse(response);
    }
}
```

## Security Considerations

### Content Security
1. **XSS Prevention**: 
   - HTML sanitization
   - Content Security Policy
   - Template sandboxing
2. **Access Control**:
   - Role-based permissions
   - Content-level permissions
   - Workflow approvals

### API Security
```csharp
[Authorize(Policy = "ContentEditor")]
[ApiController]
[Route("api/v1/cms")]
public class CMSController : ControllerBase
{
    [HttpPost("content")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateContent(CreateContentDto dto)
    {
        // Sanitize HTML content
        dto.Content = _htmlSanitizer.Sanitize(dto.Content, new HtmlSanitizerOptions
        {
            AllowedTags = new[] { "p", "h1", "h2", "h3", "h4", "h5", "h6", 
                                  "strong", "em", "a", "img", "ul", "ol", "li",
                                  "blockquote", "code", "pre" },
            AllowedAttributes = new Dictionary<string, string[]>
            {
                ["a"] = new[] { "href", "title", "target", "rel" },
                ["img"] = new[] { "src", "alt", "title", "width", "height" }
            },
            AllowDataAttributes = false
        });
        
        // Validate file uploads
        if (dto.FeaturedImage != null)
        {
            if (!IsValidImage(dto.FeaturedImage))
                return BadRequest("Invalid image file");
        }
        
        var content = await _contentService.CreateContent(dto, User.GetUserId());
        return Created($"/api/v1/cms/content/{content.Id}", content);
    }
}
```

## Performance Optimization

### Content Caching Strategy
```csharp
public class ContentCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ICDNService _cdn;
    
    public async Task<CachedContent> GetCachedContent(
        string slug, 
        string language, 
        DeviceType device)
    {
        var cacheKey = $"content:{slug}:{language}:{device}";
        
        // L1: Memory cache
        if (_memoryCache.TryGetValue(cacheKey, out CachedContent cached))
            return cached;
        
        // L2: Redis cache
        var redisCached = await _cache.GetAsync<CachedContent>(cacheKey);
        if (redisCached != null)
        {
            _memoryCache.Set(cacheKey, redisCached, TimeSpan.FromMinutes(5));
            return redisCached;
        }
        
        // Generate and cache
        var content = await GenerateContent(slug, language, device);
        
        // Cache in Redis
        await _cache.SetAsync(cacheKey, content, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
            SlidingExpiration = TimeSpan.FromHours(2)
        });
        
        // Cache in memory
        _memoryCache.Set(cacheKey, content, TimeSpan.FromMinutes(5));
        
        // Push to CDN edge locations
        await _cdn.PushToEdge(cacheKey, content);
        
        return content;
    }
    
    public async Task InvalidateContent(ContentItem content)
    {
        var patterns = new[]
        {
            $"content:{content.Slug}:*",
            $"rendered:{content.Id}:*",
            $"api:content:{content.Id}:*"
        };
        
        foreach (var pattern in patterns)
        {
            await _cache.RemoveByPatternAsync(pattern);
        }
        
        // Invalidate CDN
        await _cdn.PurgeByTag($"content-{content.Id}");
    }
}
```

### Search Optimization
```csharp
public class ContentSearchService
{
    private readonly IElasticsearchClient _elasticsearch;
    
    public async Task IndexContent(ContentItem content)
    {
        var document = new ContentDocument
        {
            Id = content.Id,
            Title = content.Title,
            Content = StripHtml(content.Content),
            Excerpt = content.Excerpt,
            Tags = content.Tags,
            Categories = content.Categories,
            Author = content.Author,
            PublishedAt = content.PublishedAt,
            Language = content.Language,
            ContentType = content.ContentType,
            // Computed fields for relevance
            Popularity = CalculatePopularity(content),
            Freshness = CalculateFreshness(content)
        };
        
        await _elasticsearch.IndexAsync(document, idx => idx
            .Index("content")
            .Pipeline("content-pipeline"));
    }
    
    public async Task<SearchResults> Search(ContentSearchRequest request)
    {
        var searchResponse = await _elasticsearch.SearchAsync<ContentDocument>(s => s
            .Index("content")
            .Query(q => q
                .Bool(b => b
                    .Must(BuildMustQueries(request))
                    .Filter(BuildFilterQueries(request))
                    .Should(BuildBoostQueries(request))
                )
            )
            .Aggregations(a => a
                .Terms("categories", t => t.Field(f => f.Categories))
                .Terms("tags", t => t.Field(f => f.Tags))
                .DateHistogram("timeline", d => d
                    .Field(f => f.PublishedAt)
                    .Interval(DateInterval.Month)
                )
            )
            .Sort(BuildSortDescriptor(request))
            .From((request.Page - 1) * request.PageSize)
            .Size(request.PageSize)
            .Highlight(h => h
                .Fields(f => f
                    .Field(c => c.Content)
                    .PreTags("<mark>")
                    .PostTags("</mark>")
                )
            )
        );
        
        return MapSearchResponse(searchResponse);
    }
}
```

## Testing Strategy

### Content Management Testing
```csharp
[TestClass]
public class ContentServiceTests
{
    [TestMethod]
    public async Task PublishContent_WithScheduledDate_SchedulesPublication()
    {
        // Arrange
        var content = new ContentItem
        {
            Title = "Test Article",
            Status = ContentStatus.Draft
        };
        
        var publishDate = DateTime.UtcNow.AddHours(2);
        
        // Act
        var result = await _contentService.PublishContent(content.Id, new PublishOptions
        {
            PublishAt = publishDate
        });
        
        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(ContentStatus.Scheduled, content.Status);
        Assert.AreEqual(publishDate, content.ScheduledFor);
    }
}
```

### SEO Testing
```csharp
[TestClass]
public class SEOAnalysisTests
{
    [TestMethod]
    public void AnalyzeKeywordDensity_WithOptimalDensity_ReturnsHighScore()
    {
        // Arrange
        var content = "SEO is important. Good SEO practices help. " +
                     string.Join(" ", Enumerable.Repeat("other words", 50)) +
                     " SEO matters for ranking.";
        
        var target = new SEOTarget { Keywords = new[] { "SEO" } };
        
        // Act
        var analysis = _seoService.AnalyzeContent(content, target);
        
        // Assert
        Assert.IsTrue(analysis.KeywordDensity["SEO"] >= 2);
        Assert.IsTrue(analysis.KeywordDensity["SEO"] <= 3);
        Assert.IsTrue(analysis.ContentScore >= 80);
    }
}
```

## Monitoring & Analytics

### Content Performance Metrics
```csharp
public class ContentAnalyticsService
{
    public async Task TrackContentView(int contentId, ViewContext context)
    {
        // Real-time tracking
        await _analytics.TrackEvent("ContentView", new
        {
            ContentId = contentId,
            UserId = context.UserId,
            SessionId = context.SessionId,
            Device = context.Device,
            Referrer = context.Referrer,
            TimeOnPage = context.TimeOnPage
        });
        
        // Update aggregated metrics
        await UpdateContentMetrics(contentId, context.Date);
        
        // Check for milestones
        var viewCount = await GetViewCount(contentId);
        if (viewCount % 1000 == 0)
        {
            await NotifyContentMilestone(contentId, viewCount);
        }
    }
}
```