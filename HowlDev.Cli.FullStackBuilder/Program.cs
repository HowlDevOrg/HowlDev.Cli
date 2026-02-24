using HowlDev.Cli.FullStackBuilder;
using Spectre.Console;

StaticFuncs.RefreshScreen("Select", StaticFuncs.HighlightColor);

string result = StaticFuncs.MakeSelection();

if (result == StaticFuncs.ViteApp) {
    StaticFuncs.RefreshScreen("Initialize", StaticFuncs.HighlightColor);

    (bool flowControl, ViteFrontendProjectConfig config) = ViteCSharpFuncs.InitializeFolderNames();

    if (!flowControl) {
        return;
    }

    ViteCSharpFuncs.MakeDirectories(config);

    AnsiConsole.WriteLine();

    ViteCSharpFuncs.InitializeVite(config);
    ViteCSharpFuncs.InitializeCsharp(config);

    StaticFuncs.RefreshScreen("Complete!", StaticFuncs.HighlightColor);
    flowControl = AnsiConsole.Confirm("Projects have been initialized. Would you like to further configure your stack?", false);

    if (!flowControl) {
        // Install packages before letting the user go
        ViteCSharpFuncs.InstallFrontendPackages(config);
        return;
    }

    StaticFuncs.RefreshScreen("Configuring Frontend", ViteCSharpFuncs.ViteColor);

    flowControl = AnsiConsole.Confirm("Would you like to configure the frontend? \nYou can install packages and set up the Vite config file.");
    if (flowControl) {
        ViteCSharpFuncs.ConfigureFrontend(config);
        ViteCSharpFuncs.ConfigureFrontendFiles(config);
    }

    StaticFuncs.RefreshScreen("Configuring Backend", ViteCSharpFuncs.CSharpColor);

    flowControl = AnsiConsole.Confirm("Would you like to configure the backend? \nYou can install packages, set up environment variables, and initialize Program.cs.");
    if (flowControl) {
        ViteCSharpFuncs.ConfigureBackend(config);
        ViteCSharpFuncs.ConfigureBackendFiles(config);
    }
} else if (result == StaticFuncs.NugetLib) {
    StaticFuncs.RefreshScreen("Solution", StaticFuncs.HighlightColor);

    NugetProjectConfig config = new();

    NuGetLibraryFuncs.InitializeSolution(config);

    StaticFuncs.RefreshScreen("Projects", ViteCSharpFuncs.CSharpColor);
    NuGetLibraryFuncs.AddProjects(config);
}