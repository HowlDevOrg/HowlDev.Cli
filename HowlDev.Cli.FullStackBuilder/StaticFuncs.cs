using Spectre.Console;
using System.Diagnostics;

namespace HowlDev.Cli.FullStackBuilder;

public static class StaticFuncs {
    // Color constants
    public const string ViteColor = "blue";
    public const string CSharpColor = "green";
    public const string RedColor = "red";
    public const string HighlightColor = "slateblue1";

    #region Initialization
    public static (bool flowControl, ProjectConfiguration value) InitializeFolderNames() {
        Console.Clear();

        Console.WriteLine("This tool will help scaffold a Vite frontend and a C# backend.");

        string csharp = AnsiConsole.Ask<string>($"What's the [{CSharpColor}]C#[/] project name?");
        string vite = AnsiConsole.Ask<string>($"What's the [{ViteColor}]Vite[/] project name?");
        Console.WriteLine();

        AnsiConsole.MarkupLine($"Scaffolding projects in [{CSharpColor}]/{csharp}[/] and [{ViteColor}]/{vite}[/]. Is this okay?");
        AnsiConsole.MarkupLine($"Press [{RedColor}]Esc[/] to stop, [{HighlightColor}]any other key[/] to continue.");

        ConsoleKeyInfo key = Console.ReadKey();
        if (key.Key == ConsoleKey.Escape) {
            return (false, null!);
        }

        ProjectConfiguration config = new() {
            CSharpFolder = csharp,
            ViteFolder = vite
        };

        return (true, config);
    }

    public static void MakeDirectories(ProjectConfiguration config) {
        Run("mkdir", config.CSharpFolder);
        Run("mkdir", config.ViteFolder);
    }

    public static void InitializeVite(ProjectConfiguration config) {
        Console.Clear();

        AnsiConsole.MarkupLine($"Initializing [{ViteColor}]Vite[/] project.");
        SelectionPrompt<FrontendPackageManager> p = new();
        p.Title($"Select your [{HighlightColor}]package manager[/].");
        p.AddChoices(FrontendPackageManager.Npm, FrontendPackageManager.Pnpm);
        FrontendPackageManager manager = AnsiConsole.Prompt(p);
        config.Manager = manager;
        AnsiConsole.MarkupLine($"[{RedColor}]Don't install and run now[/]. This will be done later in the process.");

        switch (manager) {
            case FrontendPackageManager.Npm:
                Run("npm", "create vite@latest ./" + config.ViteFolder, redirectOutput: false);
                break;
            case FrontendPackageManager.Pnpm:
                Run("pnpm", "create vite ./" + config.ViteFolder, redirectOutput: false);
                break;
        }
    }

    public static void InitializeCsharp(ProjectConfiguration config) {
        Console.Clear();

        AnsiConsole.MarkupLine($"Initializing [{CSharpColor}]CSharp[/] project.");
        AnsiConsole.MarkupLine($"Select your [{HighlightColor}]API type[/].");
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
                        Run("dotnet", "new web -o " + config.CSharpFolder);
                        break;
                    case BackendAPIType.WebApi:
                        Run("dotnet", "new webapi -o " + config.CSharpFolder);
                        break;
                    case BackendAPIType.ControllerApi:
                        Run("dotnet", "new webapi -controllers -o " + config.CSharpFolder);
                        break;
                }

                ctx.Status("Adding the C# solution...");
                Run("dotnet", "new sln", cwd: "./" + config.CSharpFolder);
                Run("dotnet", "sln add .", cwd: "./" + config.CSharpFolder);
            }
        );

        AnsiConsole.MarkupLine($"[{CSharpColor}]Done![/]");
    }

    public static void InstallFrontendPackages(ProjectConfiguration config) {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star2)
            .SpinnerStyle(Style.Parse(ViteColor)).Start(
            "Installing frontend packages...",
            ctx => {
                switch (config.Manager) {
                    case FrontendPackageManager.Npm:
                        Run("npm", "i", cwd: "./" + config.ViteFolder);
                        break;
                    case FrontendPackageManager.Pnpm:
                        Run("pnpm", "i", cwd: "./" + config.ViteFolder);
                        break;
                }
            }
        );
    }
    #endregion
    #region Configuration
    public static void ConfigureFrontend(ProjectConfiguration config) {
        Console.Clear();
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
                        Run("npm", "i " + string.Join(' ', result.Select(a => a.InstallString)), cwd: "./" + config.ViteFolder);
                        break;
                    case FrontendPackageManager.Pnpm:
                        Run("pnpm", "i " + string.Join(' ', result.Select(a => a.InstallString)), cwd: "./" + config.ViteFolder);
                        break;
                }
            }
        );
    }

    public static void ConfigureFrontendFiles(ProjectConfiguration config) {
        Console.Clear();
        bool flowControl = AnsiConsole.Confirm($"Would you like a function that helps make [{HighlightColor}]HTTP calls[/]?");
        if (flowControl) {
            bool useTypeScript = DetectTypeScript(config.ViteFolder);
            string extension = useTypeScript ? "ts" : "js";
            string templateName = $"fetchHelpers.{extension}";
            string resourceName = $"HowlDev.Cli.FullStackBuilder.TemplateFiles.frontendAPI.{templateName}";
            
            AnsiConsole.MarkupLine($"Detected {(useTypeScript ? "[blue]TypeScript[/]" : "[yellow]JavaScript[/]")} project");
            
            CopyEmbeddedResourceToFile(resourceName, Path.Combine(config.ViteFolder, "src", "api", templateName));
        }

        flowControl = AnsiConsole.Confirm($"Would you like ");
    }

    public static void ConfigureBackend(ProjectConfiguration config) {
        Console.Clear();
        List<PackageDefinition> result = AnsiConsole.Prompt(new MultiSelectionPrompt<PackageDefinition>()
            .Title("Select which packages you'd like to install in the backend: ")
            .NotRequired()
            .AddChoices(BackendOptions.Packages));

        AnsiConsole.Progress()
            .Start(ctx => {
                var task = ctx.AddTask("Installing backend packages...", maxValue: result.Count);
                foreach (var item in result) {
                    Run("dotnet", item.InstallString, cwd: "./" + config.CSharpFolder);
                    task.Increment(1);
                }
            }
        );
    }
    #endregion


    // Entirely by AI from here below
    private static void Run(string file, string args, string? cwd = null, bool redirectOutput = true) {
        var psi = new ProcessStartInfo {
            FileName = file,
            Arguments = args,
            WorkingDirectory = cwd ?? Environment.CurrentDirectory,
            RedirectStandardOutput = redirectOutput,
            RedirectStandardError = true
        };

        using var p = Process.Start(psi)!;
        p.WaitForExit();

        if (p.ExitCode != 0)
            throw new Exception($"{file} failed");
    }

    private static void CopyEmbeddedResourceToFile(string resourceName, string destinationPath) {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null) {
            throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
        }

        string? destinationDir = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(destinationDir) && !Directory.Exists(destinationDir)) {
            Directory.CreateDirectory(destinationDir);
        }

        using var fileStream = File.Create(destinationPath);
        stream.CopyTo(fileStream);
        
        AnsiConsole.MarkupLine($"Copied template to [{HighlightColor}]{destinationPath}[/]");
    }

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
