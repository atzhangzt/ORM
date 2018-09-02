using System.Data;
using System.Data.SQLite;
//using Mono.Data.Sqlite;
namespace ServiceStack.OrmLite.Sqlite
{
    public class SqliteOrmLiteDialectProvider : SqliteOrmLiteDialectProviderBase
    {
        public static SqliteOrmLiteDialectProvider Instance = new SqliteOrmLiteDialectProvider();

        protected override IDbConnection CreateConnection(string connectionString)
        {
            //return new SqliteConnection(connectionString);
            return new SQLiteConnection(connectionString);
        }

        public override IDbDataParameter CreateParam()
        {
            return new SQLiteParameter(); 
            //return new SqliteParameter();
        }
    }
}