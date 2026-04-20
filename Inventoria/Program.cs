using Inventoria.Models;
using Inventoria.Models.Database;

internal static class Program
{
    private static void Main()
    {
        Product asusLaptop = new(
            "Asus Laptop",
            "Gaming Laptop",
            "laptop",
            1600.00,
            10,
            2
        );

        Database.Add(asusLaptop);

        Console.WriteLine(Database.Read(1,"price"));
        Database.Write(1,"price",5000.00);
        Console.WriteLine(Database.Read(1,"price"));

        Console.ReadLine();
    }
}