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
    MSBuild("./src/Reactive.EventAggregator.sln", settings =>
	    settings.SetConfiguration(configuration));
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit2("./src/**/bin/" + configuration + "/*.Tests.dll");
});


Task("Pack")
    .IsDependentOn("Run-Unit-Tests")
	.Does(() =>
{
	// Use MSBuild
	MSBuild("./src/Reactive.EventAggregator/Reactive.EventAggregator.csproj", settings =>
        settings.SetConfiguration(configuration)
			.WithTarget("Pack"));
});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);