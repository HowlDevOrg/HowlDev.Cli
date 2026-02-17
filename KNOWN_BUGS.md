# Known Bugs

This is basically just my current work list. When one is fixed, I put the version it was fixed in at the start. 

- The TS export currently doesn't correctly import custom types.
- (0.3.3) Zod exports should prioritize array, then default (for a default array), as the other way breaks.
    - This also applies to `default()`.
- Imports don't automatically de-duplicate.
- New files should mimic file structure of the `/dtos` folder (so you can have nested items that have the same placement in both places). You can already nest folders, but they just export to the first level of folders.