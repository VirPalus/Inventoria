using Inventoria.Models;
using Inventoria.Models.Database;

namespace Inventoria;

/// <summary>
/// Beheert de menuweergave van de applicatie en verwerkt de gekozen menu-acties.
/// </summary>
public static class Menu
{
    /// <summary>
    /// Bevat de tekstuele inhoud van het hoofdmenu.
    /// </summary>
    public static readonly string HoofdMenu = 
    "=== Inventoria ===\n"+
    "1. Product toevoegen\n" + 
    "2. Product aanpassen\n"+
    "3. Product verwijderen\n"+
    "4. Product zoeken\n"+
    "5. Alle producten tonen\n"+
    "6. Low-stock overzicht\n"+
    "0. Afsluiten\n\n"+
    "Keuzen: ";

    /// <summary>
    /// Bevat de tekstuele inhoud van het menu voor productaanpassingen.
    /// </summary>
    public static readonly string AanpassingsMenu = 
    "Wat wil je aanpassen?\n"+
    "1. Naam\n" + 
    "2. Beschrijving\n"+
    "3. Categorie\n"+
    "4. Prijs\n"+
    "5. Voorraad aanpassen\n"+
    "6. Minimum voorraad aanpassen\n"+
    "0. Terug\n\n"+
    "Keuzen: ";


    /// <summary>
    /// Toont het opgegeven menu, vraagt een geldige keuze en voert de bijhorende actie uit.
    /// </summary>
    public static int ToonMenu(string menu)
    {
        string input;
        int keuzen;
        do
        {
            Console.Clear();
            Console.Write(menu);
            input = Console.ReadLine() ?? string.Empty;
        } while(!(int.TryParse(input, out keuzen) && keuzen >= 0 && keuzen <= 6)); // Blijf doorgaan zolang de keuzen geen geldig getal tussen 0 en 6 is.
        if (keuzen == 0)
        {
            return keuzen;
        }

        if (menu == HoofdMenu)
        {
            switch (keuzen)
            {
                case 1:
                    ProductToevoegen();
                break;
                
                case 2:
                    int aanpassing = ToonMenu(AanpassingsMenu);
                    while (aanpassing != 0)
                    {
                        ProductAanpassen(aanpassing);
                        aanpassing = ToonMenu(AanpassingsMenu);
                    }
                break;

                case 3:
                    //Product verwijderen
                break;

                case 4:
                    //Product zoeken
                break;

                case 5:
                    //Alle producten tonen
                break;

                case 6:
                    //Low-stock overzicht
                break;
            }
        }
        return keuzen;
    }

    /// <summary>
    /// Vraagt productgegevens op, maakt een nieuw product aan en slaat dit op in de database.
    /// </summary>
    public static void ProductToevoegen()
    {
        
        string name = VraagString("Name: ");
        string description = VraagString("Description: ");
        string category = VraagString("Category: ");
        double price = VraagDouble("Prijs: ");
        int quantity = VraagInt("Quantity: ");
        int minQuantity = VraagInt("minQuantity: ");

        Product newProduct = new(name, description, category, price, quantity, minQuantity);
        int id = Database.Add(newProduct);
        Bevestiging($"\n{Database.Read(id, "name")} is toegevoegd.");
    }

    /// <summary>
    /// Past een specifiek veld van een bestaand product aan op basis van de gekozen optie.
    /// </summary>
    public static void ProductAanpassen(int keuzen)
    {
        switch (keuzen)
        {
            case 1:
                Database.Write(VraagId("Id: "),"name",VraagString("Name: "));
                Bevestiging("\nProduct is aangepast.");
            break;
            
            case 2:
                Database.Write(VraagId("Id: "),"description",VraagString("Description: "));
                Bevestiging("\nProduct is aangepast.");
            break;

            case 3:
                Database.Write(VraagId("Id: "),"category",VraagString("Category: "));
                Bevestiging("\nProduct is aangepast.");
            break;

            case 4:
                Database.Write(VraagId("Id: "),"price",VraagDouble("Price: "));
                Bevestiging("\nProduct is aangepast.");
            break;

            case 5:
                Database.Write(VraagId("Id: "),"stock.quantity",VraagInt("Quantity: "));
                Bevestiging("\nProduct is aangepast.");
            break;

            case 6:
                Database.Write(VraagId("Id: "),"stock.minQuantity",VraagInt("MinQuantity: "));
                Bevestiging("\nProduct is aangepast.");
            break;

            default:
            break;
        }
    }
    /// <summary>
    /// Vraagt een geldig bestaand id op uit de database.
    /// </summary>
    public static int VraagId(string vraag)
    {
        string input;
        int getal;
        bool geldig;

        do
        {
            Console.Write(vraag);
            input = Console.ReadLine() ?? string.Empty;

            geldig = int.TryParse(input, out getal) && Database.Exists(getal);

            if (!geldig)
            {
                ClearLine();
            }

        } while (!geldig);

        return getal;
    }

    /// <summary>
    /// Vraagt invoer aan de gebruiker totdat een geldig geheel getal wordt ingevoerd.
    /// </summary>
    public static int VraagInt(string vraag)
    {
        string input;
        int getal;
        do
        {
            Console.Write(vraag);
            input = Console.ReadLine() ?? string.Empty;
        } while(!(int.TryParse(input, out getal) && getal >= 0));
        return getal;
    }

    /// <summary>
    /// Vraagt invoer aan de gebruiker totdat een niet-lege tekstwaarde wordt ingevoerd.
    /// </summary>
    public static string VraagString(string vraag)
    {
        string input;
        do
        {
            Console.Write(vraag);
            input = Console.ReadLine() ?? string.Empty;
        } while(string.IsNullOrWhiteSpace(input));
        return input;
    }

    /// <summary>
    /// Vraagt invoer aan de gebruiker totdat een geldig kommagetal wordt ingevoerd.
    /// </summary>
    public static double VraagDouble(string vraag)
    {
        string input;
        double getal;
        do
        {
            Console.Write(vraag);
            input = Console.ReadLine() ?? string.Empty;
        } while(!(double.TryParse(input, out getal) && getal >= 0));
        return getal;
    }

    /// <summary>
    /// Drukt een bevestigingstekst af en wacht op enter.
    /// </summary>
    private static void Bevestiging(string bevestiging)
    {
        Console.WriteLine(bevestiging);
        Console.WriteLine("Druk op Enter om verder te gaan...");
        Console.ReadLine();
    }

    /// <summary>
    /// Wist een consolelijn.
    /// </summary>
    private static void ClearLine()
    {
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write(new string(' ', Console.WindowWidth - 1));
        Console.SetCursorPosition(0, Console.CursorTop);
    }
}