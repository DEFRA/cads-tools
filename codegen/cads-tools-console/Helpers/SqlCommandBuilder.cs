using Npgsql;

namespace Cads.Tools.Console.Helpers;

internal class SqlCommandBuilder
{
    public async static Task<NpgsqlCommand> GetSchemaQueryCommand(NpgsqlConnection connection, string schema)
    {
        var sql = @"
                SELECT 
                    t.table_name,
                    c.column_name,
                    c.data_type,
                    c.is_nullable
                FROM information_schema.tables t
                JOIN information_schema.columns c ON t.table_name = c.table_name
                WHERE t.table_schema = @schema and c.table_schema = @schema
                ORDER BY t.table_name, c.ordinal_position";

        var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@schema", schema);
        return cmd;
    }

    public async static Task<NpgsqlCommand> GetSchemaWithPkQueryCommand(NpgsqlConnection connection, string schema)
    {
        var sql = @"
                SELECT 
                    t.table_name,
                    t.table_schema,
                    STRING_AGG(kcu.column_name, ', ' ORDER BY kcu.ordinal_position) AS primary_key
                FROM information_schema.tables t
                LEFT JOIN information_schema.table_constraints tc 
                    ON t.table_name = tc.table_name 
                    AND t.table_schema = tc.table_schema
                    AND tc.constraint_type = 'PRIMARY KEY'
                LEFT JOIN information_schema.key_column_usage kcu 
                    ON tc.constraint_name = kcu.constraint_name
                    AND kcu.table_schema = t.table_schema
                WHERE t.table_schema = @schema
                GROUP BY t.table_name, t.table_schema
                ORDER BY t.table_name";

        var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@schema", schema);
        return cmd;
    }
}
