var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Debug");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var packPath1            = Directory("./src/ExtCore.Infrastructure");
var packPath2            = Directory("./src/ExtCore.WebApplication");
var buildArtifacts       = Directory("./artifacts/packages");

var isAppVeyor          = AppVeyor.IsRunningOnAppVeyor;
var isWindows           = IsRunningOnWindows();
var netcore             = "netcoreapp1.0";
var netstandard         = "netstandard1.6";

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

///////////////////////////////////////////////////////////////////////////////
// Restore
///////////////////////////////////////////////////////////////////////////////
Task("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreRestoreSettings
    {
        Sources = new [] { "https://api.nuget.org/v3/index.json" }
    };

    var projects = GetFiles("./**/*.csproj");

	foreach(var project in projects)
	{
	    DotNetCoreRestore(project.GetDirectory().FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings 
    {
        Configuration = configuration
    };

    // libraries
	var projects = GetFiles("./src/**/*.csproj");

    if (!isWindows)
    {
        Information("Not on Windows - building only for " + netstandard);
        settings.Framework = netstandard;
    }

	foreach(var project in projects)
	{
	    DotNetCoreBuild(project.GetDirectory().FullPath, settings); 
    }

    // tests
	projects = GetFiles("./test/**/*.csproj");

    if (!isWindows)
    {
        Information("Not on Windows - building only for " + netcore);
        settings.Framework = netcore;
    }

	foreach(var project in projects)
	{
	    DotNetCoreBuild(project.GetDirectory().FullPath, settings); 
    }
});

///////////////////////////////////////////////////////////////////////////////
// Test
///////////////////////////////////////////////////////////////////////////////
Task("Test")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration
    };

    var projects = GetFiles("./test/**/*.csproj");

    if (!isWindows)
    {
        Information("Not on Windows - testing only for " + netcore);
        settings.Framework = netcore;
    }

    foreach(var project in projects)
	{
        DotNetCoreTest(project.FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Pack
///////////////////////////////////////////////////////////////////////////////
Task("Pack")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    if (!isWindows)
    {
        Information("Not on Windows - skipping pack");
        return;
    }

    var settings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
		
		// 外部コードのステップインを許可する
		// ［ツール］−［オプション］−［デバッグ］−［全般］ノードにて、『マイコードのみを有効にする』のチェックを外して無効にする。
		// symbols.nupkgにpdb,srcからデバッグが可能になる。
		IncludeSource = true,
		IncludeSymbols = true,
    };

    // add build suffix for CI builds
    if(isAppVeyor)
    {
        settings.VersionSuffix = "build" + AppVeyor.Environment.Build.Number.ToString().PadLeft(5,'0');
    }

    DotNetCorePack(packPath1, settings);
    DotNetCorePack(packPath2, settings);

    //pack
    var nuGetPackSettings   = new NuGetPackSettings {
                                Id                      = "MyStandard.Library",
                                Version                 = "1.0.0-alpha1",
                                Title                   = "MyStandard.Library",
                                Authors                 = new[] {"f7q"},
                                Owners                  = new[] {"f7q"},
                                Description             = "A set of standard My APIs.",
                                ProjectUrl              = new Uri("https://github.com/f7q/ExtCore-Sample"),
                                IconUrl                 = new Uri("http://go.microsoft.com/fwlink/?LinkID=288859"),
                                LicenseUrl              = new Uri("https://github.com/f7q/ExtCore-Sample/blob/dev/LICENSE.txt"),
                                Copyright               = "(c)f7q. All rights reserved. Copyright 2017",
                                ReleaseNotes            = new [] {"https://github.com/f7q/ExtCore-Sample/releases"},
                                Tags                    = new [] {"learnning"},
                                RequireLicenseAcceptance= false,
                                OutputDirectory         = buildArtifacts
                            };

     NuGetPack("./nuspec/MyStandard.Library.nuspec", nuGetPackSettings);
});


Task("Default")
  .IsDependentOn("Build")
  //.IsDependentOn("Test")
  .IsDependentOn("Pack");

RunTarget(target);