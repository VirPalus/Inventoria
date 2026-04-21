using Inventoria.Models;
using Inventoria.Models.Database;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine(Database.ReadNode(1));
        Console.ReadLine();
    }
}