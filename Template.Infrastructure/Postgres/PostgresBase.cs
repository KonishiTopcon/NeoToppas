using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Npgsql;

namespace NeoToppas.Infrastructure.Postgres
{
    public static class PostgresBase
    {
        
        private static string conn_str = "Server=10.192.139.9; Port=5432; User Id=konishi; Password=konishi; Database=NeoToppas;Enlist=true";

        /// <summary>
        /// postgresに引数のsqlを投げて、データテーブルを返す
        /// </summary>
        /// <param name="sql">SELECTを含むSQL命令</param>
        /// <returns></returns>
        public static DataTable GetDataTable(String sql)
        {
                //TransactionScopeの利用
                using (TransactionScope ts = new TransactionScope())
                {
                    using (NpgsqlConnection conn2 = new NpgsqlConnection(conn_str))
                    {
                        NpgsqlCommand cmd = null;
                        string cmd_str = null;
                        NpgsqlDataAdapter da = null;

                        //PostgreSQLへ接続後、SELECT結果を取得
                        conn2.Open();

                        //SELECT処理
                        cmd = new NpgsqlCommand(sql, conn2);
                        da = new NpgsqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        ts.Complete();//トランザクションのコミット
                        return dt;
                    }
                }
        }

        public static int InsertDataTable(String sql)
        {
            //TransactionScopeの利用
            using (TransactionScope ts = new TransactionScope())
            {
                using (NpgsqlConnection con = new NpgsqlConnection(conn_str))
                {
                    string cmd_str = null;
                    NpgsqlDataAdapter da = null;

                    //PostgreSQLへ接続後、SELECT結果を取得
                    con.Open();
                    using var cmd = new NpgsqlCommand(sql, con);
                    int result = cmd.ExecuteNonQuery();
                    ts.Complete();//トランザクションのコミット
                    return result;
                }
            }
        }
    }
}
