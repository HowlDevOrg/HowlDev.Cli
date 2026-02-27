using Spectre.Console;

namespace HowlDev.Cli.FullStackBuilder;
#pragma warning disable CS1591 

public static class NuGetLibraryFuncs {
    public static void InitializeSolution(NugetProjectConfig config) {
        bool makeNewFolder = AnsiConsole.Confirm("Make a new folder and navigate to it?", false);
        if (makeNewFolder) {
            string newFolder = AnsiConsole.Ask<string>("What is the new folder name?");
            StaticFuncs.Run("mkdir", newFolder);
            StaticFuncs.Run("cd", newFolder);
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

        MultiSelectionPrompt<string> p = new();
        p.Title($"Select any that were created in [{StaticFuncs.RedColor}]error[/] These will be removed.");
        p.AddChoices(tempList);
        List<string> remove = AnsiConsole.Prompt(p);
        foreach (string item in remove) {
            tempList.Remove(item);
        }

        config.Projects = tempList;
    }

    public static void CreateTests(NugetProjectConfig config) {
        AnsiConsole.MarkupLine($"What is your preferred [{ViteCSharpFuncs.CSharpColor}]test suite[/]?");

        SelectionPrompt<TestRunnerType> p = new();
        p.AddChoices(TestRunnerType.TUnit, TestRunnerType.NUnit, TestRunnerType.XUnit);
        config.TestRunner = AnsiConsole.Prompt(p);

        AnsiConsole.MarkupLine($"Select the projects you'd like to make a tied test suite to.");
        MultiSelectionPrompt<string> prompt = new();
        prompt.AddChoices(config.Projects);
        config.TestProjects = AnsiConsole.Prompt(prompt);
    }

    public static void CompleteSolution(NugetProjectConfig config) {
        string version = Environment.Version.Major.ToString();

        AnsiConsole.Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse(ViteCSharpFuncs.CSharpColor)).Start(
                    "Building the C# projects...",
                    ctx => {
                        foreach (string project in config.Projects) {
                            StaticFuncs.Run("dotnet", $"new classlib -f net{version}.0 -o " + project);
                            File.WriteAllText(Path.Combine(project, project + ".csproj"), NugetConfiguration.CsProj);
                        }

                        if (config.TestProjects.Count > 0) {
                            ctx.Status("Adding test projects...");
                            StaticFuncs.Run("mkdir", "Tests");
                            StaticFuncs.Run("cd", "Tests");

                            foreach (string project in config.TestProjects) {
                                string testProjectName = project + ".Tests";
                                switch (config.TestRunner) {
                                    case TestRunnerType.TUnit:
                                        StaticFuncs.Run("dotnet", $"new console -f net{version}.0 -n " + testProjectName);
                                        StaticFuncs.Run("cd", testProjectName);
                                        StaticFuncs.Run("dotnet", "dotnet add package TUnit --prerelease");
                                        StaticFuncs.Run("cd", "..");
                                        break;
                                    case TestRunnerType.NUnit:
                                        StaticFuncs.Run("dotnet", $"new nunit -f net{version}.0 -o " + testProjectName);
                                        break;
                                    case TestRunnerType.XUnit:
                                        StaticFuncs.Run("dotnet", $"new xunit -f net{version}.0 -o " + testProjectName);
                                        break;
                                }
                            }

                            ctx.Status("Linking projects...");
                            foreach (string project in config.TestProjects) {
                                string testProjectName = project + ".Tests";
                                StaticFuncs.Run("dotnet", "sln add Tests/" + testProjectName);
                                StaticFuncs.Run("dotnet", $"add reference Tests/{testProjectName} {project}");
                            }
                        }

                        ctx.Status("Loading into solution...");
                        foreach (string project in config.Projects) {
                            StaticFuncs.Run("dotnet", "sln add " + project);
                        }
                    }
                );
    }

}
