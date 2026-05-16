using Spectre.Console;

namespace HowlDev.Cli.FullStackBuilder;
#pragma warning disable CS1591 

public static class NuGetLibraryFuncs {
    public static void InitializeSolution(NugetProjectConfig config) {
        bool makeNewFolder = AnsiConsole.Confirm("Make a new folder and navigate to it?", false);
        if (makeNewFolder) {
            string newFolder = AnsiConsole.Ask<string>("What is the new folder name?");
            if (!Directory.Exists(newFolder)) {
                StaticFuncs.Run("mkdir", newFolder);
            }

            config.WorkingDir = newFolder + "/";
            config.TopLevel = newFolder + "/";
        }

        config.SolutionName = AnsiConsole.Ask<string>("What is your solution name?");

        StaticFuncs.Run("dotnet", "new sln -n " + config.SolutionName, config.WorkingDir);
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
        p.Title($"[{StaticFuncs.HighlightColor}]Select[/] any that were created [{StaticFuncs.RedColor}]in error[/]. These will be removed.");
        p.AddChoices(tempList);
        p.NotRequired();
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
        prompt.NotRequired();
        config.TestProjects = AnsiConsole.Prompt(prompt);
    }

    public static void CompleteSolution(NugetProjectConfig config) {
        string version = Environment.Version.Major.ToString();

        AnsiConsole.Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse(ViteCSharpFuncs.CSharpColor))
                    .Start("Building the C# projects...",
                    ctx => {
                        foreach (string project in config.Projects) {
                            StaticFuncs.Run("dotnet", $"new classlib -f net{version}.0 -o " + project, config.WorkingDir);
                            File.WriteAllText(Path.Combine(config.WorkingDir, project, project + ".csproj"), NugetConfiguration.CsProj(version));
                        }

                        if (config.TestProjects.Count > 0) {
                            ctx.Status("Adding test projects...");
                            StaticFuncs.Run("mkdir", "Tests", config.WorkingDir);
                            config.WorkingDir += "Tests";

                            foreach (string project in config.TestProjects) {
                                string testProjectName = project + ".Tests";
                                switch (config.TestRunner) {
                                    case TestRunnerType.TUnit:
                                        StaticFuncs.Run("dotnet", $"new console -f net{version}.0 -n " + testProjectName, config.WorkingDir);
                                        StaticFuncs.Run("dotnet", "dotnet add package TUnit --prerelease", Path.Combine(config.WorkingDir, testProjectName));
                                        break;
                                    case TestRunnerType.NUnit:
                                        StaticFuncs.Run("dotnet", $"new nunit -f net{version}.0 -o " + testProjectName, config.WorkingDir);
                                        break;
                                    case TestRunnerType.XUnit:
                                        StaticFuncs.Run("dotnet", $"new xunit -f net{version}.0 -o " + testProjectName, config.WorkingDir);
                                        break;
                                }
                            }

                            ctx.Status("Linking projects...");
                            foreach (string project in config.TestProjects) { // this is intended to be TestProjects
                                string testProjectName = project + ".Tests";
                                StaticFuncs.Run("dotnet", "sln add Tests/" + testProjectName, config.TopLevel);
                                StaticFuncs.Run("dotnet", $"add reference ../../{project}", Path.Combine(config.WorkingDir, testProjectName));
                                // the above is intended to be project
                            }
                        }

                        ctx.Status("Loading into solution...");
                        foreach (string project in config.Projects) {
                            StaticFuncs.Run("dotnet", "sln add " + project, config.TopLevel);
                        }
                    }
                );
    }

    public static void FinalizeFiles(NugetProjectConfig config) {
        AnsiConsole.MarkupLine($"Select any files you want to generate.");

        MultiSelectionPrompt<string> prompt = new();
        prompt.AddChoices("README.md", "CHANGELOG.md", "KNOWN_BUGS.md", ".gitignore");
        prompt.NotRequired();
        List<string> newFiles = AnsiConsole.Prompt(prompt);
        foreach (string file in newFiles) {
            string content = file == ".gitignore" ? NugetConfiguration.Gitignore : "";
            File.WriteAllText(Path.Combine(config.TopLevel ?? "", file), content);
        }
    }
}
