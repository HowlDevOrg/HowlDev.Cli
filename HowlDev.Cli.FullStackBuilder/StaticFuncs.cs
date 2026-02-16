namespace HowlDev.Cli.FullStackBuilder;

public static class StaticFuncs {
    public static void Run(string file, string args, string? cwd = null) {
        var psi = new ProcessStartInfo
        {
            FileName = file,
            Arguments = args,
            WorkingDirectory = cwd ?? Environment.CurrentDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var p = Process.Start(psi)!;
        p.WaitForExit();

        if (p.ExitCode != 0)
            throw new Exception($"{file} failed");
    }
}