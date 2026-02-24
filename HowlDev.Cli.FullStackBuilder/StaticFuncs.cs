using Spectre.Console;
using System.Diagnostics;

namespace HowlDev.Cli.FullStackBuilder;

#pragma warning disable CS1591
public static class StaticFuncs {
    // Color constants
    public const string RedColor = "red";
    public const string HighlightColor = "slateblue1";
    public const string ViteApp = "Vite + CSharp App";
    public const string NugetLib = "NuGet Library";

    public static void RefreshScreen(string title, string color) {
        Console.Clear();
        AnsiConsole.Write(
            new Panel(Align.Center(new Markup($"[{color}]{title}[/]")))
                .Expand()
                .Border(BoxBorder.Double)
        );
    }

    public static string MakeSelection() {
        AnsiConsole.MarkupLine("Choose which type of project you'd like to set up.");
        SelectionPrompt<string> p = new();
        p.Title($"Select the [{HighlightColor}]project[/] to build.");
        p.AddChoices(ViteApp, NugetLib);
        return AnsiConsole.Prompt(p);
    }

    internal static void Run(string file, string args, string? cwd = null, bool redirectOutput = true) {
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

    internal static void CopyEmbeddedResourceToFile(string resourceName, string destinationPath) {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
        string? destinationDir = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(destinationDir) && !Directory.Exists(destinationDir)) {
            Directory.CreateDirectory(destinationDir);
        }

        using var fileStream = File.Create(destinationPath);
        stream.CopyTo(fileStream);

        AnsiConsole.MarkupLine($"Copied template to [{HighlightColor}]{destinationPath}[/]");
    }
}
