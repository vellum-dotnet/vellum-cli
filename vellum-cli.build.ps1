param (
    $Configuration = "Release",
    $BuildRepositoryUri = "",
    $SourcesDir = $PWD,
    $CoverageDir = "_codeCoverage",
    $TestReportTypes = "Cobertura",
    $PackagesDir = "_packages",
    $LogLevel = "minimal"
)
Get-Module Endjin.RecommendedPractices.Build | Remove-Module -Force
Import-Module ./Endjin.RecommendedPractices.Build -Verbose
. Endjin.RecommendedPractices.Build.tasks

# build variables
$SolutionToBuild = "Solutions/Vellum.Cli.sln"
$PluginsToPackage = @(
    "Solutions/Vellum.Cli.Cloudinary/Vellum.Cli.Cloudinary.csproj"
    "Solutions/Vellum.Cli.Tinify/Vellum.Cli.Tinify.csproj"
)

# Synopsis: Build, Test and Package
task . FullBuild

# extensibility targets
task PreBuild -Before Build
task PostBuild -After Build
task PreTest -Before Test
task PostTest -After Test
task PreTestReport -Before TestReport
task PostTestReport -After TestReport
task PrePackage -Before Package
task PostPackage -After Package {
    # custom nuget packaging for plugins
    $semVer = 
    Write-Build Green "SemVer: $semver"
    foreach ($project in $PluginsToPackage) {
        exec {
            dotnet publish $project `
                           --configuration $Configuration `
                           --no-build `
                           --no-restore `
                           /p:Version="$(($script:GitVersion).SemVer)" `
                           /p:EndjinRepositoryUrl="$BuildRepositoryUri" `
                           --verbosity $LogLevel
        }

        # patch the version number in the .nuspec file
        $nuspecFile = $project -replace "\.csproj",".nuspec"
        Write-Build Gray "Patching version number in: $nuspecFile"
        $nuspec = [xml](Get-Content $nuspecFile)
        $nuspec.package.metadata.version = $script:GitVersion.SemVer
        $nuspec.Save($nuspecFile)

        exec {
            dotnet pack $project `
                        --configuration $Configuration `
                        --no-build `
                        --no-restore `
                        --output $PackagesDir `
                        /p:EndjinRepositoryUrl="$BuildRepositoryUri" `
                        /p:IsPackable=true `
                        /p:IncludeSymbols=false `
                        /p:NoWarn=NU5100 `
                        --verbosity $LogLevel
        }
    }
}