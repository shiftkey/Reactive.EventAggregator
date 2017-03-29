#tool nuget:?package=xunit.runner.console&version=2.1.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/Reactive.EventAggregator/Reactive.EventAggregator/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./src/Reactive.EventAggregator.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./src/Reactive.EventAggregator.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./src/Reactive.EventAggregator.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit2("./src/**/bin/" + configuration + "/*.Tests.dll");
});

Task("Package")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() => 
{
  var nuGetPackSettings   = new NuGetPackSettings
  {
    Version                 = "3.0.0",
    ProjectUrl              = new Uri("https://github.com/shiftkey/Reactive.EventAggregator/"),
    // TODO: an icon?
    //IconUrl                 = new Uri("http://cdn.rawgit.com/SomeUser/TestNuget/master/icons/testnuget.png"),
    LicenseUrl              = new Uri("https://github.com/shiftkey/Reactive.EventAggregator/blob/master/LICENSE.md"),
    Symbols                 = false,
    NoPackageAnalysis       = true,
    Files                   = new [] 
    {
      new NuSpecContent { Source = "Reactive.EventAggregator.dll", Target = "lib/net45" },
      new NuSpecContent { Source = "Reactive.EventAggregator.dll", Target = "lib/portable-Net45+winrt45+wp8+wpa81" },
      new NuSpecContent { Source = "Reactive.EventAggregator.dll", Target = "lib/portable-win81+wpa81" },
      new NuSpecContent { Source = "Reactive.EventAggregator.dll", Target = "lib/portable-windows8+net45+wp8" },
      new NuSpecContent { Source = "Reactive.EventAggregator.dll", Target = "lib/windows8" },
      new NuSpecContent { Source = "Reactive.EventAggregator.dll", Target = "lib/windowsphone8" },
    },
    BasePath                = "./src/Reactive.EventAggregator/bin/Portable/" + configuration + "/",
    OutputDirectory         = "./"
  };

NuGetPack("./src/Reactive.EventAggregator/Reactive.EventAggregator.nuspec", nuGetPackSettings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);