namespace HowlDev.Cli.TextDTO;

public class DTODefinition {
    public string Name { get; set; }
    public string Type { get; set; }
    public string Namespace { get; set; }
    public bool IgnoreWarnings { get; set; } = false;
    public PropertyDefinition[] Properties { get; set; } = [];
    public string[] EnumValues { get; set; } = [];
}

public class PropertyDefinition {
    public string Name { get; set; }
    public string Type { get; set; }
    public string? Default { get; set; } = null;
    public bool Nullable { get; set; } = false;
}