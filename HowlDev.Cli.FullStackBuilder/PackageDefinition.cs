namespace HowlDev.Cli.FullStackBuilder;

public class PackageDefinition(string displayName, string installString) {
    public string InstallString = installString;
    public override string ToString() {
        return displayName;
    }
}
