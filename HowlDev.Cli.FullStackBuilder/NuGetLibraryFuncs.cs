using Spectre.Console;

namespace HowlDev.Cli.FullStackBuilder;
#pragma warning disable CS1591 

public static class NuGetLibraryFuncs {
    public static void InitializeSolution(NugetProjectConfig config) {
        bool makeNewFolder = AnsiConsole.Confirm("Make a new folder and navigate to it?", false);
        if (makeNewFolder) {
            string newFolder = AnsiConsole.Ask<string>("What is the new folder name?");
            StaticFuncs.Run("mkdir", newFolder);
            StaticFuncs.Run("cd", "./" + newFolder);
        }
        config.SolutionName = AnsiConsole.Ask<string>("What is your solution name?");

        StaticFuncs.Run("dotnet", "new sln -n " + config.SolutionName);
    }

    public static void AddProjects(NugetProjectConfig config) {
        AnsiConsole.MarkupLine($"Write your list of projects. Include [{StaticFuncs.HighlightColor}]\".\"[/] at the start to append to the end of the Solution name.");
        AnsiConsole.MarkupLine($"Write a [{StaticFuncs.HighlightColor}]![/] to stop the loop.");
        List<string> tempList = [];
        while (true) {
            string userPrompt = AnsiConsole.Ask<string>("Type the next project name.").Trim();
            if (userPrompt == "!") {
                break;
            }
            if (userPrompt.StartsWith('.')) {
                tempList.Add(config.SolutionName + userPrompt);
            } else {
                tempList.Add(userPrompt);
            }
        }
        
        
    }
}