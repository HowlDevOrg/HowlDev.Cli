using HowlDev.Cli.FullStackBuilder;

(bool flowControl, ProjectConfiguration config) = StaticFuncs.InitializeFolderNames();

if (!flowControl) {
    return;
}

StaticFuncs.MakeDirectories(config);

Console.WriteLine();

// StaticFuncs.InitializeVite(config);
StaticFuncs.InitializeCsharp(config);
