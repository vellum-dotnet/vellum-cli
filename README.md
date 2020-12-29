# vellum-cli
[![Build Status](https://dev.azure.com/endjin-labs/vellum.cli/_apis/build/status/vellum-dotnet.vellum.cli?branchName=master)](https://dev.azure.com/endjin-labs/vellum.cli/_build/latest?definitionId=4&branchName=master)
[![GitHub license](https://img.shields.io/badge/License-Apache%202-blue.svg)](https://raw.githubusercontent.com/vellum-dotnet/vellum-cli/master/LICENSE)
[![IMM](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum.cli/total?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/total?cache=false)

`Vellum` is a Static Content Management System, available as a .NET Global Tool, and is built using Microsoft's `System.CommandLine` [libraries](https://github.com/dotnet/command-line-api). These packages, while still marked as experimental, are seeing lots of real-world usage, including tools such as [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) and [].NET Interactive](https://github.com/dotnet/interactive). A useful blog post for understanding `System.CommandLine` is [Radu Matei's](https://twitter.com/matei_radu) blog post "[Building self-contained, single executable .NET Core 3 CLI tools](https://radu-matei.com/blog/self-contained-dotnet-cli/)".

## dotnet global tools

`vellum` is a [.NET global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools), which means once installed, it's available on the PATH of your machine. 

To list all the global tools installed on your machine, open a command prompt and type:

`dotnet tool list -g`

To install the `vellum` global tool use the following command:

`dotnet tool install -g vellum`

To install a specific version, use:

`dotnet tool install -g vellum --version <version-number>`

To update to the latest version of the tool, use:

`dotnet tool update -g vellum`

To uninstall the tool, use:

`dotnet tool uninstall -g vellum`

## dotnet-suggest

`vellum` supports [dotnet suggest](https://github.com/dotnet/command-line-api/wiki/dotnet-suggest), for tab based auto completion.

To install dotnet suggest:

`dotnet tool install -g dotnet-suggest`

Next check if you have a PowerShell profile configured, by opening a PowerShell prompt and typing the following:

`echo $profile`

You should see something like:

`$ENV:USERPROFILE\Documents\PowerShell\Microsoft.PowerShell_profile.ps1`

If you don't see such a file run the following command:

`Invoke-WebRequest -Uri https://raw.githubusercontent.com/dotnet/command-line-api/main/src/System.CommandLine.Suggest/dotnet-suggest-shim.ps1 -OutFile $profile`

Otherwise, copy the contents of the file above and paste it into your pre-existing profile.

## Commands

Once you have `dotnet-suggest` installed, you can use `vellum` then TAB to explore the available commands. Here is a detailed list of the available commands:

`vellum environment` - Manipulate the vellum environment & settings. Will list available sub-commands.

`vellum environment init` - Initialize the environment & settings.

`vellum environment set --username <USER.NAME>` - Sets the current User's username.

`vellum environment set --workspace-path <PATH>` - Sets the path to your vellum workspace.

`vellum environment set --publish-path <PATH>` - Sets the path to where your artefacts are generated.

`vellum environment set --key <KEY> --value <VALUE>` - Store key value pairs in configuration.

`vellum plugins install <PACKAGE ID>` - Install a vellum plugin.

`vellum plugins uninstall <PACKAGE ID>` - Uninstall a vellum plugin.

NOT IMPLEMENTED YET `vellum plugins list available` - Lists available vellum plugins from the default package repository (nuget.org).

`vellum plugins list installed` - Lists installed vellum plugins.

`vellum new <TEMPLATE NAME> [--path <PATH>]` - Will create a new file based on the template name selected. The location is derived by convention based on the template content-type, but can be overriden by the `--path` option. 

`vellum content list --published`

`vellum content list --draft`

`vellum content schedule`
## Plugins

`vellum` supports external plugins.

### Cloudinary

[Cloudinary](https://cloudinary.com/) is a Content Delivery Network that also offers sophiticated APIs for manipulating media. 

`vellum cloudinary settings list` - lists the current settings.

`vellum cloudinary settings update <CLOUD> <KEY> <SECRET>` - update the settings for Cloudinary authentication.

`vellum cloudinary upload <FILE PATH>` - uploads the file to `assets/images/blog/<YYYY>/<MM>/<lowercase_file_name>` and will return you the full public path.

### Tinify

[Tinify](https://tinypng.com/) is an API for optimising PNG and JPG image formats.

To use, first you need to register for an [API Key](https://tinypng.com/developers), this will allow you to process 500 images per month.

`vellum tinify settings update <KEY>` - updates tinify setting with your API Key.

`vellum tinify settings list ` - lists your tinify settings.

## Licenses

[![GitHub license](https://img.shields.io/badge/License-Apache%202-blue.svg)](https://raw.githubusercontent.com/corvus-dotnet/vellum.cli/master/LICENSE)

vellum.cli is available under the Apache 2.0 open source license.

For any licensing questions, please email [&#108;&#105;&#99;&#101;&#110;&#115;&#105;&#110;&#103;&#64;&#101;&#110;&#100;&#106;&#105;&#110;&#46;&#99;&#111;&#109;](&#109;&#97;&#105;&#108;&#116;&#111;&#58;&#108;&#105;&#99;&#101;&#110;&#115;&#105;&#110;&#103;&#64;&#101;&#110;&#100;&#106;&#105;&#110;&#46;&#99;&#111;&#109;)

## Project Sponsor

This project is sponsored by [endjin](https://endjin.com), a UK based Microsoft Gold Partner for Cloud Platform, Data Platform, Data Analytics, DevOps, and a Power BI Partner. Endjin helps small teams achieve big things.

For more information about our products and services, or for commercial support of this project, please [contact us](https://endjin.com/contact-us). 

We produce two free weekly newsletters; [Azure Weekly](https://azureweekly.info) for all things about the Microsoft Azure Platform, and [Power BI Weekly](https://powerbiweekly.info).

Keep up with everything that's going on at endjin via our [blog](https://blogs.endjin.com/), follow us on [Twitter](https://twitter.com/endjin), or [LinkedIn](https://www.linkedin.com/company/1671851/).

Our other Open Source projects can be found on GitHub on our [endjin](https://github.com/endjin), [Corvus](https://github.com/corvus-dotnet), [Menes](https://github.com/menes-dotnet), [Marain](https://github.com/marain-dotnet), and [AIS.NET](https://github.com/ais-net) Orgs.

## Code of conduct

This project has adopted a code of conduct adapted from the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community. This code of conduct has been [adopted by many other projects](http://contributor-covenant.org/adopters/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [&#104;&#101;&#108;&#108;&#111;&#064;&#101;&#110;&#100;&#106;&#105;&#110;&#046;&#099;&#111;&#109;](&#109;&#097;&#105;&#108;&#116;&#111;:&#104;&#101;&#108;&#108;&#111;&#064;&#101;&#110;&#100;&#106;&#105;&#110;&#046;&#099;&#111;&#109;) with any additional questions or comments.

## IP Maturity Model (IMM)

The IMM is endjin's IP quality framework.

[![Shared Engineering Standards](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/74e29f9b-6dca-4161-8fdd-b468a1eb185d?nocache=true)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/74e29f9b-6dca-4161-8fdd-b468a1eb185d?cache=false)

[![Coding Standards](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/f6f6490f-9493-4dc3-a674-15584fa951d8?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/f6f6490f-9493-4dc3-a674-15584fa951d8?cache=false)

[![Executable Specifications](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/bb49fb94-6ab5-40c3-a6da-dfd2e9bc4b00?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/bb49fb94-6ab5-40c3-a6da-dfd2e9bc4b00?cache=false)

[![Code Coverage](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/0449cadc-0078-4094-b019-520d75cc6cbb?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/0449cadc-0078-4094-b019-520d75cc6cbb?cache=false)

[![Benchmarks](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/64ed80dc-d354-45a9-9a56-c32437306afa?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/64ed80dc-d354-45a9-9a56-c32437306afa?cache=false)

[![Reference Documentation](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/2a7fc206-d578-41b0-85f6-a28b6b0fec5f?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/2a7fc206-d578-41b0-85f6-a28b6b0fec5f?cache=false)

[![Design & Implementation Documentation](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/f026d5a2-ce1a-4e04-af15-5a35792b164b?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/f026d5a2-ce1a-4e04-af15-5a35792b164b?cache=false)

[![How-to Documentation](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/145f2e3d-bb05-4ced-989b-7fb218fc6705?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/145f2e3d-bb05-4ced-989b-7fb218fc6705?cache=false)

[![Date of Last IP Review](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/da4ed776-0365-4d8a-a297-c4e91a14d646?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/da4ed776-0365-4d8a-a297-c4e91a14d646?cache=false)

[![Framework Version](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/6c0402b3-f0e3-4bd7-83fe-04bb6dca7924?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/6c0402b3-f0e3-4bd7-83fe-04bb6dca7924?cache=false)

[![Associated Work Items](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/79b8ff50-7378-4f29-b07c-bcd80746bfd4?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/79b8ff50-7378-4f29-b07c-bcd80746bfd4?cache=false)

[![Source Code Availability](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/30e1b40b-b27d-4631-b38d-3172426593ca?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/30e1b40b-b27d-4631-b38d-3172426593ca?cache=false)

[![License](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/d96b5bdc-62c7-47b6-bcc4-de31127c08b7?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/d96b5bdc-62c7-47b6-bcc4-de31127c08b7?cache=false)

[![Production Use](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/87ee2c3e-b17a-4939-b969-2c9c034d05d7?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/87ee2c3e-b17a-4939-b969-2c9c034d05d7?cache=false)

[![Insights](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/71a02488-2dc9-4d25-94fa-8c2346169f8b?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/71a02488-2dc9-4d25-94fa-8c2346169f8b?cache=false)

[![Packaging](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/547fd9f5-9caf-449f-82d9-4fba9e7ce13a?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/547fd9f5-9caf-449f-82d9-4fba9e7ce13a?cache=false)

[![Deployment](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/edea4593-d2dd-485b-bc1b-aaaf18f098f9?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/vellum-dotnet/vellum-cli/rule/edea4593-d2dd-485b-bc1b-aaaf18f098f9?cache=false)