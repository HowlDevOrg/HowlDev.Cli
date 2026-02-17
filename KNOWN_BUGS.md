# Known Bugs

This is basically just my current work list. When one is fixed, I put the version it was fixed in at the start. 

- The TS export currently doesn't correctly import custom types.
- (0.3.3) Zod exports should prioritize array, then default (for a default array), as the other way breaks.
    - This also applies to `default()`.