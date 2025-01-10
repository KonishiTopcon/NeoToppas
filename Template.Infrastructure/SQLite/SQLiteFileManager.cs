using System.Data.SQLite;
using System.Reflection;
using Template.Domain.Entities;

namespace Template.Infrastructure.SQLite
{
    public class SQLiteFileManager
    {
        public static void CreateDatabaseFile()
        {
            if (!File.Exists(SQLiteHelper.DbPath))
            {
                SQLiteConnection.CreateFile(SQLiteHelper.DbPath);
            }
            //　テーブル作成
            CreateTable(typeof(UserEntity));
        }

        public static void DeleteDatabaseFile()
        {
            if (File.Exists(SQLiteHelper.DbPath))
            {
                File.Delete(SQLiteHelper.DbPath);
            }
        }

        private static void CreateTable(Type entity)
        {
            // テーブル名をエンティティ名から取得し、スネークケースに変換
            string tableName = SQLiteHelper.CamelToSnake(entity.Name.Replace("Entity", string.Empty));

            // 既にテーブルが存在する場合は作成せずにリターン
            if (SQLiteHelper.IsExistTable(tableName))
            {
                return;
            }

            var sqlColumns = new List<string>();

            // プライマリキーとして指定されたプロパティを探す
            var properties = entity.GetProperties();
            foreach (var prop in properties)
            {
                string columnDefinition = $"{SQLiteHelper.CamelToSnake(prop.Name)} {SQLiteHelper.MapTypeToSql(prop.PropertyType)}";

                // プライマリキー属性が付与されているか確認
                if (prop.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                {
                    columnDefinition += " PRIMARY KEY";
                }

                sqlColumns.Add(columnDefinition);
            }

            // SQL文の生成と実行
            string sql = $"CREATE TABLE '{tableName}' ({string.Join(", ", sqlColumns)});";
            SQLiteHelper.Execute(sql, null);
        }
    }
}