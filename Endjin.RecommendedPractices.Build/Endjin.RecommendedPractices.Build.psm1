function Assert-DotNetTool
{
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [string] $Name,

        [Parameter()]
        [Version] $Version,
        
        [Parameter()]
        [bool] $Global = $true
    )

    $existingTools = & dotnet tool list -g
    
    if ( !($existingTools | select-string $Name) ) {
        & dotnet tool install -g $Name --version "$Version"
    }
}

Set-Alias Endjin.RecommendedPractices.Build.tasks $PSScriptRoot/Endjin.RecommendedPractices.Build.tasks.ps1
Export-ModuleMember -Function Assert-DotNetTool `
                    -Alias Endjin.RecommendedPractices.Build.tasks