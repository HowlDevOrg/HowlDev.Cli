namespace HowlDev.Cli.FullStackBuilder;

#pragma warning disable CS1591 
public class PackageDefinition(string displayName, string installString) {
    public string InstallString = installString;
    public override string ToString() {
        return displayName;
    }
}
