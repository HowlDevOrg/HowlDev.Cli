namespace HowlDev.Cli.FullStackBuilder;

#pragma warning disable CS1591 
public enum TestRunnerType {
    TUnit,
    XUnit,
    NUnit,
}

public static class NugetConfiguration {
    public static string Gitignore => """
    .vs/
    .vscode/
    bin/
    obj/
    node_modules/
    TestResults/
    .env
    """;
    
    public static string CsProj(string version) => $"""
    <Project Sdk="Microsoft.NET.Sdk">

        <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net{version}.0</TargetFramework>
            <Version>0.0.1</Version>
            <Authors>Sample Author</Authors>
            <Description>Sample Description</Description>
            <Copyright>Copyright (c) Organization Year</Copyright>
            <PackageTags></PackageTags>
            <PackageLicenseExpression>MIT</PackageLicenseExpression>
            <RepositoryUrl>https://github.com/Cody-Howell/HowlDev.Cli</RepositoryUrl>
            <PackageProjectUrl>https://wiki.codyhowell.dev/cli</PackageProjectUrl>
            <PackageReadmeFile>README.md</PackageReadmeFile>
            <PackageIcon>_HowlDevLogo.png</PackageIcon>
            <GenerateDocumentationFile>true</GenerateDocumentationFile>
            <ImplicitUsings>enable</ImplicitUsings>
            <Nullable>enable</Nullable>
            <IncludeSymbols>true</IncludeSymbols>
            <SymbolPackageFormat>snupkg</SymbolPackageFormat>
            <PackageOutputPath>./nupkg</PackageOutputPath>
        </PropertyGroup>

        <ItemGroup>
            <None Include="../README.md" Pack="true" PackagePath="README.md" />
            <None Include="../_HowlDevLogo.png" Pack="true" PackagePath="" />
        </ItemGroup>

    </Project>

    """;
}
