namespace HowlDev.Cli.FullStackBuilder;

#pragma warning disable CS1591 
public static class FrontendStrings {
    public static string Build(string apiName) => $"""
            build: {"{"}
                outDir: "../{apiName}/wwwroot",
                emptyOutDir: true,
            {"}"},
        """;

    // public static string Proxy(int port) => $"""
    //     server: {"{"}
    //         proxy: {"{"}
    //             "/api": "https://localhost:{port}",
    //         {"}"},
    //     {"}"},
    //     """;
}
