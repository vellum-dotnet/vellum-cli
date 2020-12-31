task GitVersion {
    Assert-DotNetTool -Name "GitVersion.Tool" -Version 5.2.0
    $gitVersionOutputJson = exec { dotnet-gitversion /output json /nofetch }
    $env:GitVersionOutput = $gitVersionOutputJson
    $script:GitVersion = $gitVersionOutputJson | ConvertFrom-Json -AsHashtable
    
    Write-Build Green "##vso[task.setvariable variable=GitVersion.SemVer]$($script:GitVersion.SemVer)"
    Write-Build Green "##vso[task.setvariable variable=GitVersion.PreReleaseTag]$($script:GitVersion.PreReleaseTag)"
    Write-Build Green "##vso[task.setvariable variable=GitVersion.MajorMinorPatch]$($script:GitVersion.MajorMinorPatch)"
}

task Build {
    exec { 
        dotnet build $SolutionToBuild `
                     --configuration $Configuration `
                     /p:Version="$(($script:GitVersion).SemVer)" `
                     /p:EndjinRepositoryUrl="$BuildRepositoryUri" `
                     --verbosity $LogLevel
    }
}

task Test {
    exec { 
        dotnet test $SolutionToBuild `
                    --configuration $Configuration `
                    --no-build `
                    --no-restore `
                    /p:CollectCoverage=true `
                    /p:CoverletOutputFormat=cobertura `
                    --verbosity $LogLevel
    }
}

task TestReport {
    Assert-DotNetTool -Name "dotnet-reportgenerator-globaltool" -Version 4.8.3
    exec {
        reportgenerator "-reports:$SourcesDir/**/**/coverage.cobertura.xml" `
                        "-targetdir:$CoverageDir" `
                        "-reporttypes:$TestReportTypes"
    }
}

task Package {
    exec { 
        dotnet pack $SolutionToBuild `
                    --configuration $Configuration `
                    --no-build `
                    --no-restore `
                    --output $PackagesDir `
                    /p:EndjinRepositoryUrl="BuildRepositoryUri" `
                    /p:PackageVersion="$(($script:GitVersion).SemVer)" `
                    --verbosity $LogLevel
    }
}

task FullBuild GitVersion,Build,Test,TestReport,Package