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

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Gets the next available id that will be assigned to a new item.
    /// Takes the last item's id + 1. Returns 1 if the database is empty.
    /// </summary>
    public static int NextId
    {
        get
        {
            if (!TryLoadRoot(out JsonNode root))
            {
                return 1;
            }

            if (root["database"] is not JsonArray array)
            {
                return 1;
            }

            if (array.Count == 0)
            {
                return 1;
            }

            int lastIndex = array.Count - 1;
            if (array[lastIndex] is not JsonNode lastItem)
            {
                return array.Count + 1;
            }

            if (lastItem["id"] is not JsonValue id)
            {
                return array.Count + 1;
            }

            return id.GetValue<int>() + 1;
        }
    }

    /// <summary>
    /// Checks whether an item with the given id exists in the database.
    /// </summary>
    /// <param name="id">Id of the item to check.</param>
    /// <returns>True if the item exists, otherwise false.</returns>
    public static bool Exists(int id)
    {
        return !string.IsNullOrEmpty(Read(id, "id"));
    }

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
    /// Loads and parses the database JSON file. Creates it if missing.
    /// </summary>
    /// <param name="root">The parsed root node, or null if the file is empty.</param>
    /// <returns>True if the file was loaded and parsed, false if empty or just created.</returns>
    private static bool TryLoadRoot(out JsonNode root)
    {
        root = null!;

        if (!TryFile(out string content))
        {
            return false;
        }

        if (JsonNode.Parse(content) is not JsonNode parsed)
        {
            return false;
        }

        root = parsed;
        return true;
    }

    /// <summary>
    /// Saves the root JSON node back to the database file.
    /// </summary>
    /// <param name="root">The JSON root node to save.</param>
    private static void SaveRoot(JsonNode root)
    {
        string fullPath = Path.Combine(JsonFolderPath, "database.json");
        File.WriteAllText(fullPath, root.ToJsonString(WriteOptions));
    }

    /// <summary>
    /// Sets a value at a dot-separated path inside a JSON node.
    /// Only primitive values can be overwritten, not nested objects or arrays.
    /// </summary>
    /// <param name="node">The JSON node to start from.</param>
    /// <param name="path">Dot-separated path (e.g. "stock.quantity").</param>
    /// <param name="value">The new value.</param>
    /// <returns>True if the path was found and set, otherwise false.</returns>
    private static bool SetPath(JsonNode node, string path, object value)
    {
        JsonNode current = node;
        string[] parts = path.Split('.');

        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (current[parts[i]] is not JsonNode next)
            {
                return false;
            }
            current = next;
        }

        int lastIndex = parts.Length - 1;
        string lastKey = parts[lastIndex];

        if (current[lastKey] is not JsonValue)
        {
            return false;
        }

        current[lastKey] = ConvertToJsonNode(value);
        return true;
    }

    /// <summary>
    /// Navigates through a JSON node using a dot-separated path.
    /// </summary>
    /// <param name="node">The JSON node to start from.</param>
    /// <param name="path">Dot-separated path (e.g. "stock.quantity").</param>
    /// <returns>The resolved value as string, or empty string if any part was not found.</returns>
    private static string ResolvePath(JsonNode node, string path)
    {
        JsonNode current = node;
        string[] parts = path.Split('.');

        foreach (string part in parts)
        {
            if (current[part] is not JsonNode next)
            {
                return string.Empty;
            }
            current = next;
        }

        return current.ToString();
    }

    /// <summary>
    /// Converts a .NET value into a JsonNode using typed overloads to avoid custom serialization.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A JsonNode representing the value.</returns>
    private static JsonNode ConvertToJsonNode(object value)
    {
        if (value is string stringValue)
        {
            return JsonValue.Create(stringValue)!;
        }
        if (value is int intValue)
        {
            return JsonValue.Create(intValue);
        }
        if (value is long longValue)
        {
            return JsonValue.Create(longValue);
        }
        if (value is double doubleValue)
        {
            return JsonValue.Create(doubleValue);
        }
        if (value is decimal decimalValue)
        {
            return JsonValue.Create(decimalValue);
        }
        if (value is bool boolValue)
        {
            return JsonValue.Create(boolValue);
        }

        string fallback = value.ToString() ?? string.Empty;
        return JsonValue.Create(fallback)!;
    }

    /// <summary>
    /// Reads an entire item from the database by id.
    /// </summary>
    /// <param name="id">Id of the item to find.</param>
    /// <returns>The item as a JSON string, or empty string if not found.</returns>
    public static string ReadNode(int id)
    {
        if (!TryLoadRoot(out JsonNode root))
        {
            return string.Empty;
        }

        if (root["database"] is not JsonArray array)
        {
            return string.Empty;
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
            return item.ToJsonString(WriteOptions);
        }

        return string.Empty;
    }

    /// <summary>
    /// Reads a value by id. Supports nested attributes via dot notation (e.g. "stock.quantity").
    /// </summary>
    /// <param name="id">Id of the item to find.</param>
    /// <param name="attribute">Attribute path, e.g. "name" or "stock.quantity".</param>
    /// <returns>The value as string, or empty string if not found.</returns>
    public static string Read(int id, string attribute)
    {
        if (!TryLoadRoot(out JsonNode root))
        {
            return string.Empty;
        }

        if (root["database"] is not JsonArray array)
        {
            return string.Empty;
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
            return ResolvePath(item, attribute);
        }

        return string.Empty;
    }

    /// <summary>
    /// Writes a new value to an existing attribute. Supports nested attributes via dot notation.
    /// The id field cannot be overwritten to prevent database corruption.
    /// </summary>
    /// <param name="id">Id of the item to update.</param>
    /// <param name="attribute">Attribute path, e.g. "name" or "stock.quantity".</param>
    /// <param name="value">The new value.</param>
    /// <returns>True if the write succeeded, false if id or attribute was not found, or if attribute is "id".</returns>
    public static bool Write(int id, string attribute, object value)
    {
        if (attribute == "id")
        {
            return false;
        }

        if (!TryLoadRoot(out JsonNode root))
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
        JsonObject sourceObj = item.ToJsonObject();

        if (!TryLoadRoot(out JsonNode root))
        {
            if (JsonNode.Parse("{\"database\":[]}") is not JsonNode emptyRoot)
            {
                return -1;
            }
            root = emptyRoot;
        }

        if (root["database"] is not JsonArray array)
        {
            return -1;
        }

        int newId = NextId;

        JsonObject finalObj = new()
        {
            ["id"] = newId
        };

        foreach (KeyValuePair<string, JsonNode?> pair in sourceObj)
        {
            if (pair.Value is not JsonNode value)
            {
                continue;
            }
            finalObj[pair.Key] = value.DeepClone();
        }

        array.Add(finalObj);
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
        if (!TryLoadRoot(out JsonNode root))
        {
            return false;
        }

        if (root["database"] is not JsonArray array)
        {
            return false;
        }

        for (int i = 0; i < array.Count; i++)
        {
            if (array[i] is not JsonNode item)
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