## Code style:

### Naming:
- camelCase with minor changes: const and public variables start with lowercase letter. No '_' inside variable name.
- All functions start with capital letter and fully describe their action. No more than 4 words in function name.
- Function names are verbs.

### Structure:
- Every class excluding key ones (World, Player e.t.c.) should not be more than 100 lines long.
- No line should be longer than 150 symbols.
- Public functions go before private. Methods go before functions. Often used functions go before less used.
- Public keyword should be used only if it's needed.
- No unused imports.
- Group logically connected methods and functions into regions if there are more than 3 of them.
- Use readonly keyword if IDE suggests or if needed.

### Complexity:
To analyze complexity in VS you can run: Analyze -> Calculate Code Metrics -> For Solution.

- Keep maintainability above 40
- Keep cyclomatic complexity below 8 in each function
