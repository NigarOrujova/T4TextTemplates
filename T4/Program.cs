using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;
using T4.AppCode.Infrastructure;

public class Program
{
    public static void Main()
    {
        string connection = "Server=DESKTOP-KLFF7EB;Database=NORTHWND;Integrated security=sspi;";
        string folderPath = "C:\\Users\\Nigar\\Desktop\\Ufo\\T4\\T4\\Models\\";

        List<Table> tables = GetTables(connection);
        foreach (Table table in tables)
        {
            table.Columns = GetColumns(connection, table.Name);
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }



        foreach (Table table in tables)
        {
            string filePath = Path.Combine(folderPath, table.Name.Replace(" ", "") + ".cs");
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("namespace T4.Models");
                writer.WriteLine("{");
                writer.WriteLine("    public class " + table.Name.Replace(" ", ""));
                writer.WriteLine("    {");

                foreach (Column column in table.Columns)
                {
                    writer.WriteLine("          " + column.Name);
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }
    }
        private static List<Table> GetTables(string connection)
        {
            List<Table> tables = new List<Table>();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            tables.Add(new Table
                            {
                                Name = reader.GetString(0)
                            }) ;
                        }
                    }
                }
            }
            return tables;
        }
       private static List<Column> GetColumns(string connection, string tableName)
        {
            List<Column> columns = new List<Column>();

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT CONCAT(\r\n                 " +
                    "'public ',\r\n                 " +
                    "IIF(DATA_TYPE = 'nvarchar', 'string' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'uniqueidentifier', 'string' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'ntext', 'string' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'nchar', 'string' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'int', 'int' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'smallint', 'short' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'tinyint', 'byte' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'money', 'decimal' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'float', 'float' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'decimal', 'decimal' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'bit', 'bool' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'datetime', 'DateTime' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'date', 'DateTime' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'real', 'float' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'varbinary', 'byte[]' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'datetime2', 'DateTime' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 " +
                    "IIF(DATA_TYPE = 'image', 'byte[]' + (IIF(IS_NULLABLE = 'YES', '?', '')), ''),\r\n                 ' ',\r\n                 " +
                    "COLUMN_NAME,\r\n                 ' { get; set; }'\r\n                 + " +
                    "(IIF((DATA_TYPE = 'nvarchar' or DATA_TYPE = 'nchar') and (IS_NULLABLE = 'NO'), ' = null!;', ''))\r\n             )\r\n" +
                    "FROM INFORMATION_SCHEMA.COLUMNS\r\nWHERE TABLE_NAME = @tableName\r\n      and TABLE_SCHEMA = 'dbo'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@tableName", tableName);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(new Column
                            {
                                Name = reader.GetString(0)
                            });
                        }
                    }
                }
            }
            return columns;
        }
}