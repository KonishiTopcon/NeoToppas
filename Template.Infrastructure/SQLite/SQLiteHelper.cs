using System.Data.SQLite;
using System.Reflection;

namespace Template.Infrastructure.SQLite
{
    internal static class SQLiteHelper
    {
        internal static string DbPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "data.db");
        internal static string ConnectionString => $"Data Source={DbPath};Version=3;";

        internal static IReadOnlyList<T> Query<T>(string sql, SQLiteParameter[] parameters, Func<SQLiteDataReader, T> createEntity)
        {
            var result = new List<T>();
            using (var connection = new SQLiteConnection(ConnectionString))
            using (var command = new SQLiteCommand(sql, connection))
            {
                connection.Open();

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(createEntity(reader));
                    }
                }
            }
            return result;
        }

        internal static T QuerySingle<T>(string sql, SQLiteParameter[] parameters, Func<SQLiteDataReader, T> createEntity, T nullEntity)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            using (var command = new SQLiteCommand(sql, connection))
            {
                connection.Open();

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return createEntity(reader);
                    }
                }
            }
            return nullEntity;
        }

        internal static void Execute(string sql, SQLiteParameter[] parameters)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            using (var command = new SQLiteCommand(sql, connection))
            {
                connection.Open();

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                command.ExecuteNonQuery();
            }
        }

        internal static void ExecuteTransaction(List<(string sql, SQLiteParameter[] parameters)> sqlCommands)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var (sql, parameters) in sqlCommands)
                        {
                            using (var command = new SQLiteCommand(sql, connection))
                            {
                                if (parameters != null)
                                {
                                    command.Parameters.AddRange(parameters);
                                }
                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        internal static bool IsExistTable(string tableName)
        {
            string sql = "SELECT COUNT(*) FROM sqlite_master WHERE TYPE='table' AND name=@tableName;";
            var parameters = new[] { new SQLiteParameter("@tableName", tableName) };

            int result = QuerySingle(sql, parameters, reader => Convert.ToInt32(reader["COUNT(*)"]), 0);
            return result > 0;
        }

        internal static string CamelToSnake(string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, "(?<=[a-z])([A-Z])", "_$1").ToLower();
        }

        internal static string MapTypeToSql(Type type)
        {
            return type.Name switch
            {
                "String" => "TEXT",
                "DateTime" => "TEXT",
                "Int32" => "INTEGER",
                "Single" => "REAL",
                "Double" => "REAL",
                _ => "TEXT",
            };
        }
    }
}
