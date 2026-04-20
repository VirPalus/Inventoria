using System.Text.Json.Nodes;
using Inventoria.Models.Interfaces;

namespace Inventoria.Models;

/// <summary>
/// Represents a product in the inventory.
/// <para>Constructor parameters:</para>
/// <para>name, description, category, price, quantity, minQuantity.</para>
/// </summary>
public sealed class Product : IJsonObject
{
    /// <summary>The product name.</summary>
    public string Name { get; set; }

    /// <summary>The product description.</summary>
    public string Description { get; set; }

    /// <summary>The product category.</summary>
    public string Category { get; set; }

    /// <summary>The product price.</summary>
    public decimal Price { get; set; }

    /// <summary>The current stock quantity.</summary>
    public int Quantity { get; set; }

    /// <summary>The minimum stock level before alert.</summary>
    public int MinQuantity { get; set; }

    /// <summary>
    /// Initializes a new Product.
    /// </summary>
    /// <param name="name">Product name.</param>
    /// <param name="description">Product description.</param>
    /// <param name="category">Product category.</param>
    /// <param name="price">Product price.</param>
    /// <param name="quantity">Current stock quantity.</param>
    /// <param name="minQuantity">Minimum stock level before alert.</param>
    public Product(string name, string description, string category, double price, int quantity, int minQuantity)
    {
        Name = name;
        Description = description;
        Category = category;
        Price = (decimal)price;
        Quantity = quantity;
        MinQuantity = minQuantity;
    }

    /// <summary>
    /// Converts this product into a JsonObject for database storage.
    /// </summary>
    /// <returns>A JsonObject representing this product (without id).</returns>
    public JsonObject ToJsonObject()
    {
        return new JsonObject
        {
            ["name"] = Name,
            ["description"] = Description,
            ["category"] = Category,
            ["price"] = Price,
            ["stock"] = new JsonObject
            {
                ["quantity"] = Quantity,
                ["minQuantity"] = MinQuantity
            }
        };
    }
}