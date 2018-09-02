using ServiceStack.OrmLite;
using System;
using System.Data;
using System.Reflection;
using ServiceStack;
using ServiceStack.OrmLite.Oracle;
using Startest.DataAccess.Properties;

/// <summary>
/// 
/// </summary>
namespace Startest.DataAccess
{
    public class ConnectionFactory
    {
        OrmLiteConnectionFactory factory = null;
        public ConnectionFactory()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dbType">Type of the database.</param>
        public void SetData(string connectionString, enumDbType dbType)
        {
            if (dbType == enumDbType.PostgreSQL)
                factory = new OrmLiteConnectionFactory(connectionString, PostgreSqlDialect.Provider);
            if (dbType == enumDbType.Oracle)
                factory = new OrmLiteConnectionFactory(connectionString, OracleDialect.Provider);
            if (dbType == enumDbType.MySql)
                factory = new OrmLiteConnectionFactory(connectionString, MySqlDialect.Provider);
            if (dbType == enumDbType.Sqlite)
                factory = new OrmLiteConnectionFactory(connectionString, SqliteDialect.Provider);
            if (dbType == enumDbType.SqlServer)
                factory = new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);
        }
        
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
           
            if (args.Name.Contains("System.Data.SQLite"))
            {
                string file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System.Data.SQLite.dll");
                if (!System.IO.File.Exists(file))
                {
                    System.IO.File.WriteAllBytes(file,Resources.System_Data_SQLite);
                } 
                return Assembly.LoadFile(file);
            }
            if (args.Name.Contains("Mono.Security"))
            {
                return Assembly.Load(Resources.Mono_Security);
            }
            if (args.Name.Contains("Npgsql"))
            {
                return Assembly.Load(Resources.Npgsql);
            }
            if (args.Name.Contains("DDTek.Oracle"))
            {
                return Assembly.Load(Resources.DDTek_Oracle);
            }
            if (args.Name.Contains("MySql.Data"))
            {
                return Assembly.Load(Resources.MySql_Data);
            }
            if (args.Name.Contains("System.Memory"))
            {
                return Assembly.Load(Resources.System_Memory);
            }
            throw new DllNotFoundException(args.Name);
        }
        
        public DbConnection CreateDbConnection()
        {
            System.Data.IDbConnection conn = factory.OpenDbConnection();
            return new DbConnection(conn);
        }
    }
}
