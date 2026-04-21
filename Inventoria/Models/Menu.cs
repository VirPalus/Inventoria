using Inventoria.Models;
using Inventoria.Models.Database;

namespace Inventoria;

/// <summary>
/// A class that gives a menu.
/// </summary>
public static class Menu
{
    /// <summary>
    /// Main menu text.
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
    /// Second menu text.
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
    /// Gives a menu based on question.
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
        } while (!(int.TryParse(input, out keuzen) && keuzen >= 0 && keuzen <= 6));

        if (keuzen == 0)
        {
            return keuzen;
        }

        if (menu.Contains("=== Inventoria ==="))
        {
          
            switch (keuzen)
            {
                case 1:
                    ProductToevoegen(keuzen);
                break;
                
                case 2:
                    int keuzen2 = ToonMenu(AanpassingsMenu);
                    while (keuzen2 != 0)
                    {
                        ProductAanpassen(keuzen2);
                        keuzen2 = ToonMenu(AanpassingsMenu);
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
    /// Menu Product Toevoegen.
    /// </summary>
    public static void ProductToevoegen(int keuzen)
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
    /// Menu Product Aanpassen.
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
    /// Methode om een Int te vragen.
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
    /// Methode om een String te vragen.
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
    /// Methode om een Double te vragen.
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