# NuGet Package Dependency Finder

Quick dirty way to find out NuGet package dependency chain. This is useful to find out all dependencies that are needed in the NuSpec file since there's no good way for `dotnet pack` to generate the info properly.

This is more of a hack. MSBuild might be a better place to do a task like this. But at least this will help find out missing dependency from the nuspec files.

Usage:

```shell
dotnet run "fullPathToYourProject.csproj"
```

For example

```shell
PS D:\Repos\PackageDependencyFinder> dotnet run .\PackageDependencyFinder.csproj

Brief
microsoft.extensions.logging                                                   5.0.0 
About to output detailed reference info. Press any key to continue. Ctrl + C to break
Details:
microsoft.extensions.logging                                            5.0.0           .\PackageDependencyFinder.csproj
```
