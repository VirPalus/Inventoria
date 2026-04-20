using Inventoria.Models;
using Inventoria.Models.Database;

internal static class Program
{
    private static void Main()
    {
        Product newProduct = new(
            "Corsair Vengeance 64GB DDR5-6000",
            "2x32GB DDR5 memory kit, CL30",
            "Memory",
            749.00,
            15,
            6
        );

        int newId = Database.Add(newProduct);

        Console.WriteLine($"Toegevoegd met id: {newId}");
        Console.WriteLine($"Naam: {Database.Read(newId, "name")}");
        Console.WriteLine($"Prijs: {Database.Read(newId, "price")}");
        Console.WriteLine($"Voorraad: {Database.Read(newId, "stock.quantity")}");


        Console.WriteLine($"\n[{newId}] Verwijderd: {Database.Remove(newId)}");

        Console.ReadLine();
    }
}