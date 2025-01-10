using System.Data.SQLite;
using System.Reflection;

namespace Template.Infrastructure.SQLite
{
    public abstract class SQLiteBase<T> where T : class
    {
        protected string TableName { get; }
        protected Func<T> EntityFactory { get; set; }

        protected SQLiteBase(Func<T> entityFactory)
        {
            TableName = SQLiteHelper.CamelToSnake(typeof(T).Name.Replace("Entity", string.Empty));
            EntityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory), "ファクトリメソッドは必須です。");
        }

        /// <summary>
        /// テーブルの内容を全て削除
        /// </summary>
        public void ClearTable()
        {
            var sql = $"DELETE FROM {TableName};";
            SQLiteHelper.Execute(sql, null);
        }
        
        /// <summary>
        /// 単一のデータを削除する
        /// </summary>
        /// <param name="keyPropertyName">主キーとなるプロパティ名</param>
        /// <param name="keyValue">削除するエンティティのキーの値</param>
        public void DeleteSingle(string keyPropertyName, object keyValue)
        {
            var keyProperty = typeof(T).GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (keyProperty == null)
            {
                throw new ArgumentException($"Property '{keyPropertyName}' does not exist on type '{typeof(T).Name}'");
            }

            string keyColumnName = SQLiteHelper.CamelToSnake(keyPropertyName);
            string sql = $"DELETE FROM {TableName} WHERE {keyColumnName} = @Key";

            var args = new List<SQLiteParameter>
        {
            new SQLiteParameter("@Key", keyValue ?? DBNull.Value)
        };

            SQLiteHelper.Execute(sql, args.ToArray());
        }

        /// <summary>
        /// 複数のデータを削除する
        /// </summary>
        /// <param name="keyPropertyName">主キーとなるプロパティ名</param>
        /// <param name="keyValues">削除するエンティティのキーの値のリスト</param>
        public void DeleteList(string keyPropertyName, List<object> keyValues)
        {
            var keyProperty = typeof(T).GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (keyProperty == null)
            {
                throw new ArgumentException($"Property '{keyPropertyName}' does not exist on type '{typeof(T).Name}'");
            }

            string keyColumnName = SQLiteHelper.CamelToSnake(keyPropertyName);
            string sql = $"DELETE FROM {TableName} WHERE {keyColumnName} = @Key";

            var sqlCommands = new List<(string sql, SQLiteParameter[] parameters)>();

            foreach (var keyValue in keyValues)
            {
                var args = new List<SQLiteParameter>
            {
                new SQLiteParameter("@Key", keyValue ?? DBNull.Value)
            };

                sqlCommands.Add((sql, args.ToArray()));
            }

            SQLiteHelper.ExecuteTransaction(sqlCommands);
        }

        /// <summary>
        /// テーブルの内容を全て取得
        /// </summary>
        /// <returns>テーブルの内容をリスト形式で返す</returns>
        public List<T> GetAllTableData()
        {
            var sql = $"SELECT * FROM {TableName}";
            return SQLiteHelper.Query(sql, null, reader =>
            {
                // T型のコンストラクタを取得（引数があるコンストラクタを前提とする）
                var constructor = typeof(T).GetConstructors().FirstOrDefault();
                if (constructor == null)
                {
                    throw new InvalidOperationException($"{typeof(T).Name} に有効なコンストラクタが存在しません。");
                }

                // コンストラクタのパラメータを取得し、それに合わせてリーダーから値を取得
                var parameters = constructor.GetParameters().Select(parameterInfo =>
                {
                    string columnName = SQLiteHelper.CamelToSnake(parameterInfo.Name);
                    return reader[columnName] != DBNull.Value
                        ? Convert.ChangeType(reader[columnName], parameterInfo.ParameterType)
                        : null;
                }).ToArray();

                // コンストラクタを呼び出してインスタンスを生成
                var entity = (T)constructor.Invoke(parameters);
                return entity;
            }).ToList();
        }


        /// <summary>
        /// 指定のプロパティで値で検索した値を取得
        /// </summary>
        /// <param name="propertyName">検索するプロパティの名前</param>
        /// <param name="value">検索する値</param>
        /// <returns>最初に一致したエンティティ、存在しない場合はnull</returns>
        public T GetDataSingle(string propertyName, object value)
        {
            return GetDataList(propertyName, value).FirstOrDefault();
        }

        /// <summary>
        /// 指定のプロパティで値で検索した値をリストで取得
        /// </summary>
        /// <param name="propertyName">検索するプロパティの名前</param>
        /// <param name="value">検索する値</param>
        /// <returns>検索結果のエンティティのリスト</returns>
        public List<T> GetDataList(string propertyName, object value)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                throw new ArgumentException($"Property '{propertyName}' does not exist on type '{typeof(T).Name}'");
            }

            // プロパティ名をスネークケースに変換
            string snakeCasePropertyName = SQLiteHelper.CamelToSnake(propertyName);

            var sql = $"SELECT * FROM {TableName} WHERE {snakeCasePropertyName} = @Value";

            var args = new List<SQLiteParameter>
    {
        new SQLiteParameter("@Value", value ?? DBNull.Value)
    };

            return SQLiteHelper.Query(sql, args.ToArray(), reader =>
            {
                // T型のコンストラクタを取得（引数があるコンストラクタを前提とする）
                var constructor = typeof(T).GetConstructors().FirstOrDefault();
                if (constructor == null)
                {
                    throw new InvalidOperationException($"{typeof(T).Name} に有効なコンストラクタが存在しません。");
                }

                // コンストラクタのパラメータを取得し、それに合わせてリーダーから値を取得
                var parameters = constructor.GetParameters().Select(parameterInfo =>
                {
                    string columnName = SQLiteHelper.CamelToSnake(parameterInfo.Name);
                    return reader[columnName] != DBNull.Value
                        ? Convert.ChangeType(reader[columnName], parameterInfo.ParameterType)
                        : null;
                }).ToArray();

                // コンストラクタを呼び出してインスタンスを生成
                var entity = (T)constructor.Invoke(parameters);
                return entity;
            }).ToList();
        }

        /// <summary>
        /// テーブルにデータを追加
        /// </summary>
        /// <param name="entity">追加するエンティティ</param>
        /// <param name="throwOnException">例外をスローするかどうか</param>
        public void SetDataSingle(T entity, bool throwOnException = false)
        {
            SetDataList(new List<T>() { entity }, throwOnException);
        }
        /// <summary>
        /// テーブルに複数データを追加
        /// </summary>
        /// <param name="entities">追加するエンティティのリスト</param>
        /// <param name="throwOnException">例外をスローするかどうか</param>
        public void SetDataList(List<T> entities, bool throwOnException = false)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string fieldNames = string.Join(", ", properties.Select(p => SQLiteHelper.CamelToSnake(p.Name)));
            string parameterNames = string.Join(", ", properties.Select(p => "@" + p.Name));

            string sql = $"INSERT INTO {TableName} ({fieldNames}) VALUES ({parameterNames})";

            var sqlCommands = new List<(string sql, SQLiteParameter[] parameters)>();

            foreach (var entity in entities)
            {
                var args = new List<SQLiteParameter>();

                foreach (var prop in properties)
                {
                    var value = prop.GetValue(entity);
                    args.Add(new SQLiteParameter("@" + prop.Name, value ?? DBNull.Value));
                }

                sqlCommands.Add((sql, args.ToArray()));
            }

            try
            {
                SQLiteHelper.ExecuteTransaction(sqlCommands);
            }
            catch (SQLiteException ex)
            {
                if (throwOnException)
                {
                    throw new InvalidOperationException("データ挿入中にエラーが発生しました。", ex);
                }
                else
                {
                    Console.WriteLine($"エラーが発生しました: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 単一のデータを更新する
        /// </summary>
        /// <param name="entity">更新するエンティティ</param>
        /// <param name="keyPropertyName">主キーとなるプロパティ名</param>
        public void UpdateSingle(string keyPropertyName, T entity)
        {
            UpdateList(keyPropertyName, new List<T> { entity });
        }

        /// <summary>
        /// 複数のデータを更新する
        /// </summary>
        /// <param name="entities">更新するエンティティのリスト</param>
        /// <param name="keyPropertyName">主キーとなるプロパティ名</param>
        public void UpdateList(string keyPropertyName, List<T> entities)
        {
            var keyProperty = typeof(T).GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (keyProperty == null)
            {
                throw new ArgumentException($"Property '{keyPropertyName}' does not exist on type '{typeof(T).Name}'");
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.Name != keyPropertyName).ToList();
            string setClause = string.Join(", ", properties.Select(p => $"{SQLiteHelper.CamelToSnake(p.Name)} = @{p.Name}"));
            string keyColumnName = SQLiteHelper.CamelToSnake(keyPropertyName);

            string sql = $"UPDATE {TableName} SET {setClause} WHERE {keyColumnName} = @Key";

            var sqlCommands = new List<(string sql, SQLiteParameter[] parameters)>();

            foreach (var entity in entities)
            {
                var args = new List<SQLiteParameter>();

                foreach (var prop in properties)
                {
                    var value = prop.GetValue(entity);
                    args.Add(new SQLiteParameter("@" + prop.Name, value ?? DBNull.Value));
                }

                var key = keyProperty.GetValue(entity);
                args.Add(new SQLiteParameter("@Key", key ?? DBNull.Value));

                sqlCommands.Add((sql, args.ToArray()));
            }

            SQLiteHelper.ExecuteTransaction(sqlCommands);
        }
    }
}