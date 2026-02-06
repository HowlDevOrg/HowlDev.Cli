# HowlDev.Core

## HowlDev.Core.TextDTO

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
  "namespace": "ProjectTracker.Classes", // This is an optional thing for C# classes only (only file-scoped is available)
  "ignoreWarnings": true, // Also optional; uses the language-specific disable of warnings
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

## Changelog

0.2.1 (2/6/26)

- Updated internal HowlDev.IO.Text.ConfigFile to handle some edge cases better in JSON. 

0.2 (2/3/26)

- BREAKING CHANGE
  - Files now need the "type" parameter in the top level of the object. This can only be a Class or Enum (currently).
- New feature: Enums! Enums are proper Enum types in C#, type unions in JS (`type Thing = "one" | "two"`), and proper enums in Zod. 
- More resilient tests (that I probably should have considered a while ago). 

0.1.2 (2/3/26)

- Added Zod support for the array type

0.1.1 (1/8/26)

- Found out about the RollForward property for applications, so now you don't have to specifically have the 8.0 runtime to run this app. 

0.1.0 (1/8/26)

- Initialized
- Only primitives are really supported. No type checking happens. 