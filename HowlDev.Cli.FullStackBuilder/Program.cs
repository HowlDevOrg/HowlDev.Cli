using HowlDev.Cli.FullStackBuilder;
using Spectre.Console;

StaticFuncs.RefreshScreen("Initialize", StaticFuncs.HighlightColor);

(bool flowControl, ProjectConfiguration config) = StaticFuncs.InitializeFolderNames();

if (!flowControl) {
    return;
}

StaticFuncs.MakeDirectories(config);

AnsiConsole.WriteLine();

StaticFuncs.InitializeVite(config);
StaticFuncs.InitializeCsharp(config);

StaticFuncs.RefreshScreen("Complete!", StaticFuncs.HighlightColor);
flowControl = AnsiConsole.Confirm("Projects have been initialized. Would you like to further configure your stack?", false);

if (!flowControl) {
    // Install packages before letting the user go
    StaticFuncs.InstallFrontendPackages(config);
    return;
}

StaticFuncs.RefreshScreen("Configuring Frontend", StaticFuncs.ViteColor);

flowControl = AnsiConsole.Confirm("Would you like to configure the frontend? \nYou can install packages and set up the Vite config file.");
if (flowControl) {
    StaticFuncs.ConfigureFrontend(config);
    StaticFuncs.ConfigureFrontendFiles(config);
}

StaticFuncs.RefreshScreen("Configuring Backend", StaticFuncs.CSharpColor);

flowControl = AnsiConsole.Confirm("Would you like to configure the backend? \nYou can install packages, set up environment variables, and initialize Program.cs.");
if (flowControl) {
    StaticFuncs.ConfigureBackend(config);
    StaticFuncs.ConfigureBackendFiles(config);
}
