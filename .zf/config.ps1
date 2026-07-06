<#
Build process configuration for vellum-cli, using the 'ZeroFailed.Build.DotNet' extension
to provide the features needed when building .NET solutions.
#>

$zerofailedExtensions = @(
    @{
        # References the extension from its GitHub repository. If not already installed, the latest version from 'main' will be downloaded.
        Name = "ZeroFailed.Build.DotNet"
        GitRepository = "https://github.com/zerofailed/ZeroFailed.Build.DotNet"
        GitRef = "main"
    }
)

# Load the tasks and process
. ZeroFailed.tasks -ZfPath $here/.zf


#
# Build process control options
#
$SkipInit = $false
$SkipVersion = $false
$SkipBuild = $false
$CleanBuild = $Clean
$SkipTest = $false
$SkipTestReport = $false
$SkipAnalysis = $false
$SkipPackage = $false

#
# Build process configuration
#
$SolutionToBuild = (Resolve-Path (Join-Path $here ".\Solutions\Vellum.Cli.slnx")).Path
$ProjectsToPublish = @(
    "Solutions/Vellum.Cli.Cloudinary/Vellum.Cli.Cloudinary.csproj"
    "Solutions/Vellum.Cli.Tinify/Vellum.Cli.Tinify.csproj"
)
$NuSpecFilesToPackage = @(
    "Solutions/Vellum.Cli.Cloudinary/Vellum.Cli.Cloudinary.nuspec"
    "Solutions/Vellum.Cli.Tinify/Vellum.Cli.Tinify.nuspec"
)
$NugetPublishSource = property ZF_NUGET_PUBLISH_SOURCE "$here/_local-nuget-feed"
$IncludeAssembliesInCodeCoverage = "Vellum*"


# Synopsis: Build, Test and Package
task . FullBuild

#
# Build Process Extensibility Points - uncomment and implement as required
#

# task RunFirst {}
# task PreInit {}
# task PostInit {}
# task PreVersion {}
# task PostVersion {}
# task PreBuild {}
# task PostBuild {}
# task PreTest {}
# task PostTest {}
# task PreTestReport {}
# task PostTestReport {}
# task PreAnalysis {}
# task PostAnalysis {}
# task PrePackage {}
# task PostPackage {}
# task PrePublish {}
# task PostPublish {}
# task RunLast {}
