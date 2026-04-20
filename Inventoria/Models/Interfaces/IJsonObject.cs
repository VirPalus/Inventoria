namespace Inventoria.Models.Interfaces;

using System.Text.Json.Nodes;

/// <summary>
/// Represents an object that can be converted to a JsonObject.
/// </summary>
public interface IJsonObject
{
    /// <summary>
    /// Converts this object into a JsonObject for database storage.
    /// </summary>
    /// <returns>A JsonObject representation of this object.</returns>
    JsonObject ToJsonObject();
}