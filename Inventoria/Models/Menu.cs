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
    public static string HoofdMenu = 
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
    public static string AanpassingsMenu = 
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
        } while(!(int.TryParse(input, out keuzen) && keuzen >= 0 && keuzen <= 6)); // Blijf doorgaan zolang NIET kan parse EN groter of gelijk is aan 0 EN kleiner of gelijk is aan 6.

        if (keuzen == 0)
        {
            return keuzen;
        }

        if (menu.Contains("=== Inventoria ==="))
        {
          
            switch (keuzen)
            {
                case 1:
                    ProductToevoegen();
                break;
                
                case 2:
                    int Aanpassing = ToonMenu(AanpassingsMenu);
                    while (Aanpassing != 0)
                    {
                        ProductAanpassen(Aanpassing);
                        Aanpassing = ToonMenu(AanpassingsMenu);
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

        Product NewProduct = new(name, description, category, price, quantity, minQuantity);
        int id = Database.Add(NewProduct);
        Console.WriteLine($"{Database.Read(id,"name")} is toegevoegd.\n");

    }

    /// <summary>
    /// Past een specifiek veld van een bestaand product aan op basis van de gekozen optie.
    /// </summary>
    public static void ProductAanpassen(int keuzen)
    {
        switch (keuzen)
        {
            case 1:Database.Write(VraagInt("Id: "),"name",VraagString("Name: "));
            break;
            
            case 2:Database.Write(VraagInt("Id: "),"description",VraagString("Description: "));
            break;

            case 3:Database.Write(VraagInt("Id: "),"category",VraagString("Category: "));
            break;

            case 4:Database.Write(VraagInt("Id: "),"price",VraagString("Price: "));
            break;

            case 5:Database.Write(VraagInt("Id: "),"stock.quantity",VraagString("Quantity: "));
            break;

            case 6:Database.Write(VraagInt("Id: "),"stock.minQuantity",VraagString("MinQuantity: "));
            break;

            default:
            break;
        }
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
        } while (!int.TryParse(input, out getal));
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
        } while (string.IsNullOrWhiteSpace(input));
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
        } while (!double.TryParse(input, out getal));
        return getal;
    }
}