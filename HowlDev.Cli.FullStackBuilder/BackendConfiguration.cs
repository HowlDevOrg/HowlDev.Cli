namespace HowlDev.Cli.FullStackBuilder;

public enum BackendAPIType {
    CoreEmpty,
    WebApi,
    ControllerApi
}

public static class BackendOptions {
    public static PackageDefinition[] Packages {
        get => [
            new PackageDefinition("HowlDev Authentication", "add package HowlDev.Web.Authentication.AccountAuth"),
            new PackageDefinition("HowlDev DbConnector", "add package HowlDev.Web.Helpers.DbConnector"),
            new PackageDefinition("HowlDev WebSockets", "add package HowlDev.Web.Helpers.WebSockets"),
            new PackageDefinition("HowlDev CLI - TextDTO", "tool install --local HowlDev.Cli.TextDTO"),
            new PackageDefinition("Dapper", "add package Dapper"),
            new PackageDefinition("NpgSql", "add package NpgSql"),
            new PackageDefinition("EFCore", "add package Microsoft.EntityFrameworkCore"),
        ];
    }
}
