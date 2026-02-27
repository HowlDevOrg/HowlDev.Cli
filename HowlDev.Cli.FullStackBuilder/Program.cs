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

    StaticFuncs.RefreshScreen("Complete!", StaticFuncs.HighlightColor);
} else if (result == StaticFuncs.NugetLib) {
    StaticFuncs.RefreshScreen("Solution", StaticFuncs.HighlightColor);

    NugetProjectConfig config = new();

    NuGetLibraryFuncs.InitializeSolution(config);

    StaticFuncs.RefreshScreen("Projects", ViteCSharpFuncs.CSharpColor);
    NuGetLibraryFuncs.AddProjects(config);

    StaticFuncs.RefreshScreen("Testing", ViteCSharpFuncs.CSharpColor);
    bool flowControl = AnsiConsole.Confirm("Would you like to configure tests? You can create pre-hooked up projects with your preferred suite.");

    if (flowControl) {
        StaticFuncs.RefreshScreen("Testing", ViteCSharpFuncs.CSharpColor);
        NuGetLibraryFuncs.CreateTests(config);
    }

    StaticFuncs.RefreshScreen("Create Solution", ViteCSharpFuncs.CSharpColor);
    NuGetLibraryFuncs.CompleteSolution(config);

    StaticFuncs.RefreshScreen("Nuget", ViteCSharpFuncs.CSharpColor);
    flowControl = AnsiConsole.Confirm("Include a NuGet workflow to automatically update versions when you push to a .csproj file?", false);

    if (flowControl) {
        StaticFuncs.RefreshScreen("Nuget", ViteCSharpFuncs.CSharpColor);
        StaticFuncs.CopyEmbeddedResourceToFile("HowlDev.Cli.FullStackBuilder.TemplateFiles.nuget.workflow.yaml", ".github/workflows");
        AnsiConsole.MarkupLine($"Set up [{StaticFuncs.HighlightColor}]trusted publishers[/] in NuGet and your username as a repository secret.");
        AnsiConsole.MarkupLine("This will need to be done for each repository. The secret should be named \"NUGET_USER\"");
        AnsiConsole.Confirm("Press anything to continue.");
    }

    StaticFuncs.RefreshScreen("Complete!", StaticFuncs.HighlightColor);
}
