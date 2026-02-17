namespace HowlDev.Cli.TextDTO; 

/// <summary>
/// Holds a dictionary to locate references to other keys in other files.
/// </summary>
public class CrossFileReference : ICrossFileReference {
    private Dictionary<string, (string, string)> localDict = [];

    /// <summary/>
    public void AddKey(string key, string filename, string csharpNamespace) {
        localDict.Add(key, (filename, csharpNamespace));
    }

    /// <inheritdoc/>
    public bool ContainsKey(string key) {
        return localDict.ContainsKey(key);
    }

    /// <inheritdoc/>
    public (string file, string csharpNamespace) GetReference(string key) {
        return localDict[key];
    }
}