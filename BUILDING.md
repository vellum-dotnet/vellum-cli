# Building vellum-cli

This project uses the [`InvokeBuild`](https://github.com/nightroman/Invoke-Build) PowerShell module for build automation.

It can be installed via the [PowerShell Gallery](https://www.powershellgallery.com/packages/InvokeBuild/), for example:

```
Install-Module InvokeBuild -Scope CurrentUser -Force -Repository PSGallery
```

From the root of this repo, run the build as follows:

```
Invoke-Build ./vellum-cli.build.ps1 -Task .
```