namespace HowlDev.Cli.FullStackBuilder; 

#pragma warning disable CS1591 
public static class BackendStrings {
    public static string PGConnectionString(string dbName) => $"""
        "ConnectionStrings": {"{"}
            "PostgresConnection": "Host=localhost;Port=5432;Database={dbName};Username=admin;Password=123456abc;"
        {"}"}
    """;
}