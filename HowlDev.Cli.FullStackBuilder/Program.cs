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

Console.Clear();
AnsiConsole.Write(new Rule($"[{StaticFuncs.ViteColor}]Configuring Frontend[/]"));

flowControl = AnsiConsole.Confirm("Would you like to configure the frontend? \nYou can install packages and set up the Vite config file.");
if (flowControl) {
    StaticFuncs.ConfigureFrontend(config);
    StaticFuncs.ConfigureFrontendFiles(config);
    Console.WriteLine();
}

flowControl = AnsiConsole.Confirm("Would you like to configure the backend? \nYou can install packages, set up environment variables, and initialize Program.cs.");
if (flowControl) {
    StaticFuncs.ConfigureBackend(config);
}
