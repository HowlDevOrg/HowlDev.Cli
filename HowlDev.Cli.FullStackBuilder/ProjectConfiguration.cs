namespace HowlDev.Cli.FullStackBuilder;

#pragma warning disable CS1591 
public class ProjectConfiguration {
    public string CSharpFolder { get; set; } = "";
    public string ViteFolder { get; set; } = "";
    public string DatabaseName { get; set; } = "";
    public FrontendPackageManager Manager { get; set; }
}
