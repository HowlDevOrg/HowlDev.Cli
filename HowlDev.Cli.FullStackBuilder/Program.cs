using HowlDev.Cli.FullStackBuilder;
using Spectre.Console;

(bool flowControl, ProjectConfiguration config) = StaticFuncs.InitializeFolderNames();

if (!flowControl) {
    return;
}

StaticFuncs.MakeDirectories(config);

Console.WriteLine();

StaticFuncs.InitializeVite(config);
StaticFuncs.InitializeCsharp(config);

Console.Clear();
flowControl = AnsiConsole.Confirm("Projects have been initialized. Would you like to further configure your stack?", false);

if (!flowControl) {
    // Install packages before letting the user go
    StaticFuncs.InstallFrontendPackages(config);
    return;
}

StaticFuncs.ConfigureFrontend(config);
