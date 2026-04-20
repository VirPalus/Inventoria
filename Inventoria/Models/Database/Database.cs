using System.Text.Json;
using System.Text.Json.Nodes;
using Inventoria.Models.Interfaces;

namespace Inventoria.Models.Database;

/// <summary>
/// Static helper class for managing the JSON database file.
/// <para>Supports reading, writing, adding and removing items by id.</para>
/// <para>Supports nested attribute access via dot notation (e.g. "stock.quantity").</para>
/// </summary>
public static class Database
{
    private static readonly string JsonFolderPath = Path.Combine(AppContext.BaseDirectory, "Models", "Database", "JSON");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Reads the database JSON file. Creates it if missing.
    /// </summary>
    /// <param name="content">The file content, or empty string if the file is empty.</param>
    /// <returns>True if content was read, false if file was empty or just created.</returns>
    public static bool TryFile(out string content)
    {
        content = string.Empty;
        string fullPath = Path.Combine(JsonFolderPath, "database.json");

        if (!File.Exists(fullPath))
        {
            Directory.CreateDirectory(JsonFolderPath);
            File.WriteAllText(fullPath, "{}");
            return false;
        }

        string fileContent = File.ReadAllText(fullPath);
        if (string.IsNullOrWhiteSpace(fileContent) || fileContent.Trim() == "{}")
        {
            return false;
        }

        content = fileContent;
        return true;
    }

    /// <summary>
    /// Navigates through a JSON element using a dot-separated path.
    /// </summary>
    /// <param name="element">The JSON element to start from.</param>
    /// <param name="path">Dot-separated path (e.g. "stock.quantity").</param>
    /// <returns>The resolved value as string, or empty string if any part was not found.</returns>
    private static string ResolvePath(JsonElement element, string path)
    {
        JsonElement current = element;
        string[] parts = path.Split('.');

        foreach (string part in parts)
        {
            if (!current.TryGetProperty(part, out JsonElement next))
            {
                return string.Empty;
            }
            current = next;
        }

        return current.ToString();
    }

    /// <summary>
    /// Sets a value at a dot-separated path inside a JSON node.
    /// </summary>
    /// <param name="node">The JSON node to start from.</param>
    /// <param name="path">Dot-separated path (e.g. "stock.quantity").</param>
    /// <param name="value">The new value.</param>
    /// <returns>True if the path was found and set, otherwise false.</returns>
    private static bool SetPath(JsonNode node, string path, object value)
    {
        string[] parts = path.Split('.');
        JsonNode current = node;

        for (int i = 0; i < parts.Length - 1; i++)
        {
            JsonNode? next = current[parts[i]];
            if (next is null)
            {
                return false;
            }
            current = next;
        }

        string lastKey = parts[^1];
        current[lastKey] = JsonValue.Create(value);
        return true;
    }

    /// <summary>
    /// Calculates the next available id by finding the current maximum and adding 1.
    /// </summary>
    /// <param name="array">The database array to check.</param>
    /// <returns>The next available id. Returns 1 if the array is empty.</returns>
    private static int GetNextId(JsonArray array)
    {
        int maxId = 0;

        foreach (JsonNode item in array.OfType<JsonNode>())
        {
            if (item["id"] is not JsonValue idNode)
            {
                continue;
            }

            int currentId = idNode.GetValue<int>();
            if (currentId > maxId)
            {
                maxId = currentId;
            }
        }

        return maxId + 1;
    }

    /// <summary>
    /// Saves the root JSON node back to the database file.
    /// </summary>
    /// <param name="root">The JSON root node to save.</param>
    private static void SaveRoot(JsonNode root)
    {
        string fullPath = Path.Combine(JsonFolderPath, "database.json");
        File.WriteAllText(fullPath, root.ToJsonString(JsonOptions));
    }

    /// <summary>
    /// Reads a value by id. Supports nested attributes via dot notation (e.g. "stock.quantity").
    /// </summary>
    /// <param name="id">Id of the item to find.</param>
    /// <param name="attribute">Attribute path, e.g. "name" or "stock.quantity".</param>
    /// <returns>The value as string, or empty string if not found.</returns>
    public static string Read(int id, string attribute)
    {
        if (!TryFile(out string content))
        {
            return string.Empty;
        }

        using JsonDocument doc = JsonDocument.Parse(content);
        JsonElement array = doc.RootElement.GetProperty("database");

        foreach (JsonElement item in array.EnumerateArray())
        {
            if (item.GetProperty("id").GetInt32() == id)
            {
                return ResolvePath(item, attribute);
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Writes a new value to an existing attribute. Supports nested attributes via dot notation.
    /// </summary>
    /// <param name="id">Id of the item to update.</param>
    /// <param name="attribute">Attribute path, e.g. "name" or "stock.quantity".</param>
    /// <param name="value">The new value.</param>
    /// <returns>True if the write succeeded, false if id or attribute was not found.</returns>
    public static bool Write(int id, string attribute, object value)
    {
        if (!TryFile(out string content))
        {
            return false;
        }
        if (JsonNode.Parse(content) is not JsonNode root)
        {
            return false;
        }
        if (root["database"] is not JsonArray array)
        {
            return false;
        }
        foreach (JsonNode item in array.OfType<JsonNode>())
        {
            if (item["id"] is not JsonValue idNode)
            {
                continue;
            }
            if (idNode.GetValue<int>() != id)
            {
                continue;
            }
            if (!SetPath(item, attribute, value))
            {
                return false;
            }
            SaveRoot(root);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds a new item to the database. Id is generated automatically.
    /// </summary>
    /// <param name="item">The item to add (must implement IJsonObject).</param>
    /// <returns>The id of the newly added item, or -1 if adding failed.</returns>
    public static int Add(IJsonObject item)
    {
        JsonObject obj = item.ToJsonObject();
        if (!TryFile(out string content))
        {
            content = "{\"database\":[]}";
        }
        if (JsonNode.Parse(content) is not JsonNode root)
        {
            return -1;
        }
        if (root["database"] is not JsonArray array)
        {
            return -1;
        }
        int newId = GetNextId(array);
        obj["id"] = newId;
        array.Add(obj);
        SaveRoot(root);
        return newId;
    }

    /// <summary>
    /// Removes an item from the database by id.
    /// </summary>
    /// <param name="id">Id of the item to remove.</param>
    /// <returns>True if the item was removed, false if id was not found.</returns>
    public static bool Remove(int id)
    {
        if (!TryFile(out string content))
        {
            return false;
        }
        if (JsonNode.Parse(content) is not JsonNode root)
        {
            return false;
        }
        if (root["database"] is not JsonArray array)
        {
            return false;
        }

        for (int i = 0; i < array.Count; i++)
        {
            JsonNode? item = array[i];
            if (item is null)
            {
                continue;
            }
            if (item["id"] is not JsonValue idNode)
            {
                continue;
            }
            if (idNode.GetValue<int>() != id)
            {
                continue;
            }

            array.RemoveAt(i);
            SaveRoot(root);
            return true;
        }

        return false;
    }
}