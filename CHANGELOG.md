# HowlDev.Cli.TextDTO

## Changelog

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

0.0.1 (2/16/26)

- Init
- Can create both projects, install some packages, adjust some files.
