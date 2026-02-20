namespace HowlDev.Cli.TextDTO;
#pragma warning disable CS1591 

public class DTODefinition {
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public bool IgnoreWarnings { get; set; } = false;
    public PropertyDefinition[] Properties { get; set; } = [];
    public string[] EnumValues { get; set; } = [];
}

public class PropertyDefinition {
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Default { get; set; } = null;
    public bool Nullable { get; set; } = false;
}