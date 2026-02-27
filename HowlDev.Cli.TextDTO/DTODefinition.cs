namespace HowlDev.Cli.TextDTO;
#pragma warning disable CS1591, CA1819

public class DTODefinition {
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public bool IgnoreWarnings { get; set; }
    public PropertyDefinition[] Properties { get; set; } = [];
    public string[] EnumValues { get; set; } = [];
}

public class PropertyDefinition {
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Default { get; set; }
    public bool Nullable { get; set; }
}
