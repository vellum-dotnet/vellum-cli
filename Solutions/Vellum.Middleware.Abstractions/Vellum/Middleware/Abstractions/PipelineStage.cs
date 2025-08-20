namespace Vellum.Middleware.Abstractions;

/// <summary>
/// Defines the stages in the Vellum pipeline execution.
/// </summary>
public enum PipelineStage
{
    /// <summary>
    /// Initialization stage for setting up the pipeline.
    /// </summary>
    Initialization = 100,

    /// <summary>
    /// Pre-discovery stage for preparation before content discovery.
    /// </summary>
    PreDiscovery = 200,

    /// <summary>
    /// Discovery stage for finding content files.
    /// </summary>
    Discovery = 300,

    /// <summary>
    /// Post-discovery stage for processing after content discovery.
    /// </summary>
    PostDiscovery = 400,

    /// <summary>
    /// Loading stage for loading content from discovered files.
    /// </summary>
    Loading = 450,

    /// <summary>
    /// Pre-parsing stage for preparation before content parsing.
    /// </summary>
    PreParsing = 500,

    /// <summary>
    /// Parsing stage for processing content files.
    /// </summary>
    Parsing = 600,

    /// <summary>
    /// Post-parsing stage for processing after content parsing.
    /// </summary>
    PostParsing = 700,

    /// <summary>
    /// Pre-mapping stage for preparation before content mapping.
    /// </summary>
    PreMapping = 800,

    /// <summary>
    /// Mapping stage for mapping content to structured data.
    /// </summary>
    Mapping = 900,

    /// <summary>
    /// Post-mapping stage for processing after content mapping.
    /// </summary>
    PostMapping = 1000,

    /// <summary>
    /// Pre-rendering stage for preparation before content rendering.
    /// </summary>
    PreRendering = 1100,

    /// <summary>
    /// Rendering stage for generating output from content.
    /// </summary>
    Rendering = 1200,

    /// <summary>
    /// Post-rendering stage for processing after content rendering.
    /// </summary>
    PostRendering = 1300,

    /// <summary>
    /// Pre-output stage for preparation before writing output.
    /// </summary>
    PreOutput = 1400,

    /// <summary>
    /// Output stage for writing generated content to files.
    /// </summary>
    Output = 1500,

    /// <summary>
    /// Post-output stage for processing after writing output.
    /// </summary>
    PostOutput = 1600,

    /// <summary>
    /// Finalization stage for cleanup and final processing.
    /// </summary>
    Finalization = 1700
}