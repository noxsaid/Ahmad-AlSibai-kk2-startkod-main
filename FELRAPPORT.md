## Felrapport

| # | Bug | Where | Cause | Commit hash |
|---|-----|-------|-------|-------------|
| 1 | Startup crash (`IndexOutOfRangeException`) | `ShoppingList.cs:90`, called from `Program.cs:2` | `Split('\n')` leaves an empty last piece, so `parts[1]` doesn't exist | `9f3c0495e2f177b85b309ea92b3e32ea51d27c3c` |
| 2 | Names vanish on screen | `Load()` | The `\r` of `\r\n` stays on the names, and the terminal jumps back to the line start | `9f3c0495e2f177b85b309ea92b3e32ea51d27c3c` (same commit as #1) |
| 3 | Wrong total (121 instead of 136) | `ShoppingList.cs:28`, `Total()` | The loop started at `i = 1`, so the first item was skipped | `35380ada67bc53a8a12445de6e40d0a5a3062c16` |
| 4 | Menu crash on `abc` (`FormatException`) | `Program.cs:16` | `int.Parse` on user input | `94e7153ae7b4f21a1130f0191956ad6d122ef70c` |
| 5 | `Pris:` crash on `abc` | `Program.cs:23` | `int.Parse` on user input | `7e1fcb24becfac1d65f6e80a7f48a10ced11d01d` |
| 6 | Number outside the list at `Nummer:` (`ArgumentOutOfRangeException`) | `ShoppingList.cs:20`, from `Program.cs:44` | No bounds check before `items.RemoveAt(number - 1)` | `058bc74b606c8ce48eec1ab06a4eb54b907b0ed9` |
| 7 | `Nummer:` crash on `abc` | `Program.cs:43` | `int.Parse` on user input | `0e1f6f6c2bae460ba7ef880740bb3bede53db8c7` |
| 8 | Hidden failure: "Listan är sparad." printed when the file is read-only | `Save()` in `ShoppingList.cs` | An empty `catch { }` swallowed `UnauthorizedAccessException`, and the success message printed anyway | `2eb9c9af2a40edb3d5d7ce9d4601eb173cf80fe7` |



