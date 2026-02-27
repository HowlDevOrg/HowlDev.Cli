namespace HowlDev.Cli.TextDTO;

/// <summary>
/// Holds a dictionary to locate references to other keys in other files.
/// </summary>
public interface ICrossFileReference {
    /// <summary>
    /// Returns True if the internal dictionary contains a given key.
    /// </summary>
    bool ContainsKey(string key);

    /// <summary>
    /// Returns the value of the key. Does no error checking. 
    /// </summary>
    (string file, string csharpNamespace) GetReference(string key);
}
