# Felrapport

The starter code is commit `fe075b1a1bed4edf7d703e1dd44e8adec478484d`.
The bugs below are listed in the order of the commits that fixed them.

## Bug 1: Wrong total in Total()

**Steps to reproduce**
1. Make sure `items.txt` contains Mjölk 15, Bröd 32 and Ost 89 (No empty line).
2. Run `dotnet run`.
3. Look at the "Totalt" line under the list.

**Expected:** The program should display the list of items from `items.txt` and the total amount, which is 136 kr.

**Actual:** The program displayed 121 kr (`ShoppingList.cs`, line 28).

**Root cause:** In `ShoppingList.cs` the loop started at index 1 instead of 0, so the first item (index 0, Mjölk 15 kr) was never added to the sum. 136 − 15 = 121.

**Wrong code line:**
```csharp
for (int i = 1; i < items.Count; i++)
```
**Fix:** Replaced `int i = 1` with `int i = 0`

**Commit:** `35380ada67bc53a8a12445de6e40d0a5a3062c16`

## Bug 2: Item names missing except last item

**Steps to reproduce**
1. `items.txt` has Windows line endings (`\r\n`) and no empty line at the end.
2. Run `dotnet run`.

**Expected:** 
```text
1. Mjölk - 15 kr
2. Bröd - 32 kr
3. Ost - 89 kr
Totalt: 136 kr
```
**Actual:**  (`ShoppingList.cs`, line 84 and 85)
```text
- 15 kr
- 32 kr
3. Ost - 89 kr
Totalt: 136 kr
```
**Root cause:** Windows ends each line in a file with `\r\n`, but the old code split the text only at `\n`. That left a `\r` at the end of every name except the last one. When the terminal prints `\r`, the cursor returns to the start of the line, so the rest of the line (` - 15 kr`) was printed over the beginning of the line, and the name was hidden. The last line (`Ost`) had no line break after it, so there was no `\r` to cause this and it printed it correctly.


**Wrong code line:**
```csharp
    string text = File.ReadAllText(path);
    string[] lines = text.Split('\n');
```
**Fix:** 
```csharp
    string[] lines = File.ReadAllLines(path);
```  
`ReadAllLines` handles both `\r\n` and removes the line breaks, so no `\r` is left on the names.

**Commit:** `9f3c0495e2f177b85b309ea92b3e32ea51d27c3c`
(The commit message says "Refactor", but this commit fixed this bug and the startup crash in Bug 3.)

## Bug 3: Crash when the menu choice is not a number

**Steps to reproduce**
1. Run `dotnet run`.
2. At "Välj:", type `abc`.

**Expected:** The program shows a message asking for a number from the menu and returns to the menu.
**Actual:** The program crashes with `FormatException`: "The input string 'abc' was not in a correct format" (`Program.cs`, line 16).

**Root cause:** `int.Parse` throws a `FormatException` when the text is not a number. The exception was not caught, so the program ended.

**Wrong code line:**
```csharp
    int choice = int.Parse(Console.ReadLine());
```
**Fix:** 
```csharp
if (int.TryParse(Console.ReadLine(), out int choice)) 
```
the menu choice is now read with `int.TryParse`. If the input is not a number, the program prints "Felaktig inmatning. Vänligen ange en siffra från menyn." and shows the menu again.

**Commit:** `94e7153ae7b4f21a1130f0191956ad6d122ef70c`



## Bug 4: Crash when the price is not a number

**Steps to reproduce**
1. Run `dotnet run`.
2. Choose `1`, type the name `ägg`.
3. At "Pris:", type `abc`.

**Expected:** The program prints a message and asks for the price again. After a valid price (for example 69), the item is added and the list is shown, with no crash
```Text
1. Mjölk - 15 kr
2. Bröd - 32 kr
3. Ost - 89 kr
4. ägg - 69 kr
Totalt: 205 kr
```
**Actual:** program throws a `FormatException` since the input string 'abc' was not in a correct format (`Program.cs`, line 22-23)

**Root cause:** `int.Parse` throws `FormatException` when the text is not a number, and nothing caught it. The user had already typed a name, so the whole entry was lost. 

**Wrong code line:**
```csharp
     Console.Write("Pris: ");
    int price = int.Parse(Console.ReadLine());
```

**Fix:** 
```csharp
int price = 0;
            bool valid = false;

            while (!valid)
            {
                Console.Write("Pris: ");

                if (int.TryParse(Console.ReadLine(), out price))
                {
                    valid = true;
                }
                else
                {
                    Console.WriteLine("Det där var inte ett heltal, försök igen.");
                }
            }
```
The price is now read in a while loop with int.TryParse. If the input is not a number, the program prints a message and asks again. The item is added to the list only after a valid price has been entered.

**Commit:** `7e1fcb24becfac1d65f6e80a7f48a10ced11d01d`



## Bug 5: Crash when the item number is outside the list

**Steps to reproduce**
1. Run `dotnet run`.
2. Choose `2`.
3. At "Nummer:", type `0`.

**Expected:** the program says the number is invalid and returns to the menu, without crashing

**Actual:** Program crashes and throws a `ArgumentOutOfRangeException` (`ShoppingList.cs`, line 18, 20, called from `Program.cs`, line 44)

**Root cause:** the user sees numbering from 1, the code subtracts 1 to get the index, and `0 − 1 = −1` doesn't exist. `99` is too large. There was no check before `items.RemoveAt(number - 1)`

**Wrong code line:**  
`Program.cs`
```csharp
list.RemoveAt(number);
```
`ShoppingList.cs`
```csharp
line_18: public void RemoveAt(int number)
line_20: items.RemoveAt(number - 1);
```
**Fix:**
`program.cs`
```csharp
if (list.RemoveAt(number))
            {
                Console.WriteLine("Varan har tagits bort.");
            }
            else
            {
                Console.WriteLine("Ogiltigt nummer. Vänligen välj ett nummer från listan.");
            }
```
`ShoppingList.cs`
```csharp
public bool RemoveAt(int number)
    {
        items.RemoveAt(number - 1);
        if (number >= 1 && number <= items.Count)
        {
            items.RemoveAt(number - 1);
            return true;
        }
        return false;
    }
```
`RemoveAt` now checks that `number >= 1 && number <= items.Count` and returns true or false. `Program.cs` prints a message when it returns false, so ShoppingList handles the data and Program.cs handles what the user sees.**Commit:** `058bc74b606c8ce48eec1ab06a4eb54b907b0ed9`


## Bug 6: Crash when the item number is not a number

**Steps to reproduce**
1. Run `dotnet run`.
2. Choose `2`.
3. At "Nummer:", type `abc`.

**Expected:** the program says the entry is invalid and returns to the menu, without crashing
**Actual:** Program crashes and throws a `FormatException` (`Program.cs`, line 43)

**Root cause:** `int.Parse` throws `FormatException` when the text is not a number, and nothing caught it, so the program ended. This is the third input in the program that used `int.Parse` (after the menu choice and the price), and each prompt has its own steps to reproduce, so it is a separate bug.

**Wrong code line:**
```csharp
int number = int.Parse(Console.ReadLine());
```

**Fix:**
```csharp
 else if (choice == 2)
        {
            Console.Write("Nummer: ");
            int number = int.Parse(Console.ReadLine());
            if (int.TryParse(Console.ReadLine(), out int number))
            {
            if (list.RemoveAt(number))
            {
                Console.WriteLine("Varan har tagits bort.");
            {
                Console.WriteLine("Ogiltigt nummer. Vänligen välj ett nummer från listan.");
            }
        }
            else
            {
                Console.WriteLine("Felaktig inmatning. Vänligen ange en siffra.");
            }
        }
```
The Nummer: input is now read with int.TryParse. If the input is not a number, the program prints "Felaktig inmatning. Vänligen ange en siffra." and returns to the menu.


**Commit:** `0e1f6f6c2bae460ba7ef880740bb3bede53db8c7`


## Bug 7: Fix silent failure when saving to a read-only file


**Steps to reproduce**
1. Make `items.txt` read-only (right-click, Properties, tick Read-only).
2. Run `dotnet run`.
3. Choose `1` and add an item.
4. Choose `3` (Spara).

**Expected:** Program writes a message saying if file is read-only (if ifle is read-only) if not then it saves into `Items.txt`.
**Actual:** The program prints "Listan är sparad.", but items.txt is unchanged. No error is shown and the program does not crash. This is a hidden failure: the user is told the list was saved when it was not.  (`ShoppingList.cs`, `program.cs`)

**Root cause:** `File.WriteAllText` throws `UnauthorizedAccessException` when the file is read-only. The empty `catch { }` swallowed the exception, so nothing told the user. The line `Console.WriteLine("Listan är sparad.");` came after the `try/catch`, so it ran whether the save worked or not.

**Wrong code line:**
`ShoppingList.cs`, line 66 - 82
```csharp
public void Save()
    {
        List<string> lines = new List<string>();

        foreach (Item item in items)
        {
            lines.Add($"{item.Price};{item.Name}");
        }

        try
        {
            File.WriteAllText(path, string.Join("\r\n", lines) + "\r\n");
        }
        catch
        {
        }

        Console.WriteLine("Listan är sparad.");
    }
```

`program.cs`, line 61
```csharp
                list.Save();
```
**Fix:**
`ShoppingList.cs`, line 66 - 88
```csharp
public bool Save()
    {
        List<string> lines = new List<string>();

        foreach (Item item in items)
        {
            lines.Add($"{item.Price};{item.Name}");
        }

        try
        {
            File.WriteAllText(path, string.Join("\r\n", lines) + "\r\n");
            return true;
        }
        catch (UnauthorizedAccessException)
            {
                return false;
            }
        catch (IOException)
        {
            return false;
        }
    }
```
`program.cs`, line 61 - 69
```CSHARP
 if (list.Save())
            {
                Console.WriteLine ("Listan är sparad.");
            }
            else
            {
                Console.WriteLine ("Misslyckades med att spara listan. Kontrollera om filen är skrivskyddad (read-only).");
            }
        }
```

**Commit:** `2eb9c9af2a40edb3d5d7ce9d4601eb173cf80fe7`






# Designval




# Klassdiagram