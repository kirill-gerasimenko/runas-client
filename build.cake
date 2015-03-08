///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target          = Argument<string>("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutions				= GetFiles("./**/*.sln");
var solutionDirs			= solutions.Select(solution => solution.GetDirectory());
var buildDir				= "./build";
var coreSolutionBuildDir	= "./src/RunAsClient.Core/bin/" + configuration;
var cliSolutionBuildDir		= "./src/RunAsClient/bin/" + configuration;
var coreNuspecPath			= "./nuspec/RunAsClient.Core.nuspec";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    // Clean solution directories.
    foreach(var solutionDir in solutionDirs)
    {
        Information("Cleaning {0}", solutionDir);
        CleanDirectories(solutionDir + "/**/bin/" + configuration);
        CleanDirectories(solutionDir + "/**/obj/" + configuration);
    }
});

Task("Restore")
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}", solution);
        NuGetRestore(solution, new NuGetRestoreSettings 
		{
			PackagesDirectory = "./packages"
		});
    }
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    // Build all solutions.
    foreach(var solution in solutions)
    {
        Information("Building {0}", solution);
        MSBuild(solution, settings => 
            settings.SetPlatformTarget(PlatformTarget.MSIL)
                .WithProperty("TreatWarningsAsErrors","true")
                .WithProperty("Platform","Any CPU")
                .WithTarget("Build")
                .SetConfiguration(configuration));
    }

	if (DirectoryExists(buildDir))
		CleanDirectory(buildDir);
	else
		CreateDirectory(buildDir);

	CopyFileToDirectory(coreSolutionBuildDir + "/RunAsClient.Core.dll",
						buildDir);
	CopyFileToDirectory(cliSolutionBuildDir + "/RunAsClient.exe",
						buildDir);
});

Task("Create-Core-NuGet-Package")
	.IsDependentOn("Build")
	.Does(() => 
{
	NuGetPack(coreNuspecPath, new NuGetPackSettings 
	{
		Version = "0.0.2",
		BasePath = buildDir,
		OutputDirectory = buildDir
	});
});

Task("Default")
    .IsDependentOn("Create-Core-NuGet-Package");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
