using Spectre.Console;

namespace HowlDev.Cli.FullStackBuilder; 
#pragma warning disable CS1591

public static class ViteCSharpFuncs {
    public const string ViteColor = "blue";
    public const string CSharpColor = "green";

    #region Initialization
    public static (bool flowControl, ViteFrontendProjectConfig value) InitializeFolderNames() {
        Console.WriteLine("This tool will help scaffold a Vite frontend and a C# backend.");

        string csharp = AnsiConsole.Ask<string>($"What's the [{CSharpColor}]C#[/] project name?");
        string vite = AnsiConsole.Ask<string>($"What's the [{ViteColor}]Vite[/] project name?");
        Console.WriteLine();

        AnsiConsole.MarkupLine($"Scaffolding projects in [{CSharpColor}]/{csharp}[/] and [{ViteColor}]/{vite}[/]. Is this okay?");
        AnsiConsole.MarkupLine($"Press [{StaticFuncs.RedColor}]Esc[/] to stop, [{StaticFuncs.HighlightColor}]any other key[/] to continue.");

        ConsoleKeyInfo key = Console.ReadKey();
        if (key.Key == ConsoleKey.Escape) {
            return (false, null!);
        }

        ViteFrontendProjectConfig config = new() {
            CSharpFolder = csharp,
            ViteFolder = vite
        };

        return (true, config);
    }

    public static void MakeDirectories(ViteFrontendProjectConfig config) {
        StaticFuncs.Run("mkdir", config.CSharpFolder);
        StaticFuncs.Run("mkdir", config.ViteFolder);
    }

    public static void InitializeVite(ViteFrontendProjectConfig config) {
        StaticFuncs.RefreshScreen("Initialize", StaticFuncs.HighlightColor);

        AnsiConsole.MarkupLine($"Initializing [{ViteColor}]Vite[/] project.");
        SelectionPrompt<FrontendPackageManager> p = new();
        p.Title($"Select your [{StaticFuncs.HighlightColor}]package manager[/].");
        p.AddChoices(FrontendPackageManager.Npm, FrontendPackageManager.Pnpm);
        FrontendPackageManager manager = AnsiConsole.Prompt(p);
        config.Manager = manager;
        AnsiConsole.MarkupLine($"[{StaticFuncs.RedColor}]Don't install and run now[/]. This will be done later in the process.");

        switch (manager) {
            case FrontendPackageManager.Npm:
                StaticFuncs.Run("npm", "create vite@latest ./" + config.ViteFolder, redirectOutput: false);
                break;
            case FrontendPackageManager.Pnpm:
                StaticFuncs.Run("pnpm", "create vite ./" + config.ViteFolder, redirectOutput: false);
                break;
        }
    }

    public static void InitializeCsharp(ViteFrontendProjectConfig config) {
        StaticFuncs.RefreshScreen("Initialize", StaticFuncs.HighlightColor);

        AnsiConsole.MarkupLine($"Initializing [{CSharpColor}]CSharp[/] project.");
        AnsiConsole.MarkupLine($"Select your [{StaticFuncs.HighlightColor}]API type[/].");
        SelectionPrompt<BackendAPIType> p = new();
        p.AddChoices(BackendAPIType.CoreEmpty, BackendAPIType.WebApi, BackendAPIType.ControllerApi);
        BackendAPIType manager = AnsiConsole.Prompt(p);

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .SpinnerStyle(Style.Parse(CSharpColor)).Start(
            "Building the C# project...",
            ctx => {
                switch (manager) {
                    case BackendAPIType.CoreEmpty:
                        StaticFuncs.Run("dotnet", "new web -o " + config.CSharpFolder);
                        break;
                    case BackendAPIType.WebApi:
                        StaticFuncs.Run("dotnet", "new webapi -o " + config.CSharpFolder);
                        break;
                    case BackendAPIType.ControllerApi:
                        StaticFuncs.Run("dotnet", "new webapi -controllers -o " + config.CSharpFolder);
                        break;
                }

                ctx.Status("Adding the C# solution...");
                StaticFuncs.Run("dotnet", "new sln", cwd: "./" + config.CSharpFolder);
                StaticFuncs.Run("dotnet", "sln add .", cwd: "./" + config.CSharpFolder);
            }
        );

        AnsiConsole.MarkupLine($"[{CSharpColor}]Done![/]");
    }

    public static void InstallFrontendPackages(ViteFrontendProjectConfig config) {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse(ViteColor)).Start(
            "Installing frontend packages...",
            ctx => {
                switch (config.Manager) {
                    case FrontendPackageManager.Npm:
                        StaticFuncs.Run("npm", "i", cwd: "./" + config.ViteFolder);
                        break;
                    case FrontendPackageManager.Pnpm:
                        StaticFuncs.Run("pnpm", "i", cwd: "./" + config.ViteFolder);
                        break;
                }
            }
        );
    }
    #endregion
    #region Configuration
    public static void ConfigureFrontend(ViteFrontendProjectConfig config) {
        StaticFuncs.RefreshScreen("Configuring Frontend", ViteColor);

        List<PackageDefinition> result = AnsiConsole.Prompt(new MultiSelectionPrompt<PackageDefinition>()
            .Title("Select which packages you'd like to install in the frontend: ")
            .NotRequired()
            .AddChoices(FrontendOptions.Packages));

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .SpinnerStyle(Style.Parse(ViteColor)).Start(
            "Installing frontend packages...",
            ctx => {
                switch (config.Manager) {
                    case FrontendPackageManager.Npm:
                        StaticFuncs.Run("npm", "i " + string.Join(' ', result.Select(a => a.InstallString)), cwd: "./" + config.ViteFolder);
                        break;
                    case FrontendPackageManager.Pnpm:
                        StaticFuncs.Run("pnpm", "i " + string.Join(' ', result.Select(a => a.InstallString)), cwd: "./" + config.ViteFolder);
                        break;
                }
            }
        );
    }

    public static void ConfigureFrontendFiles(ViteFrontendProjectConfig config) {
        StaticFuncs.RefreshScreen("Configuring Frontend", ViteColor);

        bool flowControl = AnsiConsole.Confirm($"Would you like a function that helps make [{StaticFuncs.HighlightColor}]HTTP calls[/]?");
        bool useTypeScript = DetectTypeScript(config.ViteFolder);
        if (flowControl) {
            string extension = useTypeScript ? "ts" : "js";
            string templateName = $"fetchHelpers.{extension}";
            string resourceName = $"HowlDev.Cli.FullStackBuilder.TemplateFiles.frontendAPI.{templateName}";

            AnsiConsole.MarkupLine($"Detected {(useTypeScript ? "[blue]TypeScript[/]" : "[yellow]JavaScript[/]")} project.");

            StaticFuncs.CopyEmbeddedResourceToFile(resourceName, Path.Combine(config.ViteFolder, "src", "api", templateName));
        }

        flowControl = AnsiConsole.Confirm($"Would you like to build the output directly into the API?", false);
        if (flowControl) {
            string viteFile = Path.Combine(config.ViteFolder, $"vite.config.{(useTypeScript ? "ts" : "js")}");
            string[] file = File.ReadAllLines(viteFile);
            List<string> newFile = [];

            foreach (string line in file) {
                newFile.Add(line);
                if (line.TrimEnd().EndsWith(',')) {
                    newFile.AddRange(FrontendStrings.Build(config.CSharpFolder).Split('\n'));
                }
            }

            File.WriteAllLines(viteFile, newFile);
        }
    }

    public static void ConfigureBackend(ViteFrontendProjectConfig config) {
        Console.Clear();
        List<PackageDefinition> result = AnsiConsole.Prompt(new MultiSelectionPrompt<PackageDefinition>()
            .Title("Select which packages you'd like to install in the backend: ")
            .NotRequired()
            .AddChoices(BackendOptions.Packages));

        AnsiConsole.Progress()
            .Start(ctx => {
                var task = ctx.AddTask("Installing backend packages...", maxValue: result.Count);
                foreach (var item in result) {
                    StaticFuncs.Run("dotnet", item.InstallString, cwd: "./" + config.CSharpFolder);
                    task.Increment(1);
                }
            }
        );
    }

    public static void ConfigureBackendFiles(ViteFrontendProjectConfig config) {
        StaticFuncs.RefreshScreen("Configuring Backend", ViteColor);

        bool flowControl = AnsiConsole.Confirm($"Would you like to set up a [{StaticFuncs.HighlightColor}]Postgres[/] connection string?");
        if (flowControl) {
            config.DatabaseName = AnsiConsole.Ask<string>("What would you like to name your database?");
            string appSettings = Path.Combine(config.CSharpFolder, "appsettings.json");
            string[] file = File.ReadAllLines(appSettings);
            List<string> newFile = [];

            foreach (string line in file) {
                if (line.Contains("AllowedHosts")) {
                    newFile.Add(line + ",");
                    newFile.AddRange(BackendStrings.PGConnectionString(config.DatabaseName).Split('\n'));
                } else {
                    newFile.Add(line);
                }
            }
            File.WriteAllLines(appSettings, newFile);
        }
    }
    #endregion

    private static bool DetectTypeScript(string viteFolder) {
        string srcPath = Path.Combine(viteFolder, "src");

        if (!Directory.Exists(srcPath)) {
            return false; // Default to JavaScript if src folder doesn't exist yet
        }

        bool hasTypeScriptFiles = Directory.GetFiles(srcPath, "*.tsx", SearchOption.TopDirectoryOnly).Length > 0 ||
                                  Directory.GetFiles(srcPath, "*.ts", SearchOption.TopDirectoryOnly).Length > 0;

        return hasTypeScriptFiles;
    }
}