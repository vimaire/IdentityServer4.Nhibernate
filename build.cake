var target = Argument("target", "Default");
var version = Argument("libversion","1.0.0");
var configuration = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var packPath = Directory("./src/IdentityServer4.NHibernate");
var buildArtifacts = Directory("./artifacts/packages");

var isAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var isWindows = IsRunningOnWindows();

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings 
    {
        Configuration = configuration,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
                                    .SetVersion(version)
    };

    var projects = GetFiles("./src/**/*.csproj");

    foreach(var project in projects)
	{
	    DotNetCoreBuild(project.GetDirectory().FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Test
///////////////////////////////////////////////////////////////////////////////
Task("Test")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration
    };

    if (!isWindows)
    {
        Information("Not running on Windows - skipping tests for .NET Framework");
        settings.Framework = "netcoreapp2.0";
    }

    var projects = GetFiles("./test/**/*.csproj");
    foreach(var project in projects)
    {
        DotNetCoreTest(project.FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Pack
///////////////////////////////////////////////////////////////////////////////
Task("Pack")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
        ArgumentCustomization = args => args.Append("--include-symbols"),
        MSBuildSettings = new DotNetCoreMSBuildSettings()
                                    .SetVersion(version)
    };

    // add build suffix for CI builds
    if(isAppVeyor)
    {
        settings.VersionSuffix = "build" + AppVeyor.Environment.Build.Number.ToString().PadLeft(5,'0');
    }

    DotNetCorePack(packPath, settings);
});


Task("Default")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("Pack");

Task("Debug")
.Does(() =>{
    Information($"Target: {target}");
    Information($"Version: {version}");
    Information($"Configuration: {configuration}");
});

RunTarget(target);