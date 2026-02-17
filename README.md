# HowlDev.Cli

## HowlDev.Cli.TextDTO

This is a command-line NuGet package that you can install with:

```bash
dotnet tool install --global HowlDev.Cli.TextDTO

# Or inside of your project
dotnet tool install HowlDev.Cli.TextDTO
```

Format (the ... is a repeat of tag and location): 

```bash
textdto <dtopath> <tag> <exportLocation> ...
```

An example of usage: 

```bash
textdto ./dtos cs ./api/dtos ts ./client/src/types ts-z ./client/src/zod
```

You can export a given file as a set of DTO's with any of the 3 following tags:

- cs
  - Exports a C# file with an optional namespace in the format of a class with public get and set properties
- ts
  - Creates a Type that has the provided properties
    - Ignores the Default property (impossible with type definitions)
    - Nullable means undefined, the default null state for things in JavaScript
- ts-z
  - Creates a Zod object with the provided properties (you must install Zod yourself)
  - Names are appended with Schema for Zod parsing and Type for the type inference. Both are exported.
  - Nullable uses the Zod type `.nullable()` since `.optional()` does not do anything in the presence of `.default()` 
  - Exports types using `z.infer<>`.

## File structure

The files you pass in from the schemas folder needs to be parsable by the most recent version of the ConfigFileLibrary. Currently, that supports JSON and YAML files. 

Here's the structure:

```json
{
  "name": "IdAndTitleDTO", // Required
  "type": "Class", // Required (can be either [Class, Enum])
  "namespace": "ProjectTracker.Classes", // Required for C# classes only (only file-scoped is available)
  "ignoreWarnings": true, // Optional; uses the language-specific disable of warnings
  "properties": [
    // If the type is Class
    {
      "name": "Id", // These two properties are required
      "type": "int",
      "default": "Unknown", // You can assign a default value here (optional param)
      "nullable": true // You can make a property optional (optional param)
    },
    ... // Add as many properties here as needed
    // If the type is Enum
    "Type1", "Type2", ...
  ]
}
```

The current list of types supported are C# primitives. They are all converted to a TS type when being added to a TS file.

- string
  - string for JS, TS, Zod, and C#
- int, uint, byte, long, double, etc.
  - ___ for C#, number for JS, TS, and Zod
- bool
  - bool for C#, boolean for JS, TS, and Zod

Arrays are not yet supported. 

## HowlDev.Cli.FullStackBuilder

This is a global tool that combines the Vite and Dotnet builder to quickly bootstrap full-stack apps. Install it with the below command: 

```bash
dotnet tool install --global HowlDev.Cli.FullStackBuilder
```

Run the command with: 

```bash
fsbuild
```

Provide it with file names for your two projects, then follow the steps in the console to scaffold your Vite project and your backend (do _not_ install and build the frontend at the last step. I didn't find a flag to disable that feature, but it will break this app). 

After that, it will ask if you want to configure more. In both projects, you can install some common packages (make an issue/pull request if you have more packages you'd like), and also adjust some files with common features, such as building your frontend directly to your API. 

This is very much in it's early stages. 
