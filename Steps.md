Program
  Detect culture override environment variable, if present set
  Command Line Parser (passed console args)
    - set up generate Command

GenerateHandler.ExecuteAsync
    - Read Site Template
    - Configure Services, including Razor ViewEngine
    - Create IHost
    - Resolve ContentTasks
    - Execute Generate Task

ContentTasks.Generate
    - Set CDN Options
    - Start Razor ViewEngine warmup task
    - If in preview mode, spin up WebHost to serve content
    - If in watch mode, configure SystemFileWatcher (which calls GenerateInternal()), then call GenerateInternal(), and wait on a loop.

ContentTasks.GenerateInternal()
    - Flush Cache
    - Load Template File MetaData
    - Load Page Template Meta Data
    - Convert into PagesContexts
    - Convert Site Taxonomy into Site Navigation
    - Discover Asset Path
    - Create SiteContext
    - If in Preview Mode, copy assets to the output directory and call CompileSass()
    - Call RenderSiteAsync
  
 ContentTasks.CompileSass()
    - Set ScssOptions, import files
    - Add paths
    - Read top level sass file
    - Compile
    - Write rendered output and map.
  
 ContentTasks.RenderSiteAsync()
    - Convert from PageContext to RenderItems via Mappers
    - Generate MoreLikeThese recommendations
    - If watch mode, call cachingTasks.GenerateAndPersistSiteHashMapIfNotExistsAsync & cachingTasks.DetectChangedPagesAsync and filter to changes files.
    - Filter RenderItems to be distinct and publishable
    - Convert into StructuredItems (via mappers)
    - Generate Syndication Feeds (RSS / ATOM / Google News Sitemap, Google Video Sitemap, Google Sitemap)
    - Wait for warmup task to complete
    - Resolve Renderer for PageContext content type
    - Invoke Rendering Dataflow