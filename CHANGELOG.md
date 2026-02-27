# HowlDev.Cli.TextDTO

## Changelog

0.4.0 (2/26/26)

- Works now..

0.4.0-beta.1 (2/26/26)

- Version bump, update Directory search so that it checks nested directories (which I was pretty sure it did, then it broke..).

0.4.0-beta (2/20/26)

- Updated the underlying system to create a strongly-typed object before creating objects. I'm transitioning towards a separate code generation library, and this is the first step for. (This required an update to the ConfigFileLibrary). 

0.3.1 - 0.3.5 (2/17/26)

- Fixed it so Arrays of custom types are now properly created/referenced.
- Missed a replacement function in the C# builder (0.3.2)
- I'm _SOOO_ stupid. Zod now does all properties (0.3.3). 
  - Also fixed the default ordering for Zod types.
- Fixed Null defaults for all types. (0.3.4)
- Changed ordering in Zod to allow nullable arrays to be null. (0.3.5)

0.3.0 (2/16/26)

- Updated system to automatically handle cross references, so you can make nested types (at least I hope). 

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

# HowlDev.Cli.FullStackBuilder


## Changelog

0.1.0-beta.1 (2/26/26)

- Beta testing the NuGet installation section. I'm quite happy with it!
- After publishing the beta, included a clause to only make the top level folder if it doesn't already exist.

0.0.1 (2/16/26)

- Init
- Can create both projects, install some packages, adjust some files.
