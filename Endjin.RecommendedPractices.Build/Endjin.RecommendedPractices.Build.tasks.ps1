task GitVersion {
    Assert-DotNetTool -Name "GitVersion.Tool" -Version 5.2.0
    $gitVersionOutputJson = exec { dotnet-gitversion /output json /nofetch }
    $env:GitVersionOutput = $gitVersionOutputJson
    $script:GitVersion = $gitVersionOutputJson | ConvertFrom-Json -AsHashtable

    # Set the native GitVersion output as environment variables
    foreach ($var in $script:GitVersion.Keys) {
        Set-Item -Path "env:GITVERSION_$var" -Value $script:GitVersion[$var]
    }

    # Starting with GitVersion 5.2.0, GitVersion includes isOutput=true on all of the variables it
    # sets, which is a breaking change. It means that these variables are no longer available with
    # the same name - they must now be qualified by the name of the task that produced them.
    # You can't turn this off, so we have to adapt to it. This just republishes them by their
    # old names, meaning anything previously depending on those names will continue to work.
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