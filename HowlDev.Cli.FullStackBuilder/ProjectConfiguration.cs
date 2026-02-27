namespace HowlDev.Cli.FullStackBuilder;

#pragma warning disable CS1591 
public class ViteFrontendProjectConfig {
    public string CSharpFolder { get; set; } = "";
    public string ViteFolder { get; set; } = "";
    public string DatabaseName { get; set; } = "";
    public FrontendPackageManager Manager { get; set; }
}
public class NugetProjectConfig {
    public string SolutionName { get; set; } = "";
    public List<string> Projects { get; set; } = [];
    public List<string> TestProjects { get; set; } = [];
    public TestRunnerType TestRunner { get; set; } = TestRunnerType.NUnit;
}
