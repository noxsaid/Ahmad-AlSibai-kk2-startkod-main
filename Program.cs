ShoppingList list = new ShoppingList("items.txt");
list.Load();

while (true)
{
    Console.WriteLine();
    list.Print();
    Console.WriteLine();
    Console.WriteLine("1. Lägg till vara");
    Console.WriteLine("2. Ta bort vara");
    Console.WriteLine("3. Spara");
    Console.WriteLine("4. Sök vara");
    Console.WriteLine("5. Avsluta");
    Console.Write("Välj: ");

    if (int.TryParse(Console.ReadLine(), out int choice))
    {
        if (choice == 1)
        {
            Console.Write("Namn: ");
            string name = Console.ReadLine();
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
            list.Add(new Item(name, price));
        }
        else if (choice == 2)
        {
            Console.Write("Nummer: ");
            int number = int.Parse(Console.ReadLine());
            list.RemoveAt(number);
        }
        else if (choice == 3)
            {
                list.Save();
            }
        else if (choice == 4)
            {
                Console.Write("Namn att söka efter: ");
                string wanted = Console.ReadLine();
                Item found = list.Find(wanted);

            if (found == null)
            {
                Console.WriteLine("Varan finns inte i listan.");
            }
            else
            {
                Console.WriteLine($"Hittade: {found}");
            }
        }
        else if (choice == 5)
        {
            break;
        }
        else
        {
            Console.WriteLine("Ogiltigt val, välj ett nummer från menyn (1-5).");
        }
    }
    else
    {
        Console.WriteLine("Felaktig inmatning. Vänligen ange en siffra från menyn.");
    }
}