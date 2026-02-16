using Spectre.Console;
using System.Diagnostics;

namespace HowlDev.Cli.FullStackBuilder;

public static class StaticFuncs {
    #region Initialization
    public static (bool flowControl, ProjectConfiguration value) InitializeFolderNames() {
        Console.Clear();

        Console.WriteLine("This tool will help scaffold a Vite frontend and a C# backend.");

        string csharp = AnsiConsole.Ask<string>("What's the [green]C#[/] project name?");
        string vite = AnsiConsole.Ask<string>("What's the [blue]Vite[/] project name?");
        Console.WriteLine();

        AnsiConsole.MarkupLine($"Scaffolding projects in [green]/{csharp}[/] and [blue]/{vite}[/]. Is this okay?");
        AnsiConsole.MarkupLine("Press [red]Esc[/] to stop, [slateblue1]any other key[/] to continue.");

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

        AnsiConsole.MarkupLine("Initializing [blue]Vite[/] project.");
        AnsiConsole.MarkupLine("Select your [slateblue1]package manager[/].");
        SelectionPrompt<FrontendPackageManager> p = new();
        p.AddChoices(FrontendPackageManager.Npm, FrontendPackageManager.Pnpm);
        FrontendPackageManager manager = AnsiConsole.Prompt(p);
        config.Manager = manager;
        AnsiConsole.MarkupLine("[red]Don't install and run now[/]. This will be done later in the process.");

        Run("cd", "./" + config.ViteFolder);
        switch (manager) {
            case FrontendPackageManager.Npm:
                Run("npm", "create vite@latest ./" + config.ViteFolder, redirectOutput: false);
                break;
            case FrontendPackageManager.Pnpm:
                Run("pnpm", "create vite ./" + config.ViteFolder, redirectOutput: false);
                break;
        }
        Run("cd", "..");
    }

    public static void InitializeCsharp(ProjectConfiguration config) {
        Console.Clear();

        AnsiConsole.MarkupLine("Initializing [green]CSharp[/] project.");
        AnsiConsole.MarkupLine("Select your [slateblue1]API type[/].");
        SelectionPrompt<BackendAPIType> p = new();
        p.AddChoices(BackendAPIType.CoreEmpty, BackendAPIType.WebApi, BackendAPIType.ControllerApi);
        BackendAPIType manager = AnsiConsole.Prompt(p);

        AnsiConsole.Status().Start(
            "Building the C# project...", 
            a => {
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
                Run("dotnet", "new sln", cwd: "./" + config.CSharpFolder);
                Run("dotnet", "sln add .", cwd: "./" + config.CSharpFolder);
            }
        );

        AnsiConsole.MarkupLine("[green]Done![/]");
    }
    #endregion

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
}
