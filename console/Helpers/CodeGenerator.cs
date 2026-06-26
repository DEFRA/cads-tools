using Npgsql;
using System.Text;

namespace Cads.Tools.Console.Helpers;

internal class CodeGenerator
{
    public const string TableInfoAttributeCode = 
        "[AttributeUsage(AttributeTargets.Field)]" +
        "\npublic class TableInfoAttribute : Attribute" +
        "\n{" +
        "\n\tpublic string TableName { get; }" +
        "\n\tpublic string Schema { get; }" +  
        "\n\tpublic string PrimaryKey { get; }" +
        "\n\n\tpublic TableInfoAttribute(string tableName, string schema, string primaryKey)" +
        "\n\t{\n" +
        "\n\t\tTableName = tableName;" +
        "\n\t\tSchema = schema;" +
        "\n\t\tPrimaryKey = primaryKey;" +
        "\n\t}" +
        "\n}";

    public static async Task<string> GenerateTableEnumAsync(string connectionString, string schema = "public", string? @namespace = null, bool verbose = false)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// Auto-generated code from PostgreSQL schema");
        sb.Append(RenderNamespace(@namespace));
        sb.AppendLine();

        // Add the TableInfo attribute class
        sb.AppendLine(TableInfoAttributeCode);
        sb.AppendLine();

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        if (verbose) System.Console.WriteLine("Fetching tables from PostgreSQL...");

        await using var cmd = await SqlCommandBuilder.GetSchemaWithPkQueryCommand(conn, schema);
        await using var reader = await cmd.ExecuteReaderAsync();

        var tables = new List<(string Name, string Schema, string PrimaryKey)>();

        while (await reader.ReadAsync())
        {
            var tableName = reader.GetString(reader.GetOrdinal("table_name"));
            var tableSchema = reader.GetString(reader.GetOrdinal("table_schema"));
            var primaryKey = reader.IsDBNull(reader.GetOrdinal("primary_key")) ? "id" : reader.GetString(reader.GetOrdinal("primary_key"));

            tables.Add((tableName, tableSchema, primaryKey));
            if (verbose) System.Console.WriteLine($"\tFound Table: {tableName} (PrimaryKey: {primaryKey})");
        }

        // Generate enum
        sb.AppendLine("public enum Tables");
        sb.AppendLine("{");

        for (int i = 0; i < tables.Count; i++)
        {
            var (tableName, tableSchema, primaryKey) = tables[i];
            var enumMemberName = ToPascalCase(tableName);

            sb.AppendLine($"\t[TableInfo(\"{tableName}\", \"{tableSchema}\", \"{primaryKey}\")]");
            sb.Append($"\t{enumMemberName}");

            if (i < tables.Count - 1)
            {
                sb.AppendLine(",");
            }
            else
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    public static async Task<string> GenerateClassFromSchemaAsync(string connectionString, string schema = "public", string? @namespace = null, bool verbose = false)
    {   
        var sb = new StringBuilder();
        sb.AppendLine("// Auto-generated code from PostgreSQL schema");
        sb.Append(RenderNamespace(@namespace));
        sb.AppendLine();

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        if (verbose) System.Console.WriteLine("Fetching table schema from PostgreSQL...");

        await using var cmd = await SqlCommandBuilder.GetSchemaQueryCommand(conn, schema);
        await using var reader = await cmd.ExecuteReaderAsync();

        string? currentTable = null;
        var classLines = new StringBuilder();

        var tables = new List<(string TableName, string ColumnName, string DataType, bool IsNullable)>();

        while (await reader.ReadAsync())
        {
            var tableName = reader.GetString(reader.GetOrdinal("table_name"));
            var columnName = reader.GetString(reader.GetOrdinal("column_name"));
            var dataType = reader.GetString(reader.GetOrdinal("data_type"));
            var isNullable = reader.GetString(reader.GetOrdinal("is_nullable")) == "YES";

            tables.Add((tableName, columnName, dataType, isNullable));
            if (verbose) System.Console.WriteLine($"\tFound Table: {tableName}, Column: {columnName}, DataType: {dataType}, IsNullable: {isNullable}");
        }

        for (int i = 0; i < tables.Count; i++)
        {
            var (tableName, columnName, dataType, isNullable) = tables[i];
            var enumMemberName = ToPascalCase(tableName);

            // Start a new class when table changes
            if (tableName != currentTable)
            {
                if (currentTable != null)
                {
                    classLines.AppendLine("}");
                    sb.Append(classLines);
                    sb.AppendLine();
                    classLines.Clear();
                }

                currentTable = tableName;
                var className = ToPascalCase(tableName);
                classLines.AppendLine($"public class {className}");
                classLines.AppendLine("{");

                if (verbose) System.Console.WriteLine($"\tGenerating class for Table: {tableName}");
            }

            var propertyName = ToPascalCase(columnName);
            var csType = MapPostgresToCSharp(dataType, isNullable);
            classLines.AppendLine($"\tpublic {csType} {propertyName} {{ get; set; }}");
        }

        // Close the last class
        if (currentTable != null)
        {
            classLines.AppendLine("}");
            sb.Append(classLines);
        }

        return sb.ToString();
    }

    static string MapPostgresToCSharp(string postgresType, bool isNullable)
    {
        var csType = postgresType.ToLower() switch
        {
            "integer" or "int4" => "int",
            "bigint" or "int8" => "long",
            "smallint" or "int2" => "short",
            "numeric" or "decimal" => "decimal",
            "real" or "float4" => "float",
            "double precision" or "float8" => "double",
            "boolean" or "bool" => "bool",
            "text" or "varchar" or "character varying" => "string",
            "char" or "character" => "string",
            "date" => "DateOnly",
            "timestamp" or "timestamp without time zone" => "DateTime",
            "timestamp with time zone" => "DateTimeOffset",
            "uuid" => "Guid",
            "json" or "jsonb" => "string",
            "bytea" => "byte[]",
            _ => "object"
        };

        if (isNullable && csType != "string" && csType != "byte[]")
        {
            csType += "?";
        }

        return csType;
    }
    
    static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var words = input.Split('_');
        var result = new StringBuilder();

        foreach (var word in words)
        {
            if (word.Length > 0)
            {
                result.Append(char.ToUpper(word[0]));
                if (word.Length > 1)
                    result.Append(word[1..].ToLower());
            }
        }

        return result.ToString();
    }

    static string RenderNamespace(string? @namespace)
    {
        if (!string.IsNullOrWhiteSpace(@namespace))
        {
            return $"namespace {@namespace};";
        }

        return string.Empty;
    }
}