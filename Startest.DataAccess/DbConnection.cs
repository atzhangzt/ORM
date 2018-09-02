using ServiceStack.OrmLite;
using ServiceStack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System.Data;

namespace Startest.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DbConnection : MarshalByRefObject, IDisposable
    {
        private System.Data.IDbConnection dbConn;
        public System.Data.IDbConnection Connection
        {
            
            get
            {
                OrmLiteConnection orm = dbConn as OrmLiteConnection;
                return orm.DbConnection; 
            }
        }
        public string ConnectionString
        {
            get
            {
                return dbConn.ConnectionString;
            }
            set
            {
                dbConn.ConnectionString = value;
            }
        }
        public int ConnectionTimeout
        {
            get
            {
                return dbConn.ConnectionTimeout;
            }
        }
        public string Database
        {
            get
            {
                return dbConn.Database;
            }
        }
        public ConnectionState State
        {
            get
            {
                return dbConn.State;
            }
        }
        internal DbConnection(System.Data.IDbConnection conn)
        {
            dbConn = conn;           
        }
        public void Dispose()
        {
            dbConn.Close();
            dbConn.Dispose();
        }
        public void Close()
        {
            dbConn.Close();
        }
        public void Open()
        {
            dbConn.Open();
        }

        #region OrmLiteReadExpressionsApi
        public T Exec<T>(Func<IDbCommand, T> filter)
        {
            return dbConn.GetExecFilter().Exec(dbConn, filter);
        }

        public void Exec(Action<IDbCommand> filter)
        {
            dbConn.GetExecFilter().Exec(dbConn, filter);
        }

        public Task<T> Exec<T>(Func<IDbCommand, Task<T>> filter)
        {
            return dbConn.GetExecFilter().Exec(dbConn, filter);
        }

        public Task Exec(Func<IDbCommand, Task> filter)
        {
            return dbConn.GetExecFilter().Exec(dbConn, filter);
        }

        public IEnumerable<T> ExecLazy<T>(Func<IDbCommand, IEnumerable<T>> filter)
        {
            return dbConn.GetExecFilter().ExecLazy(dbConn, filter);
        }

        public IDbCommand Exec(Func<IDbCommand, IDbCommand> filter)
        {
            return dbConn.GetExecFilter().Exec(dbConn, filter);
        }

        public Task<IDbCommand> Exec(Func<IDbCommand, Task<IDbCommand>> filter)
        {
            return dbConn.GetExecFilter().Exec(dbConn, filter);
        }


        /// <summary>
        /// Create a new SqlExpression builder allowing typed LINQ-like queries.
        /// </summary>
        [Obsolete("Use From<T>")]
        public SqlExpression<T> SqlExpression<T>()
        {
            return dbConn.GetExecFilter().SqlExpression<T>(dbConn);
        }

        /// <summary>
        /// Creates a new SqlExpression builder allowing typed LINQ-like queries.
        /// Alias for SqlExpression.
        /// </summary>
        public SqlExpression<T> From<T>()
        {
            return dbConn.GetExecFilter().SqlExpression<T>(dbConn);
        }

        public SqlExpression<T> From<T, JoinWith>(Expression<Func<T, JoinWith, bool>> joinExpr = null)
        {
            var sql = dbConn.GetExecFilter().SqlExpression<T>(dbConn);
            sql.Join<T, JoinWith>(joinExpr);
            return sql;
        }

        /// <summary>
        /// Creates a new SqlExpression builder for the specified type using a user-defined FROM sql expression.
        /// </summary>
        public SqlExpression<T> From<T>(string fromExpression)
        {
            var expr = dbConn.GetExecFilter().SqlExpression<T>(dbConn);
            expr.From(fromExpression);
            return expr;
        }

        /// <summary>
        /// Open a Transaction in OrmLite
        /// </summary>
        public IDbTransaction OpenTransaction()
        {
            return new OrmLiteTransaction(dbConn, dbConn.BeginTransaction());
        }

        /// <summary>
        /// Open a Transaction in OrmLite
        /// </summary>
        public IDbTransaction OpenTransaction(IsolationLevel isolationLevel)
        {
            return new OrmLiteTransaction(dbConn, dbConn.BeginTransaction(isolationLevel));
        }

        /// <summary>
        /// Create a managed OrmLite IDbCommand
        /// </summary>
        public IDbCommand OpenCommand()
        {
            return dbConn.GetExecFilter().CreateCommand(dbConn);
        }

        /// <summary>
        /// Returns results from using a LINQ Expression. E.g:
        /// <para>db.Select&lt;Person&gt;(x =&gt; x.Age &gt; 40)</para>
        /// </summary>
        public List<T> Select<T>(Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select(predicate));
        }
        /// <summary>
        /// Returns results from using a LINQ Expression. E.g:
        /// <para>db.SelectTable&lt;Person&gt;(x =&gt; x.Age &gt; 40)</para>
        /// </summary>
        public DataTable SelectTable<T>(Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectTable(predicate));
        }
        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Select&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public List<T> Select<T>(Func<SqlExpression<T>, SqlExpression<T>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select(expression));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.SelectTable(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public DataTable SelectTable<T>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectTable(expression));
        }
        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Select(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public List<T> Select<T>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select(expression));
        }


        /// <summary>
        /// Project results from a number of joined tables into a different model
        /// </summary>
        public List<Into> Select<Into, From>(SqlExpression<From> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<Into, From>(expression));
        }

        /// <summary>
        /// Project results from a number of joined tables into a different model
        /// </summary>
        public List<Into> Select<Into, From>(Func<SqlExpression<From>, SqlExpression<From>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<Into, From>(expression));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Select(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public List<T> Select<T>(ISqlExpression expression, object anonType = null)
        {
            if (anonType != null)
                return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(expression.SelectInto<T>(), anonType));

            if (expression.Params != null && expression.Params.Any())
                return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(expression.SelectInto<T>(), expression.Params.ToDictionary(param => param.ParameterName, param => param.Value)));

            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(expression.SelectInto<T>(), expression.Params));
        }

        /// <summary>
        /// Returns a single result from using a LINQ Expression. E.g:
        /// <para>db.Single&lt;Person&gt;(x =&gt; x.Age == 42)</para>
        /// </summary>
        public T Single<T>(Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single(predicate));
        }

        /// <summary>
        /// Returns a single result from using an SqlExpression lambda. E.g:
        /// <para>db.Single&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age == 42))</para>
        /// </summary>
        public T Single<T>(Func<SqlExpression<T>, SqlExpression<T>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single(expression));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Select&lt;Person&gt;(x =&gt; x.Age &gt; 40)</para>
        /// </summary>
        public T Single<T>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single(expression));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Single(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public T Single<T>(ISqlExpression expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single<T>(expression.SelectInto<T>(), expression.Params));
        }

        /// <summary>
        /// Returns a scalar result from using an SqlExpression lambda. E.g:
        /// <para>db.Scalar&lt;Person, int&gt;(x =&gt; Sql.Max(x.Age))</para>
        /// </summary>
        public TKey Scalar<T, TKey>(Expression<Func<T, TKey>> field)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar(field));
        }

        /// <summary>
        /// Returns a scalar result from using an SqlExpression lambda. E.g:
        /// <para>db.Scalar&lt;Person, int&gt;(x =&gt; Sql.Max(x.Age), , x =&gt; x.Age &lt; 50)</para>
        /// </summary>        
        public TKey Scalar<T, TKey>(Expression<Func<T, TKey>> field, Expression<Func<T, bool>> predicate)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar(field, predicate));
        }

        /// <summary>
        /// Returns the count of rows that match the LINQ expression, E.g:
        /// <para>db.Count&lt;Person&gt;(x =&gt; x.Age &lt; 50)</para>
        /// </summary>
        public long Count<T>(Expression<Func<T, bool>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Count(expression));
        }

        /// <summary>
        /// Returns the count of rows that match the SqlExpression lambda, E.g:
        /// <para>db.Count&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        public long Count<T>(Func<SqlExpression<T>, SqlExpression<T>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Count(expression));
        }

        /// <summary>
        /// Returns the count of rows that match the supplied SqlExpression, E.g:
        /// <para>db.Count(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        public long Count<T>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Count(expression));
        }

        public long Count<T>()
        {
            var expression = dbConn.GetDialectProvider().SqlExpression<T>();
            return dbConn.Exec(dbCmd => dbCmd.Count(expression));
        }

        /// <summary>
        /// Return the number of rows returned by the supplied expression
        /// </summary>
        public long RowCount<T>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.RowCount(expression));
        }

        /// <summary>
        /// Return the number of rows returned by the supplied sql
        /// </summary>
        public long RowCount(string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.RowCount(sql));
        }

        /// <summary>
        /// Returns results with references from using a LINQ Expression. E.g:
        /// <para>db.LoadSelect&lt;Person&gt;(x =&gt; x.Age &gt; 40)</para>
        /// </summary>
        public List<T> LoadSelect<T>(Expression<Func<T, bool>> predicate, string[] include = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(predicate, include));
        }

        /// <summary>
        /// Returns results with references from using a LINQ Expression. E.g:
        /// <para>db.LoadSelect&lt;Person&gt;(x =&gt; x.Age &gt; 40, include: x => new { x.PrimaryAddress })</para>
        /// </summary>
        public List<T> LoadSelect<T>(Expression<Func<T, bool>> predicate, Func<T, object> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(predicate, include(typeof(T).CreateInstance<T>()).GetType().AllAnonFields()));
        }

        /// <summary>
        /// Returns results with references from using an SqlExpression lambda. E.g:
        /// <para>db.LoadSelect&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public List<T> LoadSelect<T>(Func<SqlExpression<T>, SqlExpression<T>> expression, string[] include = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(expression, include));
        }

        /// <summary>
        /// Returns results with references from using an SqlExpression lambda. E.g:
        /// <para>db.LoadSelect&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &gt; 40), include: x => new { x.PrimaryAddress })</para>
        /// </summary>
        public List<T> LoadSelect<T>(Func<SqlExpression<T>, SqlExpression<T>> expression, Func<T, object> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(expression, include(typeof(T).CreateInstance<T>()).GetType().AllAnonFields()));
        }

        /// <summary>
        /// Returns results with references from using an SqlExpression lambda. E.g:
        /// <para>db.LoadSelect(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public List<T> LoadSelect<T>(SqlExpression<T> expression = null, string[] include = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(expression, include));
        }

        /// <summary>
        /// Returns results with references from using an SqlExpression lambda. E.g:
        /// <para>db.LoadSelect(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40), include: x => new { x.PrimaryAddress })</para>
        /// </summary>
        public List<T> LoadSelect<T>(SqlExpression<T> expression, Func<T, object> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect(expression, include(typeof(T).CreateInstance<T>()).GetType().AllAnonFields()));
        }

        /// <summary>
        /// Project results with references from a number of joined tables into a different model
        /// </summary>
        public List<Into> LoadSelect<Into, From>(SqlExpression<From> expression, string[] include = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect<Into, From>(expression, include));
        }

        /// <summary>
        /// Project results with references from a number of joined tables into a different model
        /// </summary>
        public List<Into> LoadSelect<Into, From>(SqlExpression<From> expression, Func<Into, object> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelect<Into, From>(expression, include(typeof(Into).CreateInstance<Into>()).GetType().AllAnonFields()));
        }
        #endregion

        #region OrmLiteReadApi
        /// <summary>
        /// Returns results from the active connection.
        /// </summary>
        public List<T> Select<T>()
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>());
        }

        /// <summary>
        /// Returns results from using sql. E.g:
        /// <para>db.Select&lt;Person&gt;("Age &gt; 40")</para>
        /// <para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; 40")</para>
        /// </summary>
        public List<T> Select<T>(string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>(sql));
        }

        public DataTable Select(string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select(sql));
        }
        /// <summary>
        /// 获取服务器当前时间
        /// <para>1、Oracle已经验证</para>
        /// <para>2、PostgreSQL已经验证</para>
        /// <para>3、Sqlite已经验证</para>
        /// </summary>
        /// <param name="dbConn"></param>
        /// <returns>服务器上的当前时间</returns>
        public DateTime ServerTime()
        {
            OrmLiteConnection conn = dbConn as OrmLiteConnection;

            switch (conn.DialectProvider.GetType().Name)
            {
                case "OracleOrmLiteDialectProvider":
                    return conn.Scalar<DateTime>("select sysdate from dual");
                case "PostgreSQLDialectProvider":
                    return conn.Scalar<DateTime>("select now()");
                case "SqliteOrmLiteDialectProvider":
                    return conn.Scalar<DateTime>("select datetime(current_timestamp,'localtime')");
                case "SqlServerOrmLiteDialectProvider":
                    return conn.Scalar<DateTime>("select getdate()");
                case "MySqlDialectProvider":
                    return conn.Scalar<DateTime>("select now()");
            }
            return DateTime.Now;
        }
        /// <summary>
        /// Returns results from using sql. E.g:
        /// <para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new[] { db.CreateParam("age", 40) })</para>
        /// </summary>
        public List<T> Select<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns results from using a parameterized query. E.g:
        /// <para>db.Select&lt;Person&gt;("Age &gt; @age", new { age = 40})</para>
        /// <para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new { age = 40})</para>
        /// </summary>
        public List<T> Select<T>(string sql, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>(sql, anonType));
        }

        /// <summary>
        /// Returns results from using a parameterized query. E.g:
        /// <para>db.Select&lt;Person&gt;("Age &gt; @age", new Dictionary&lt;string, object&gt; { { "age", 40 } })</para>
        /// <para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new Dictionary&lt;string, object&gt; { { "age", 40 } })</para>
        /// </summary>
        public List<T> Select<T>(string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<T>(sql, dict));
        }

        /// <summary>
        /// Returns results from using an SqlFormat query. E.g:
        /// <para>db.SelectFmt&lt;Person&gt;("Age &gt; {0}", 40)</para>
        /// <para>db.SelectFmt&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; {0}", 40)</para>
        /// </summary>
        public List<T> SelectFmt<T>(string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectFmt<T>(sqlFormat, filterParams));
        }

        /// <summary>
        /// Returns a partial subset of results from the specified tableType. E.g:
        /// <para>db.Select&lt;EntityWithId&gt;(typeof(Person))</para>
        /// <para></para>
        /// </summary>
        public List<TModel> Select<TModel>(Type fromTableType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Select<TModel>(fromTableType));
        }

        /// <summary>
        /// Returns a partial subset of results from the specified tableType using a SqlFormat query. E.g:
        /// <para>db.SelectFmt&lt;EntityWithId&gt;(typeof(Person), "Age &gt; {0}", 40)</para>
        /// </summary>
        public List<TModel> SelectFmt<TModel>(Type fromTableType, string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectFmt<TModel>(fromTableType, sqlFormat, filterParams));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.Where&lt;Person&gt;("Age", 27)</para>
        /// </summary>
        public List<T> Where<T>(string name, object value)
        {
            return dbConn.Exec(dbCmd => dbCmd.Where<T>(name, value));
        }

        /// <summary>
        /// Returns results from using an anonymous type filter. E.g:
        /// <para>db.Where&lt;Person&gt;(new { Age = 27 })</para>
        /// </summary>
        public List<T> Where<T>(object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Where<T>(anonType));
        }

        /// <summary>
        /// Returns results using the supplied primary key ids. E.g:
        /// <para>db.SelectByIds&lt;Person&gt;(new[] { 1, 2, 3 })</para>
        /// </summary>
        public List<T> SelectByIds<T>(IEnumerable idValues)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectByIds<T>(idValues));
        }

        /// <summary>
        /// Query results using the non-default values in the supplied partially populated POCO example. E.g:
        /// <para>db.SelectNonDefaults(new Person { Id = 1 })</para>
        /// </summary>
        public List<T> SelectNonDefaults<T>(T filter)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectNonDefaults<T>(filter));
        }

        /// <summary>
        /// Query results using the non-default values in the supplied partially populated POCO example. E.g:
        /// <para>db.SelectNonDefaults("Age &gt; @Age", new Person { Age = 42 })</para>
        /// </summary>
        public List<T> SelectNonDefaults<T>(string sql, T filter)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectNonDefaults<T>(sql, filter));
        }

        /// <summary>
        /// Returns a lazyily loaded stream of results. E.g:
        /// <para>db.SelectLazy&lt;Person&gt;()</para>
        /// </summary>
        public IEnumerable<T> SelectLazy<T>()
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.SelectLazy<T>());
        }

        /// <summary>
        /// Returns a lazyily loaded stream of results using a parameterized query. E.g:
        /// <para>db.SelectLazy&lt;Person&gt;("Age &gt; @age", new { age = 40 })</para>
        /// </summary>
        public IEnumerable<T> SelectLazy<T>(string sql, object anonType = null)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.SelectLazy<T>(sql, anonType));
        }

        /// <summary>
        /// Returns a lazyily loaded stream of results using a parameterized query. E.g:
        /// <para>db.SelectLazy(db.From&lt;Person&gt;().Where(x =&gt; x == 40))</para>
        /// </summary>
        public IEnumerable<T> SelectLazy<T>(SqlExpression<T> expression)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.SelectLazy<T>(expression.ToSelectStatement(), expression.Params));
        }

        /// <summary>
        /// Returns a lazyily loaded stream of results using an SqlFilter query. E.g:
        /// <para>db.SelectLazyFmt&lt;Person&gt;("Age &gt; {0}", 40)</para>
        /// </summary>
        public IEnumerable<T> SelectLazyFmt<T>(string sqlFormat, params object[] filterParams)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.SelectLazyFmt<T>(sqlFormat, filterParams));
        }

        /// <summary>
        /// Returns a stream of results that are lazily loaded using a parameterized query. E.g:
        /// <para>db.WhereLazy&lt;Person&gt;(new { Age = 27 })</para>
        /// </summary>
        public IEnumerable<T> WhereLazy<T>(object anonType)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.WhereLazy<T>(anonType));
        }

        /// <summary>
        /// Returns the first result using a parameterized query. E.g:
        /// <para>db.Single&lt;Person&gt;(new { Age = 42 })</para>
        /// </summary>
        public T Single<T>(object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single<T>(anonType));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.Single&lt;Person&gt;("Age = @age", new[] { db.CreateParam("age",40) })</para>
        /// </summary>
        public T Single<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.Single&lt;Person&gt;("Age = @age", new { age = 42 })</para>
        /// </summary>
        public T Single<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Single<T>(sql, anonType));
        }

        /// <summary>
        /// Returns the first result using a SqlFormat query. E.g:
        /// <para>db.SingleFmt&lt;Person&gt;("Age = {0}", 42)</para>
        /// </summary>
        public T SingleFmt<T>(string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleFmt<T>(sqlFormat, filterParams));
        }

        /// <summary>
        /// Returns the first result using a primary key id. E.g:
        /// <para>db.SingleById&lt;Person&gt;(1)</para>
        /// </summary>
        public T SingleById<T>(object idValue)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleById<T>(idValue));
        }

        /// <summary>
        /// Returns the first result using a name, value filter. E.g:
        /// <para>db.SingleWhere&lt;Person&gt;("Age", 42)</para>
        /// </summary>
        public T SingleWhere<T>(string name, object value)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleWhere<T>(name, value));
        }

        /// <summary>
        /// Returns a single scalar value using an SqlExpression. E.g:
        /// <para>db.Column&lt;int&gt;(db.From&lt;Persion&gt;().Select(x => Sql.Count("*")).Where(q => q.Age > 40))</para>
        /// </summary>
        public T Scalar<T>(ISqlExpression sqlExpression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar<T>(sqlExpression.ToSelectStatement(), sqlExpression.Params));
        }

        /// <summary>
        /// Returns a single scalar value using a parameterized query. E.g:
        /// <para>db.Scalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &gt; @age", new[] { db.CreateParam("age",40) })</para>
        /// </summary>
        public T Scalar<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns a single scalar value using a parameterized query. E.g:
        /// <para>db.Scalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &gt; @age", new { age = 40 })</para>
        /// </summary>
        public T Scalar<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar<T>(sql, anonType));
        }

        /// <summary>
        /// Returns a single scalar value using an SqlFormat query. E.g:
        /// <para>db.ScalarFmt&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &gt; {0}", 40)</para>
        /// </summary>
        public T ScalarFmt<T>(string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarFmt<T>(sqlFormat, sqlParams));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlExpression. E.g:
        /// <para>db.Column&lt;int&gt;(db.From&lt;Persion&gt;().Select(x => x.LastName).Where(q => q.Age == 27))</para>
        /// </summary>
        public List<T> Column<T>(ISqlExpression query)
        {
            return dbConn.Exec(dbCmd => dbCmd.Column<T>(query.ToSelectStatement(), query.Params));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.Column&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new[] { db.CreateParam("age",27) })</para>
        /// </summary>
        public List<T> Column<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Column<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlExpression. E.g:
        /// <para>db.ColumnLazy&lt;int&gt;(db.From&lt;Persion&gt;().Select(x => x.LastName).Where(q => q.Age == 27))</para>
        /// </summary>
        public IEnumerable<T> ColumnLazy<T>(ISqlExpression query)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.ColumnLazy<T>(query.ToSelectStatement(), query.Params));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.ColumnLazy&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new[] { db.CreateParam("age",27) })</para>
        /// </summary>
        public IEnumerable<T> ColumnLazy<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.ColumnLazy<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.ColumnLazy&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new { age = 27 })</para>
        /// </summary>
        public IEnumerable<T> ColumnLazy<T>(string sql, object anonType = null)
        {
            return dbConn.ExecLazy(dbCmd => dbCmd.ColumnLazy<T>(sql, anonType));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.Column&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new { age = 27 })</para>
        /// </summary>
        public List<T> Column<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Column<T>(sql, anonType));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.ColumnFmt&lt;string&gt;("SELECT LastName FROM Person WHERE Age = {0}", 27)</para>
        /// </summary>
        public List<T> ColumnFmt<T>(string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnFmt<T>(sqlFormat, sqlParams));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlExpression. E.g:
        /// <para>db.ColumnDistinct&lt;int&gt;(db.From&lt;Persion&gt;().Select(x => x.Age).Where(q => q.Age < 50))</para>
        /// </summary>
        public HashSet<T> ColumnDistinct<T>(ISqlExpression query)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinct<T>(query));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlFormat query. E.g:
        /// <para>db.ColumnDistinct&lt;int&gt;("SELECT Age FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        public HashSet<T> ColumnDistinct<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinct<T>(sql, anonType));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlFormat query. E.g:
        /// <para>db.ColumnDistinct&lt;int&gt;("SELECT Age FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        public HashSet<T> ColumnDistinct<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinct<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlFormat query. E.g:
        /// <para>db.ColumnDistinctFmt&lt;int&gt;("SELECT Age FROM Person WHERE Age &lt; {0}", 50)</para>
        /// </summary>
        public HashSet<T> ColumnDistinctFmt<T>(string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinctFmt<T>(sqlFormat, sqlParams));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an Sql Expression. E.g:
        /// <para>db.Lookup&lt;int, string&gt;(db.From&lt;Person&gt;().Select(x => new { x.Age, x.LastName }).Where(q => q.Age < 50))</para>
        /// </summary>
        public Dictionary<K, List<V>> Lookup<K, V>(ISqlExpression sqlExpression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Lookup<K, V>(sqlExpression.ToSelectStatement(), sqlExpression.Params));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an parameterized query. E.g:
        /// <para>db.Lookup&lt;int, string&gt;("SELECT Age, LastName FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        public Dictionary<K, List<V>> Lookup<K, V>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.Lookup<K, V>(sql, sqlParams));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an parameterized query. E.g:
        /// <para>db.Lookup&lt;int, string&gt;("SELECT Age, LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        public Dictionary<K, List<V>> Lookup<K, V>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Lookup<K, V>(sql, anonType));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an SqlFormat query. E.g:
        /// <para>db.LookupFmt&lt;int, string&gt;("SELECT Age, LastName FROM Person WHERE Age &lt; {0}", 50)</para>
        /// </summary>
        public Dictionary<K, List<V>> LookupFmt<K, V>(string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.LookupFmt<K, V>(sqlFormat, sqlParams));
        }

        /// <summary>
        /// Returns a Dictionary from the first 2 columns: Column 1 (Keys), Column 2 (Values) using an SqlExpression. E.g:
        /// <para>db.Dictionary&lt;int, string&gt;(db.From&lt;Person&gt;().Select(x => new { x.Id, x.LastName }).Where(x => x.Age < 50))</para>
        /// </summary>
        public Dictionary<K, V> Dictionary<K, V>(ISqlExpression query)
        {
            return dbConn.Exec(dbCmd => dbCmd.Dictionary<K, V>(query));
        }

        /// <summary>
        /// Returns a Dictionary from the first 2 columns: Column 1 (Keys), Column 2 (Values) using sql. E.g:
        /// <para>db.Dictionary&lt;int, string&gt;("SELECT Id, LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        public Dictionary<K, V> Dictionary<K, V>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Dictionary<K, V>(sql, anonType));
        }

        /// <summary>
        /// Returns a Dictionary from the first 2 columns: Column 1 (Keys), Column 2 (Values) using an SqlFormat query. E.g:
        /// <para>db.DictionaryFmt&lt;int, string&gt;("SELECT Id, LastName FROM Person WHERE Age &lt; {0}", 50)</para>
        /// </summary>
        public Dictionary<K, V> DictionaryFmt<K, V>(string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DictionaryFmt<K, V>(sqlFormat, sqlParams));
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the LINQ expression, E.g:
        /// <para>db.Exists&lt;Person&gt;(x =&gt; x.Age &lt; 50)</para>
        /// </summary>
        public bool Exists<T>(Expression<Func<T, bool>> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar(dbConn.From<T>().Where(expression).Limit(1).Select("'exists'"))) != null;
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the SqlExpression lambda, E.g:
        /// <para>db.Exists&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        public bool Exists<T>(Func<SqlExpression<T>, SqlExpression<T>> expression)
        {
            return dbConn.Exec(dbCmd =>
            {
                var q = dbCmd.GetDialectProvider().SqlExpression<T>();
                return dbCmd.Scalar(expression(q).Limit(1).Select("'exists'")) != null;
            });
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
        /// <para>db.Exists(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        public bool Exists<T>(SqlExpression<T> expression)
        {
            return dbConn.Exec(dbCmd => dbCmd.Scalar(expression.Limit(1).Select("'exists'"))) != null;
        }
        /// <summary>
        /// Returns true if the Query returns any records, using an SqlFormat query. E.g:
        /// <para>db.Exists&lt;Person&gt;(new { Age = 42 })</para>
        /// </summary>
        public bool Exists<T>(object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.Exists<T>(anonType));
        }

        /// <summary>
        /// Returns true if the Query returns any records, using a parameterized query. E.g:
        /// <para>db.Exists&lt;Person&gt;("Age = @age", new { age = 42 })</para>
        /// <para>db.Exists&lt;Person&gt;("SELECT * FROM Person WHERE Age = @age", new { age = 42 })</para>
        /// </summary>
        public bool Exists<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Exists<T>(sql, anonType));
        }

        /// <summary>
        /// Returns true if the Query returns any records, using an SqlFormat query. E.g:
        /// <para>db.ExistsFmt&lt;Person&gt;("Age = {0}", 42)</para>
        /// <para>db.ExistsFmt&lt;Person&gt;("SELECT * FROM Person WHERE Age = {0}", 50)</para>
        /// </summary>
        public bool ExistsFmt<T>(string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExistsFmt<T>(sqlFormat, filterParams));
        }

        /// <summary>
        /// Returns results from an arbitrary SqlExpression. E.g:
        /// <para>db.SqlList&lt;Person&gt;(db.From&lt;Person&gt;().Select("*").Where(q => q.Age &lt; 50))</para>
        /// </summary>
        public List<T> SqlList<T>(ISqlExpression sqlExpression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sqlExpression.ToSelectStatement(), sqlExpression.Params));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        public List<T> SqlList<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", new { age = 50 })</para>
        /// </summary>
        public List<T> SqlList<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sql, anonType));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", new Dictionary&lt;string, object&gt; { { "age", 42 } })</para>
        /// </summary>
        public List<T> SqlList<T>(string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sql, dict));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query with a dbCmd filter. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", dbCmd => ...)</para>
        /// </summary>
        public List<T> SqlList<T>(string sql, Action<IDbCommand> dbCmdFilter)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlList<T>(sql, dbCmdFilter));
        }

        /// <summary>
        /// Prepare Stored Procedure with Input parameters, optionally populated with Input Params. E.g:
        /// <para>var cmd = db.SqlProc("GetRockstarsAged", new { age = 42 })</para>
        /// </summary>
        public IDbCommand SqlProc(string name, object inParams = null, bool excludeDefaults = false)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlProc(name, inParams, excludeDefaults));
        }

        /// <summary>
        /// Returns the first column in a List using an SqlExpression. E.g:
        /// <para>db.SqlColumn&lt;string&gt;(db.From&lt;Person&gt;().Select(x => x.LastName).Where(q => q.Age < 50))</para>
        /// </summary>
        public List<T> SqlColumn<T>(ISqlExpression sqlExpression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumn<T>(sqlExpression.ToSelectStatement(), sqlExpression.Params));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumn&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        public List<T> SqlColumn<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumn<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumn&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        public List<T> SqlColumn<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumn<T>(sql, anonType));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumn&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new Dictionary&lt;string, object&gt; { { "age", 50 } })</para>
        /// </summary>
        public List<T> SqlColumn<T>(string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumn<T>(sql, dict));
        }

        /// <summary>
        /// Returns a single Scalar value using an SqlExpression. E.g:
        /// <para>db.SqlScalar&lt;int&gt;(db.From&lt;Person&gt;().Select(Sql.Count("*")).Where(q => q.Age &lt; 50))</para>
        /// </summary>
        public T SqlScalar<T>(ISqlExpression sqlExpression)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalar<T>(sqlExpression.ToSelectStatement(), sqlExpression.Params));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new[]{ db.CreateParam("age",50) })</para>
        /// </summary>
        public T SqlScalar<T>(string sql, IEnumerable<IDbDataParameter> sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalar<T>(sql, sqlParams));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        public T SqlScalar<T>(string sql, object anonType = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalar<T>(sql, anonType));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new Dictionary&lt;string, object&gt; { { "age", 50 } })</para>
        /// </summary>
        public T SqlScalar<T>(string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalar<T>(sql, dict));
        }

        /// <summary>
        /// Returns the last insert Id made from this connection.
        /// </summary>
        public long LastInsertId()
        {
            return dbConn.Exec(dbCmd => dbCmd.LastInsertId());
        }

        /// <summary>
        /// Executes a raw sql non-query using sql. E.g:
        /// <para>var rowsAffected = db.ExecuteNonQuery("UPDATE Person SET LastName={0} WHERE Id={1}".SqlFormat("WaterHouse", 7))</para>
        /// </summary>
        /// <returns>number of rows affected</returns>
        public int ExecuteNonQuery(string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQuery(sql));
        }

        /// <summary>
        /// Executes a raw sql non-query using a parameterized query. E.g:
        /// <para>var rowsAffected = db.ExecuteNonQuery("UPDATE Person SET LastName=@name WHERE Id=@id", new { name = "WaterHouse", id = 7 })</para>
        /// </summary>
        /// <returns>number of rows affected</returns>
        public int ExecuteNonQuery(string sql, object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQuery(sql, anonType));
        }

        /// <summary>
        /// Executes a raw sql non-query using a parameterized query.
        /// </summary>
        /// <returns>number of rows affected</returns>
        public int ExecuteNonQuery(string sql, Dictionary<string, object> dict)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQuery(sql, dict));
        }

        /// <summary>
        /// Returns results from a Stored Procedure, using a parameterized query.
        /// </summary>
        public List<TOutputModel> SqlProcedure<TOutputModel>(object anonType)
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlProcedure<TOutputModel>(anonType));
        }

        /// <summary>
        /// Returns results from a Stored Procedure using an SqlFormat query. E.g:
        /// <para></para>
        /// </summary>
        public List<TOutputModel> SqlProcedure<TOutputModel>(object anonType,
            string sqlFilter,
            params object[] filterParams)
            where TOutputModel : new()
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlProcedureFmt<TOutputModel>(
                anonType, sqlFilter, filterParams));
        }

        /// <summary>
        /// Returns the scalar result as a long.
        /// </summary>
        public long LongScalar()
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecLongScalar());
        }

        /// <summary>
        /// Returns the first result with all its references loaded, using a primary key id. E.g:
        /// <para>db.LoadSingleById&lt;Person&gt;(1, include = new[]{ "Address" })</para>
        /// </summary>
        public T LoadSingleById<T>(object idValue, string[] include = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSingleById<T>(idValue, include));
        }

        /// <summary>
        /// Returns the first result with all its references loaded, using a primary key id. E.g:
        /// <para>db.LoadSingleById&lt;Person&gt;(1, include = x => new{ x.Address })</para>
        /// </summary>
        public T LoadSingleById<T>(object idValue, Func<T, object> include)
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSingleById<T>(idValue, include(typeof(T).CreateInstance<T>()).GetType().AllAnonFields()));
        }

        /// <summary>
        /// Loads all the related references onto the instance. E.g:
        /// <para>db.LoadReferences(customer)</para> 
        /// </summary>
        public void LoadReferences<T>(T instance)
        {
            dbConn.Exec(dbCmd => dbCmd.LoadReferences(instance));
        }
        #endregion

        #region OrmLiteWriteExpressionsApi
        /// <summary>
        /// Use an SqlExpression to select which fields to update and construct the where expression, E.g: 
        /// 
        ///   db.UpdateOnly(new Person { FirstName = "JJ" }, ev => ev.Update(p => p.FirstName).Where(x => x.FirstName == "Jimi"));
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("FirstName" = 'Jimi')
        /// 
        ///   What's not in the update expression doesn't get updated. No where expression updates all rows. E.g:
        /// 
        ///   db.UpdateOnly(new Person { FirstName = "JJ", LastName = "Hendo" }, ev => ev.Update(p => p.FirstName));
        ///   UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        public int UpdateOnly<T>(T model, Func<SqlExpression<T>, SqlExpression<T>> onlyFields)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnly(model, onlyFields));
        }

        /// <summary>
        /// Use an SqlExpression to select which fields to update and construct the where expression, E.g: 
        /// 
        ///   var q = db.From&gt;Person&lt;());
        ///   db.UpdateOnly(new Person { FirstName = "JJ" }, q.Update(p => p.FirstName).Where(x => x.FirstName == "Jimi"));
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("FirstName" = 'Jimi')
        /// 
        ///   What's not in the update expression doesn't get updated. No where expression updates all rows. E.g:
        /// 
        ///   db.UpdateOnly(new Person { FirstName = "JJ", LastName = "Hendo" }, ev.Update(p => p.FirstName));
        ///   UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        public int UpdateOnly<T>(T model, SqlExpression<T> onlyFields)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnly(model, onlyFields));
        }

        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// 
        ///   db.UpdateOnly(new Person { FirstName = "JJ" }, p => p.FirstName, p => p.LastName == "Hendrix");
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        ///
        ///   db.UpdateOnly(new Person { FirstName = "JJ" }, p => p.FirstName);
        ///   UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        public int UpdateOnly<T, TKey>(T obj,
            Expression<Func<T, TKey>> onlyFields = null,
            Expression<Func<T, bool>> where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnly(obj, onlyFields, where));
        }

        /// <summary>
        /// Updates all non-default values set on item matching the where condition (if any). E.g
        /// 
        ///   db.UpdateNonDefaults(new Person { FirstName = "JJ" }, p => p.FirstName == "Jimi");
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("FirstName" = 'Jimi')
        /// </summary>
        public int UpdateNonDefaults<T>(T item, Expression<Func<T, bool>> obj)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateNonDefaults(item, obj));
        }

        /// <summary>
        /// Updates all values set on item matching the where condition (if any). E.g
        /// 
        ///   db.Update(new Person { Id = 1, FirstName = "JJ" }, p => p.LastName == "Hendrix");
        ///   UPDATE "Person" SET "Id" = 1,"FirstName" = 'JJ',"LastName" = NULL,"Age" = 0 WHERE ("LastName" = 'Hendrix')
        /// </summary>
        public int Update<T>(T item, Expression<Func<T, bool>> where)
        {
            return dbConn.Exec(dbCmd => dbCmd.Update(item, where));
        }

        /// <summary>
        /// Updates all matching fields populated on anonymousType that matches where condition (if any). E.g:
        /// 
        ///   db.Update&lt;Person&gt;(new { FirstName = "JJ" }, p => p.LastName == "Hendrix");
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        /// </summary>
        public int Update<T>(object updateOnly, Expression<Func<T, bool>> where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.Update(updateOnly, where));
        }

        /// <summary>
        /// Flexible Update method to succinctly execute a free-text update statement using optional params. E.g:
        /// 
        ///   db.Update&lt;Person&gt;(set:"FirstName = {0}".Params("JJ"), where:"LastName = {0}".Params("Hendrix"));
        ///   UPDATE "Person" SET FirstName = 'JJ' WHERE LastName = 'Hendrix'
        /// </summary>
        public int UpdateFmt<T>(string set = null, string where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateFmt<T>(set, where));
        }

        /// <summary>
        /// Flexible Update method to succinctly execute a free-text update statement using optional params. E.g.
        /// 
        ///   db.Update(table:"Person", set: "FirstName = {0}".Params("JJ"), where: "LastName = {0}".Params("Hendrix"));
        ///   UPDATE "Person" SET FirstName = 'JJ' WHERE LastName = 'Hendrix'
        /// </summary>
        public int UpdateFmt(string table = null, string set = null, string where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateFmt(table, set, where));
        }

        /// <summary>
        /// Insert only fields in POCO specified by the SqlExpression lambda. E.g:
        /// <para>db.InsertOnly(new Person { FirstName = "Amy", Age = 27 }, q =&gt; q.Insert(p =&gt; new { p.FirstName, p.Age }))</para>
        /// </summary>
        public void InsertOnly<T>(T obj, Func<SqlExpression<T>, SqlExpression<T>> onlyFields)
        {
            dbConn.Exec(dbCmd => dbCmd.InsertOnly(obj, onlyFields));
        }

        /// <summary>
        /// Using an SqlExpression to only Insert the fields specified, e.g:
        /// 
        ///   db.InsertOnly(new Person { FirstName = "Amy" }, q => q.Insert(p => new { p.FirstName }));
        ///   INSERT INTO "Person" ("FirstName") VALUES ('Amy');
        /// </summary>
        public void InsertOnly<T>(T obj, SqlExpression<T> onlyFields)
        {
            dbConn.Exec(dbCmd => dbCmd.InsertOnly(obj, onlyFields));
        }

        /// <summary>
        /// Delete the rows that matches the where expression, e.g:
        /// 
        ///   db.Delete&lt;Person&gt;(p => p.Age == 27);
        ///   DELETE FROM "Person" WHERE ("Age" = 27)
        /// </summary>
        public int Delete<T>(Expression<Func<T, bool>> where)
        {
            return dbConn.Exec(dbCmd => dbCmd.Delete(where));
        }

        /// <summary>
        /// Delete the rows that matches the where expression, e.g:
        /// 
        ///   db.Delete&lt;Person&gt;(ev => ev.Where(p => p.Age == 27));
        ///   DELETE FROM "Person" WHERE ("Age" = 27)
        /// </summary>
        public int Delete<T>(Func<SqlExpression<T>, SqlExpression<T>> where)
        {
            return dbConn.Exec(dbCmd => dbCmd.Delete(where));
        }

        /// <summary>
        /// Delete the rows that matches the where expression, e.g:
        /// 
        ///   var q = db.From&gt;Person&lt;());
        ///   db.Delete&lt;Person&gt;(q.Where(p => p.Age == 27));
        ///   DELETE FROM "Person" WHERE ("Age" = 27)
        /// </summary>
        public int Delete<T>(SqlExpression<T> where)
        {
            return dbConn.Exec(dbCmd => dbCmd.Delete(where));
        }

        /// <summary>
        /// Flexible Delete method to succinctly execute a delete statement using free-text where expression. E.g.
        /// 
        ///   db.Delete&lt;Person&gt;(where:"Age = {0}".Params(27));
        ///   DELETE FROM "Person" WHERE Age = 27
        /// </summary>
        public int DeleteFmt<T>(string where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmt<T>(where));
        }

        /// <summary>
        /// Flexible Delete method to succinctly execute a delete statement using free-text where expression. E.g.
        /// 
        ///   db.Delete(table:"Person", where: "Age = {0}".Params(27));
        ///   DELETE FROM "Person" WHERE Age = 27
        /// </summary>
        public int DeleteFmt(string table = null, string where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmt(table, where));
        }
        #endregion

        #region OrmLiteSchemaApi
        /// <summary>
        /// Checks whether a Table Exists. E.g:
        /// <para>db.TableExists("Person")</para>
        /// </summary>
        public bool TableExists(string tableName, string schema = null)
        {
            return dbConn.GetDialectProvider().DoesTableExist(dbConn, tableName, schema);
        }

        /// <summary>
        /// Checks whether a Table Exists. E.g:
        /// <para>db.TableExists&lt;Person&gt;()</para>
        /// </summary>
        public bool TableExists<T>()
        {
            var dialectProvider = dbConn.GetDialectProvider();
            var modelDef = typeof(T).GetModelDefinition();
            var tableName = dialectProvider.NamingStrategy.GetTableName(modelDef);
            return dialectProvider.DoesTableExist(dbConn, tableName, modelDef.Schema);
        }

        /// <summary>
        /// Create DB Tables from the schemas of runtime types. E.g:
        /// <para>db.CreateTables(typeof(Table1), typeof(Table2))</para> 
        /// </summary>
        public void CreateTables(bool overwrite, params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTables(overwrite, tableTypes));
        }

        /// <summary>
        /// Create DB Table from the schema of the runtime type. Use overwrite to drop existing Table. E.g:
        /// <para>db.CreateTable(true, typeof(Table))</para> 
        /// </summary>
        public void CreateTable(bool overwrite, Type modelType)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable(overwrite, modelType));
        }

        /// <summary>
        /// Only Create new DB Tables from the schemas of runtime types if they don't already exist. E.g:
        /// <para>db.CreateTableIfNotExists(typeof(Table1), typeof(Table2))</para> 
        /// </summary>
        public void CreateTableIfNotExists(params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTables(overwrite: false, tableTypes: tableTypes));
        }

        /// <summary>
        /// Drop existing DB Tables and re-create them from the schemas of runtime types. E.g:
        /// <para>db.DropAndCreateTables(typeof(Table1), typeof(Table2))</para> 
        /// </summary>
        public void DropAndCreateTables(params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTables(overwrite: true, tableTypes: tableTypes));
        }

        /// <summary>
        /// Create a DB Table from the generic type. Use overwrite to drop the existing table or not. E.g:
        /// <para>db.CreateTable&lt;Person&gt;(overwrite=false) //default</para> 
        /// <para>db.CreateTable&lt;Person&gt;(overwrite=true)</para> 
        /// </summary>
        public void CreateTable<T>(bool overwrite = false)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable<T>(overwrite));
        }

        /// <summary>
        /// Only create a DB Table from the generic type if it doesn't already exist. E.g:
        /// <para>db.CreateTableIfNotExists&lt;Person&gt;()</para> 
        /// </summary>
        public bool CreateTableIfNotExists<T>()
        {
            return dbConn.Exec(dbCmd => dbCmd.CreateTable<T>(overwrite: false));
        }

        /// <summary>
        /// Only create a DB Table from the runtime type if it doesn't already exist. E.g:
        /// <para>db.CreateTableIfNotExists(typeof(Person))</para> 
        /// </summary>
        public bool CreateTableIfNotExists(Type modelType)
        {
            return dbConn.Exec(dbCmd => dbCmd.CreateTable(false, modelType));
        }

        /// <summary>
        /// Drop existing table if exists and re-create a DB Table from the generic type. E.g:
        /// <para>db.DropAndCreateTable&lt;Person&gt;()</para> 
        /// </summary>
        public void DropAndCreateTable<T>()
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable<T>(true));
        }

        /// <summary>
        /// Drop existing table if exists and re-create a DB Table from the runtime type. E.g:
        /// <para>db.DropAndCreateTable(typeof(Person))</para> 
        /// </summary>
        public void DropAndCreateTable(Type modelType)
        {
            dbConn.Exec(dbCmd => dbCmd.CreateTable(true, modelType));
        }

        /// <summary>
        /// Drop any existing tables from their runtime types. E.g:
        /// <para>db.DropTables(typeof(Table1),typeof(Table2))</para> 
        /// </summary>
        public void DropTables(params Type[] tableTypes)
        {
            dbConn.Exec(dbCmd => dbCmd.DropTables(tableTypes));
        }

        /// <summary>
        /// Drop any existing tables from the runtime type. E.g:
        /// <para>db.DropTable(typeof(Person))</para> 
        /// </summary>
        public void DropTable(Type modelType)
        {
            dbConn.Exec(dbCmd => dbCmd.DropTable(modelType));
        }

        /// <summary>
        /// Drop any existing tables from the generic type. E.g:
        /// <para>db.DropTable&lt;Person&gt;()</para> 
        /// </summary>
        public void DropTable<T>()
        {
            dbConn.Exec(dbCmd => dbCmd.DropTable<T>());
        }

        #endregion

        #region OrmLiteWriteApi
        /// <summary>
        /// Get the last SQL statement that was executed.
        /// </summary>
        public string GetLastSql()
        {
            var ormLiteConn = dbConn as OrmLiteConnection;
            return ormLiteConn != null ? ormLiteConn.LastCommandText : null;
        }


        /// <summary>
        /// Execute any arbitrary raw SQL.
        /// </summary>
        /// <returns>number of rows affected</returns>
        public int ExecuteSql(string sql)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteSql(sql));
        }

        /// <summary>
        /// Execute any arbitrary raw SQL with db params.
        /// </summary>
        /// <returns>number of rows affected</returns>
        public int ExecuteSql(string sql, object dbParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteSql(sql, dbParams));
        }

        /// <summary>
        /// Insert 1 POCO, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
        /// <para>var id = db.Insert(new Person { Id = 1, FirstName = "Jimi }, selectIdentity:true)</para>
        /// </summary>
        public long Insert<T>(T obj, bool selectIdentity = false)
        {
            return dbConn.Exec(dbCmd => dbCmd.Insert(obj, selectIdentity));
        }

        /// <summary>
        /// Insert 1 or more POCOs in a transaction. E.g:
        /// <para>db.Insert(new Person { Id = 1, FirstName = "Tupac", LastName = "Shakur", Age = 25 },</para>
        /// <para>          new Person { Id = 2, FirstName = "Biggie", LastName = "Smalls", Age = 24 })</para>
        /// </summary>
        public void Insert<T>(params T[] objs)
        {
            dbConn.Exec(dbCmd => dbCmd.Insert(objs));
        }
        /// <summary>
        /// 插入或更新一条记录
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dbConn">数据库连接</param>
        /// <param name="obj">插入或者更新的对象</param>
        public void InsertOrUpdate<T>(T obj)
        {
            if (dbConn.Exists<T>(obj))
            {
                dbConn.Update<T>(obj);
            }
            else
            {
                dbConn.Insert<T>(obj);
            }
        }
        /// <summary>
        /// 插入所有的对象(存在:更新,不存在:插入)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="lstObj"></param>
        /// <returns></returns>
        public long InsertOrUpdateAll<T>(IEnumerable<T> lstObj)
        {
            IDbTransaction dbTrans = null;
            long lCount = 0;
            try
            {
                IDbCommand dbCmd = dbConn.OpenCommand();
                if (dbCmd.Transaction == null)
                    dbCmd.Transaction = dbTrans = dbCmd.Connection.BeginTransaction();

                foreach (T obj in lstObj)
                {
                    if (dbCmd.Exists<T>(obj))
                    {
                        lCount += dbCmd.Update<T>(obj);
                    }
                    else
                    {
                        lCount += dbCmd.Insert<T>(obj);
                    }
                }

                if (dbTrans != null)
                    dbTrans.Commit();
            }
            catch (Exception ex)
            {
                lCount = 0;
                dbTrans.Rollback();
            }
            finally
            {
                if (dbTrans != null)
                    dbTrans.Dispose();
            }
            return lCount;
        }
        /// <summary>
        /// Insert a collection of POCOs in a transaction. E.g:
        /// <para>db.InsertAll(new[] { new Person { Id = 9, FirstName = "Biggie", LastName = "Smalls", Age = 24 } })</para>
        /// </summary>
        public void InsertAll<T>(IEnumerable<T> objs)
        {
            dbConn.Exec(dbCmd => dbCmd.InsertAll(objs));
        }

        /// <summary>
        /// Updates 1 POCO. All fields are updated except for the PrimaryKey which is used as the identity selector. E.g:
        /// <para>db.Update(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
        /// </summary>
        public int Update<T>(T obj)
        {
            return dbConn.Exec(dbCmd => dbCmd.Update(obj));
        }

        /// <summary>
        /// Updates 1 or more POCOs in a transaction. E.g:
        /// <para>db.Update(new Person { Id = 1, FirstName = "Tupac", LastName = "Shakur", Age = 25 },</para>
        /// <para>new Person { Id = 2, FirstName = "Biggie", LastName = "Smalls", Age = 24 })</para>
        /// </summary>
        public int Update<T>(params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.Update(objs));
        }

        /// <summary>
        /// Updates 1 or more POCOs in a transaction. E.g:
        /// <para>db.UpdateAll(new[] { new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 } })</para>
        /// </summary>
        public int UpdateAll<T>(IEnumerable<T> objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAll(objs));
        }

        /// <summary>
        /// Delete rows using an anonymous type filter. E.g:
        /// <para>db.Delete&lt;Person&gt;(new { FirstName = "Jimi", Age = 27 })</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public int Delete<T>(object anonFilter)
        {
            return dbConn.Exec(dbCmd => dbCmd.Delete<T>(anonFilter));
        }

        /// <summary>
        /// Delete 1 row using all fields in the filter. E.g:
        /// <para>db.Delete(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public int Delete<T>(T allFieldsFilter)
        {
            return dbConn.Exec(dbCmd => dbCmd.Delete(allFieldsFilter));
        }

        /// <summary>
        /// Delete 1 or more rows in a transaction using all fields in the filter. E.g:
        /// <para>db.Delete(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
        /// </summary>
        public int Delete<T>(params T[] allFieldsFilters)
        {
            return dbConn.Exec(dbCmd => dbCmd.Delete(allFieldsFilters));
        }

        /// <summary>
        /// Delete 1 or more rows using only field with non-default values in the filter. E.g:
        /// <para>db.DeleteNonDefaults(new Person { FirstName = "Jimi", Age = 27 })</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public int DeleteNonDefaults<T>(T nonDefaultsFilter)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteNonDefaults(nonDefaultsFilter));
        }

        /// <summary>
        /// Delete 1 or more rows in a transaction using only field with non-default values in the filter. E.g:
        /// <para>db.DeleteNonDefaults(new Person { FirstName = "Jimi", Age = 27 }, 
        /// new Person { FirstName = "Janis", Age = 27 })</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public int DeleteNonDefaults<T>(params T[] nonDefaultsFilters)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteNonDefaults(nonDefaultsFilters));
        }

        /// <summary>
        /// Delete 1 row by the PrimaryKey. E.g:
        /// <para>db.DeleteById&lt;Person&gt;(1)</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public int DeleteById<T>(object id)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteById<T>(id));
        }

        /// <summary>
        /// Delete 1 row by the PrimaryKey where the rowVersion matches the optimistic concurrency field. 
        /// Will throw <exception cref="OptimisticConcurrencyException">RowModefiedExeption</exception> if the 
        /// row does not exist or has a different row version.
        /// E.g: <para>db.DeleteById&lt;Person&gt;(1)</para>
        /// </summary>
        public void DeleteById<T>(object id, ulong rowVersion)
        {
            dbConn.Exec(dbCmd => dbCmd.DeleteById<T>(id, rowVersion));
        }

        /// <summary>
        /// Delete all rows identified by the PrimaryKeys. E.g:
        /// <para>db.DeleteById&lt;Person&gt;(new[] { 1, 2, 3 })</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public int DeleteByIds<T>(IEnumerable idValues)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteByIds<T>(idValues));
        }

        /// <summary>
        /// Delete all rows in the generic table type. E.g:
        /// <para>db.DeleteAll&lt;Person&gt;()</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public int DeleteAll<T>()
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAll<T>());
        }

        /// <summary>
        /// Delete all rows provided. E.g:
        /// <para>db.DeleteAll&lt;Person&gt;(people)</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public int DeleteAll<T>(IEnumerable<T> rows)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAll(rows));
        }

        /// <summary>
        /// Delete all rows in the runtime table type. E.g:
        /// <para>db.DeleteAll(typeof(Person))</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public int DeleteAll(Type tableType)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAll(tableType));
        }

        /// <summary>
        /// Delete rows using a SqlFormat filter. E.g:
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public int DeleteFmt<T>(string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmt<T>(sqlFilter, filterParams));
        }

        /// <summary>
        /// Delete rows from the runtime table type using a SqlFormat filter. E.g:
        /// </summary>
        /// <para>db.DeleteFmt(typeof(Person), "Age = {0}", 27)</para>
        /// <returns>number of rows deleted</returns>
        public int DeleteFmt(Type tableType, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmt(tableType, sqlFilter, filterParams));
        }

        /// <summary>
        /// Insert a new row or update existing row. Returns true if a new row was inserted. 
        /// Optional references param decides whether to save all related references as well. E.g:
        /// <para>db.Save(customer, references:true)</para>
        /// </summary>
        /// <returns>true if a row was inserted; false if it was updated</returns>
        public bool Save<T>(T obj, bool references = false)
        {
            if (!references)
                return dbConn.Exec(dbCmd => dbCmd.Save(obj));

            return dbConn.Exec(dbCmd =>
            {
                var ret = dbCmd.Save(obj);
                dbCmd.SaveAllReferences(obj);
                return ret;
            });
        }

        /// <summary>
        /// Insert new rows or update existing rows. Return number of rows added E.g:
        /// <para>db.Save(new Person { Id = 10, FirstName = "Amy", LastName = "Winehouse", Age = 27 })</para>
        /// </summary>
        /// <returns>number of rows added</returns>
        public int Save<T>(params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.Save(objs));
        }

        /// <summary>
        /// Insert new rows or update existing rows. Return number of rows added E.g:
        /// <para>db.SaveAll(new [] { new Person { Id = 10, FirstName = "Amy", LastName = "Winehouse", Age = 27 } })</para>
        /// </summary>
        /// <returns>number of rows added</returns>
        public int SaveAll<T>(IEnumerable<T> objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveAll(objs));
        }

        /// <summary>
        /// Populates all related references on the instance with its primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveAllReferences(customer)</para> 
        /// </summary>
        public void SaveAllReferences<T>(T instance)
        {
            dbConn.Exec(dbCmd => dbCmd.SaveAllReferences(instance));
        }

        /// <summary>
        /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveReference(customer, customer.Orders)</para> 
        /// </summary>
        public void SaveReferences<T, TRef>(T instance, params TRef[] refs)
        {
            dbConn.Exec(dbCmd => dbCmd.SaveReferences(instance, refs));
        }

        /// <summary>
        /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveReference(customer, customer.Orders)</para> 
        /// </summary>
        public void SaveReferences<T, TRef>(T instance, List<TRef> refs)
        {
            dbConn.Exec(dbCmd => dbCmd.SaveReferences(instance, refs.ToArray()));
        }

        /// <summary>
        /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveReferences(customer, customer.Orders)</para> 
        /// </summary>
        public void SaveReferences<T, TRef>(T instance, IEnumerable<TRef> refs)
        {
            dbConn.Exec(dbCmd => dbCmd.SaveReferences(instance, refs.ToArray()));
        }

        // Procedures
        public void ExecuteProcedure<T>(T obj)
        {
            dbConn.Exec(dbCmd => dbCmd.ExecuteProcedure(obj));
        }
        #endregion

        #region OrmLiteSchemaModifyApi
        public void AlterTable<T>(string command)
        {
            AlterTable(typeof(T), command);
        }

        public void AlterTable(Type modelType, string command)
        {
            var sql = string.Format("ALTER TABLE {0} {1};",
                dbConn.GetDialectProvider().GetQuotedTableName(modelType.GetModelDefinition()),
                command);
            dbConn.ExecuteSql(sql);
        }

        public void AddColumn<T>(Expression<Func<T, object>> field)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition<T>(field);
            dbConn.AddColumn(typeof(T), fieldDef);
        }


        public void AddColumn(Type modelType, FieldDefinition fieldDef)
        {
            var command = dbConn.GetDialectProvider().ToAddColumnStatement(modelType, fieldDef);
            dbConn.ExecuteSql(command);
        }


        public void AlterColumn<T>(Expression<Func<T, object>> field)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition<T>(field);
            dbConn.AlterColumn(typeof(T), fieldDef);
        }

        public void AlterColumn(Type modelType, FieldDefinition fieldDef)
        {
            var command = dbConn.GetDialectProvider().ToAlterColumnStatement(modelType, fieldDef);
            dbConn.ExecuteSql(command);
        }


        public void ChangeColumnName<T>(Expression<Func<T, object>> field,
                                               string oldColumnName)
        {
            var modelDef = ModelDefinition<T>.Definition;
            var fieldDef = modelDef.GetFieldDefinition<T>(field);
            dbConn.ChangeColumnName(typeof(T), fieldDef, oldColumnName);
        }

        public void ChangeColumnName(Type modelType,
                                            FieldDefinition fieldDef,
                                            string oldColumnName)
        {
            var command = dbConn.GetDialectProvider().ToChangeColumnNameStatement(modelType, fieldDef, oldColumnName);
            dbConn.ExecuteSql(command);
        }

        public void DropColumn<T>(string columnName)
        {
            dbConn.DropColumn(typeof(T), columnName);
        }


        public void DropColumn(Type modelType, string columnName)
        {
            var provider = dbConn.GetDialectProvider();
            var command = string.Format("ALTER TABLE {0} DROP {1};",
                                           provider.GetQuotedTableName(modelType.GetModelDefinition().ModelName),
                                           provider.GetQuotedName(columnName));

            dbConn.ExecuteSql(command);
        }



        public void AddForeignKey<T, TForeign>(Expression<Func<T, object>> field, 
            Expression<Func<TForeign, object>> foreignField,
            OnFkOption onUpdate,
            OnFkOption onDelete,
            string foreignKeyName = null)
        {
            var command = dbConn.GetDialectProvider().ToAddForeignKeyStatement(field,
                                                                                    foreignField,
                                                                                    onUpdate,
                                                                                    onDelete,
                                                                                    foreignKeyName);
            dbConn.ExecuteSql(command);
        }


        public void DropForeignKey<T>(string foreignKeyName)
        {
            var provider = dbConn.GetDialectProvider();
            var modelDef = ModelDefinition<T>.Definition;
            var command = string.Format(provider.GetDropForeignKeyConstraints(modelDef),
                                           provider.GetQuotedTableName(modelDef.ModelName),
                                           provider.GetQuotedName(foreignKeyName));
            dbConn.ExecuteSql(command);
        }


        public void CreateIndex<T>(Expression<Func<T, object>> field,
            string indexName = null, bool unique = false)
        {
            var command = dbConn.GetDialectProvider().ToCreateIndexStatement(field, indexName, unique);
            dbConn.ExecuteSql(command);
        }


        public void DropIndex<T>(string indexName)
        {
            var provider = dbConn.GetDialectProvider();
            var command = string.Format("ALTER TABLE {0} DROP INDEX  {1};",
                                           provider.GetQuotedTableName(ModelDefinition<T>.Definition.ModelName),
                                           provider.GetQuotedName(indexName));
            dbConn.ExecuteSql(command);
        }

        #endregion

        #region OrmLiteReadApiAsync
#if NET45
        /// <summary>
        /// Returns results from the active connection.
        /// </summary>
        public  Task<List<T>> SelectAsync<T>(CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<T>(token));
        }

        /// <summary>
        /// Returns results from using sql. E.g:
        /// <para>db.Select&lt;Person&gt;("Age &gt; 40")</para>
        /// <para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; 40")</para>
        /// </summary>
        public  Task<List<T>> SelectAsync<T>(string sql, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<T>(sql, (object)null, token));
        }

        /// <summary>
        /// Returns results from using a parameterized query. E.g:
        /// <para>db.Select&lt;Person&gt;("Age &gt; @age", new { age = 40})</para>
        /// <para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new[] { db.CreateParam("age",40) })</para>
        /// </summary>
        public  Task<List<T>> SelectAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns results from using a parameterized query. E.g:
        /// <para>db.Select&lt;Person&gt;("Age &gt; @age", new { age = 40})</para>
        /// <para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new { age = 40})</para>
        /// </summary>
        public  Task<List<T>> SelectAsync<T>(string sql, object anonType, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns results from using a parameterized query. E.g:
        /// <para>db.Select&lt;Person&gt;("Age &gt; @age", new Dictionary&lt;string, object&gt; { { "age", 40 } })</para>
        /// <para>db.Select&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; @age", new Dictionary&lt;string, object&gt; { { "age", 40 } })</para>
        /// </summary>
        public  Task<List<T>> SelectAsync<T>(string sql, Dictionary<string, object> dict, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<T>(sql, dict, token));
        }

        /// <summary>
        /// Returns results from using an SqlFormat query. E.g:
        /// <para>db.SelectFmt&lt;Person&gt;("Age &gt; {0}", 40)</para>
        /// <para>db.SelectFmt&lt;Person&gt;("SELECT * FROM Person WHERE Age &gt; {0}", 40)</para>
        /// </summary>
        public  Task<List<T>> SelectFmtAsync<T>(CancellationToken token, string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectFmtAsync<T>(token, sqlFormat, filterParams));
        }
        public  Task<List<T>> SelectFmtAsync<T>(string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectFmtAsync<T>(default(CancellationToken), sqlFormat, filterParams));
        }

        /// <summary>
        /// Returns a partial subset of results from the specified tableType. E.g:
        /// <para>db.Select&lt;EntityWithId&gt;(typeof(Person))</para>
        /// <para></para>
        /// </summary>
        public  Task<List<TModel>> SelectAsync<TModel>(Type fromTableType, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<TModel>(fromTableType, token));
        }

        /// <summary>
        /// Returns a partial subset of results from the specified tableType using a SqlFormat query. E.g:
        /// <para>db.SelectFmt&lt;EntityWithId&gt;(typeof(Person), "Age &gt; {0}", 40)</para>
        /// </summary>
        public  Task<List<TModel>> SelectFmtAsync<TModel>(CancellationToken token, Type fromTableType, string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectFmtAsync<TModel>(token, fromTableType, sqlFormat, filterParams));
        }
        public  Task<List<TModel>> SelectFmtAsync<TModel>(Type fromTableType, string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectFmtAsync<TModel>(default(CancellationToken), fromTableType, sqlFormat, filterParams));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.Where&lt;Person&gt;("Age", 27)</para>
        /// </summary>
        public  Task<List<T>> WhereAsync<T>(string name, object value, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.WhereAsync<T>(name, value, token));
        }

        /// <summary>
        /// Returns results from using an anonymous type filter. E.g:
        /// <para>db.Where&lt;Person&gt;(new { Age = 27 })</para>
        /// </summary>
        public  Task<List<T>> WhereAsync<T>(object anonType, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.WhereAsync<T>(anonType, token));
        }

        /// <summary>
        /// Returns results using the supplied primary key ids. E.g:
        /// <para>db.SelectByIds&lt;Person&gt;(new[] { 1, 2, 3 })</para>
        /// </summary>
        public  Task<List<T>> SelectByIdsAsync<T>(IEnumerable idValues, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectByIdsAsync<T>(idValues, token));
        }

        /// <summary>
        /// Query results using the non-default values in the supplied partially populated POCO example. E.g:
        /// <para>db.SelectNonDefaults(new Person { Id = 1 })</para>
        /// </summary>
        public  Task<List<T>> SelectNonDefaultsAsync<T>(T filter, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectNonDefaultsAsync<T>(filter, token));
        }

        /// <summary>
        /// Query results using the non-default values in the supplied partially populated POCO example. E.g:
        /// <para>db.SelectNonDefaults("Age &gt; @Age", new Person { Age = 42 })</para>
        /// </summary>
        public  Task<List<T>> SelectNonDefaultsAsync<T>(string sql, T filter, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectNonDefaultsAsync<T>(sql, filter, token));
        }

        /// <summary>
        /// Returns the first result using a parameterized query. E.g:
        /// <para>db.Single&lt;Person&gt;(new { Age = 42 })</para>
        /// </summary>
        public  Task<T> SingleAsync<T>(object anonType, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleAsync<T>(anonType, token));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.Single&lt;Person&gt;("Age = @age", new[] { db.CreateParam("age",42) })</para>
        /// </summary>
        public  Task<T> SingleAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns results from using a single name, value filter. E.g:
        /// <para>db.Single&lt;Person&gt;("Age = @age", new { age = 42 })</para>
        /// </summary>
        public  Task<T> SingleAsync<T>(string sql, object anonType = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns the first result using a SqlFormat query. E.g:
        /// <para>db.SingleFmt&lt;Person&gt;("Age = {0}", 42)</para>
        /// </summary>
        public  Task<T> SingleFmtAsync<T>(CancellationToken token, string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleFmtAsync<T>(token, sqlFormat, filterParams));
        }
        public  Task<T> SingleFmtAsync<T>(string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleFmtAsync<T>(default(CancellationToken), sqlFormat, filterParams));
        }

        /// <summary>
        /// Returns the first result using a primary key id. E.g:
        /// <para>db.SingleById&lt;Person&gt;(1)</para>
        /// </summary>
        public  Task<T> SingleByIdAsync<T>(object idValue, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleByIdAsync<T>(idValue, token));
        }

        /// <summary>
        /// Returns the first result using a name, value filter. E.g:
        /// <para>db.SingleWhere&lt;Person&gt;("Age", 42)</para>
        /// </summary>
        public  Task<T> SingleWhereAsync<T>(string name, object value, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleWhereAsync<T>(name, value, token));
        }



        /// <summary>
        /// Returns a single scalar value using an SqlExpression. E.g:
        /// <para>db.Column&lt;int&gt;(db.From&lt;Persion&gt;().Select(x => Sql.Count("*")).Where(q => q.Age > 40))</para>
        /// </summary>
        public  Task<T> ScalarAsync<T>(ISqlExpression sqlExpression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarAsync<T>(sqlExpression.ToSelectStatement(), sqlExpression.Params, token));
        }

        /// <summary>
        /// Returns a single scalar value using a parameterized query. E.g:
        /// <para>db.Scalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &gt; @age", new[] { db.CreateParam("age",40) })</para>
        /// </summary>
        public  Task<T> ScalarAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns a single scalar value using a parameterized query. E.g:
        /// <para>db.Scalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &gt; @age", new { age = 40 })</para>
        /// </summary>
        public  Task<T> ScalarAsync<T>(string sql, object anonType = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns a single scalar value using an SqlFormat query. E.g:
        /// <para>db.ScalarFmt&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &gt; {0}", 40)</para>
        /// </summary>
        public  Task<T> ScalarFmtAsync<T>(CancellationToken token, string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarFmtAsync<T>(token, sqlFormat, sqlParams));
        }
        public  Task<T> ScalarFmtAsync<T>(string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarFmtAsync<T>(default(CancellationToken), sqlFormat, sqlParams));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlExpression. E.g:
        /// <para>db.Column&lt;int&gt;(db.From&lt;Persion&gt;().Select(x => x.LastName).Where(q => q.Age == 27))</para>
        /// </summary>
        public  Task<List<T>> ColumnAsync<T>(ISqlExpression query, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnAsync<T>(query.ToSelectStatement(), query.Params, token));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.Column&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new[] { db.CreateParam("age",27) })</para>
        /// </summary>
        public  Task<List<T>> ColumnAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.Column&lt;string&gt;("SELECT LastName FROM Person WHERE Age = @age", new { age = 27 })</para>
        /// </summary>
        public  Task<List<T>> ColumnAsync<T>(string sql, object anonType = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns the first column in a List using a SqlFormat query. E.g:
        /// <para>db.ColumnFmt&lt;string&gt;("SELECT LastName FROM Person WHERE Age = {0}", 27)</para>
        /// </summary>
        public  Task<List<T>> ColumnFmtAsync<T>(CancellationToken token, string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnFmtAsync<T>(token, sqlFormat, sqlParams));
        }
        public  Task<List<T>> ColumnFmtAsync<T>(string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnFmtAsync<T>(default(CancellationToken), sqlFormat, sqlParams));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlExpression. E.g:
        /// <para>db.ColumnDistinct&lt;int&gt;(db.From&lt;Persion&gt;().Select(x => x.Age).Where(q => q.Age < 50))</para>
        /// </summary>
        public  Task<HashSet<T>> ColumnDistinctAsync<T>(ISqlExpression query, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinctAsync<T>(query.ToSelectStatement(), query.Params, token));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlFormat query. E.g:
        /// <para>db.ColumnDistinct&lt;int&gt;("SELECT Age FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        public  Task<HashSet<T>> ColumnDistinctAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinctAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlFormat query. E.g:
        /// <para>db.ColumnDistinct&lt;int&gt;("SELECT Age FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        public  Task<HashSet<T>> ColumnDistinctAsync<T>(string sql, object anonType = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinctAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns the distinct first column values in a HashSet using an SqlFormat query. E.g:
        /// <para>db.ColumnDistinctFmt&lt;int&gt;("SELECT Age FROM Person WHERE Age &lt; {0}", 50)</para>
        /// </summary>
        public  Task<HashSet<T>> ColumnDistinctFmtAsync<T>(CancellationToken token, string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinctFmtAsync<T>(token, sqlFormat, sqlParams));
        }
        public  Task<HashSet<T>> ColumnDistinctFmtAsync<T>(string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ColumnDistinctFmtAsync<T>(default(CancellationToken), sqlFormat, sqlParams));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an Sql Expression. E.g:
        /// <para>db.Lookup&lt;int, string&gt;(db.From&lt;Person&gt;().Select(x => new { x.Age, x.LastName }).Where(q => q.Age < 50))</para>
        /// </summary>
        public  Task<Dictionary<K, List<V>>> LookupAsync<K, V>(ISqlExpression sqlExpression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.LookupAsync<K, V>(sqlExpression.ToSelectStatement(), sqlExpression.Params, token));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an parameterized query. E.g:
        /// <para>db.Lookup&lt;int, string&gt;("SELECT Age, LastName FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        public  Task<Dictionary<K, List<V>>> LookupAsync<K, V>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.LookupAsync<K, V>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an parameterized query. E.g:
        /// <para>db.Lookup&lt;int, string&gt;("SELECT Age, LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        public  Task<Dictionary<K, List<V>>> LookupAsync<K, V>(string sql, object anonType = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.LookupAsync<K, V>(sql, anonType, token));
        }

        /// <summary>
        /// Returns an Dictionary&lt;K, List&lt;V&gt;&gt; grouping made from the first two columns using an SqlFormat query. E.g:
        /// <para>db.LookupFmt&lt;int, string&gt;("SELECT Age, LastName FROM Person WHERE Age &lt; {0}", 50)</para>
        /// </summary>
        public  Task<Dictionary<K, List<V>>> LookupFmtAsync<K, V>(CancellationToken token, string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.LookupFmtAsync<K, V>(token, sqlFormat, sqlParams));
        }
        public  Task<Dictionary<K, List<V>>> LookupFmtAsync<K, V>(string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.LookupFmtAsync<K, V>(default(CancellationToken), sqlFormat, sqlParams));
        }

        /// <summary>
        /// Returns a Dictionary from the first 2 columns: Column 1 (Keys), Column 2 (Values) using an SqlExpression. E.g:
        /// <para>db.Dictionary&lt;int, string&gt;(db.From&lt;Person&gt;().Select(x => new { x.Id, x.LastName }).Where(x => x.Age < 50))</para>
        /// </summary>
        public  Task<Dictionary<K, V>> DictionaryAsync<K, V>(ISqlExpression query, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DictionaryAsync<K, V>(query.ToSelectStatement(), query.Params, token));
        }

        /// <summary>
        /// Returns a Dictionary from the first 2 columns: Column 1 (Keys), Column 2 (Values) using sql. E.g:
        /// <para>db.Dictionary&lt;int, string&gt;("SELECT Id, LastName FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        public  Task<Dictionary<K, V>> DictionaryAsync<K, V>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DictionaryAsync<K, V>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns a Dictionary from the first 2 columns: Column 1 (Keys), Column 2 (Values) using sql. E.g:
        /// <para>db.Dictionary&lt;int, string&gt;("SELECT Id, LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        public  Task<Dictionary<K, V>> DictionaryAsync<K, V>(string sql, object anonType = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DictionaryAsync<K, V>(sql, anonType, token));
        }

        /// <summary>
        /// Returns a Dictionary from the first 2 columns: Column 1 (Keys), Column 2 (Values) using an SqlFormat query. E.g:
        /// <para>db.DictionaryFmt&lt;int, string&gt;("SELECT Id, LastName FROM Person WHERE Age &lt; {0}", 50)</para>
        /// </summary>
        public  Task<Dictionary<K, V>> DictionaryFmtAsync<K, V>(CancellationToken token, string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DictionaryFmtAsync<K, V>(token, sqlFormat, sqlParams));
        }
        public  Task<Dictionary<K, V>> DictionaryFmtAsync<K, V>(string sqlFormat, params object[] sqlParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DictionaryFmtAsync<K, V>(default(CancellationToken), sqlFormat, sqlParams));
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the LINQ expression, E.g:
        /// <para>db.Exists&lt;Person&gt;(x =&gt; x.Age &lt; 50)</para>
        /// </summary>
        public  Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarAsync(dbConn.From<T>().Where(expression).Limit(1).Select("'exists'"), token).Then(x => x != null));
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the SqlExpression lambda, E.g:
        /// <para>db.Exists&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        public  Task<bool> ExistsAsync<T>(Func<SqlExpression<T>, SqlExpression<T>> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd =>
            {
                var q = dbCmd.GetDialectProvider().SqlExpression<T>();
                var sql = expression(q).Limit(1);
                return dbCmd.SingleAsync<T>(sql, token).Then(x => x != null);
            });
        }

        /// <summary>
        /// Returns true if the Query returns any records that match the supplied SqlExpression, E.g:
        /// <para>db.Exists(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        public  Task<bool> ExistsAsync<T>(SqlExpression<T> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarAsync(expression.Limit(1).Select("'exists'"), token).Then(x => x != null));
        }
        /// <summary>
        /// Returns true if the Query returns any records, using an SqlFormat query. E.g:
        /// <para>db.Exists&lt;Person&gt;(new { Age = 42 })</para>
        /// </summary>
        public  Task<bool> ExistsAsync<T>(object anonType, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ExistsAsync<T>(anonType, token));
        }

        /// <summary>
        /// Returns true if the Query returns any records, using a parameterized query. E.g:
        /// <para>db.Exists&lt;Person&gt;("Age = @age", new { age = 42 })</para>
        /// <para>db.Exists&lt;Person&gt;("SELECT * FROM Person WHERE Age = @age", new { age = 42 })</para>
        /// </summary>
        public  Task<bool> ExistsAsync<T>(string sql, object anonType = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ExistsAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns true if the Query returns any records, using an SqlFormat query. E.g:
        /// <para>db.ExistsFmt&lt;Person&gt;("Age = {0}", 42)</para>
        /// <para>db.ExistsFmt&lt;Person&gt;("SELECT * FROM Person WHERE Age = {0}", 50)</para>
        /// </summary>
        public  Task<bool> ExistsFmtAsync<T>(CancellationToken token, string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExistsFmtAsync<T>(token, sqlFormat, filterParams));
        }
        public  Task<bool> ExistsFmtAsync<T>(string sqlFormat, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.ExistsFmtAsync<T>(default(CancellationToken), sqlFormat, filterParams));
        }

        /// <summary>
        /// Returns results from an arbitrary SqlExpression. E.g:
        /// <para>db.SqlList&lt;Person&gt;(db.From&lt;Person&gt;().Select("*").Where(q => q.Age &lt; 50))</para>
        /// </summary>
        public  Task<List<T>> SqlListAsync<T>(ISqlExpression sqlExpression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlListAsync<T>(sqlExpression.ToSelectStatement(), sqlExpression.Params, token));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", new { age = 50 })</para>
        /// </summary>
        public  Task<List<T>> SqlListAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlListAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", new { age = 50 })</para>
        /// </summary>
        public  Task<List<T>> SqlListAsync<T>(string sql, object anonType = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlListAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", new Dictionary&lt;string, object&gt; { { "age", 42 } })</para>
        /// </summary>
        public  Task<List<T>> SqlListAsync<T>(string sql, Dictionary<string, object> dict, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlListAsync<T>(sql, dict, token));
        }

        /// <summary>
        /// Returns results from an arbitrary parameterized raw sql query with a dbCmd filter. E.g:
        /// <para>db.SqlList&lt;Person&gt;("EXEC GetRockstarsAged @age", dbCmd => ...)</para>
        /// </summary>
        public  Task<List<T>> SqlListAsync<T>(string sql, Action<IDbCommand> dbCmdFilter, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlListAsync<T>(sql, dbCmdFilter, token));
        }

        /// <summary>
        /// Returns the first column in a List using an SqlExpression. E.g:
        /// <para>db.SqlColumn&lt;string&gt;(db.From&lt;Person&gt;().Select(x => x.LastName).Where(q => q.Age < 50))</para>
        /// </summary>
        public  Task<List<T>> SqlColumnAsync<T>(ISqlExpression sqlExpression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumnAsync<T>(sqlExpression.ToSelectStatement(), sqlExpression.Params, token));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumn&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        public  Task<List<T>> SqlColumnAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumnAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumn&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        public  Task<List<T>> SqlColumnAsync<T>(string sql, object anonType = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumnAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns the first column in a List using a parameterized query. E.g:
        /// <para>db.SqlColumn&lt;string&gt;("SELECT LastName FROM Person WHERE Age &lt; @age", new Dictionary&lt;string, object&gt; { { "age", 50 } })</para>
        /// </summary>
        public  Task<List<T>> SqlColumnAsync<T>(string sql, Dictionary<string, object> dict, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlColumnAsync<T>(sql, dict, token));
        }

        /// <summary>
        /// Returns a single Scalar value using an SqlExpression. E.g:
        /// <para>db.SqlScalar&lt;int&gt;(db.From&lt;Person&gt;().Select(Sql.Count("*")).Where(q => q.Age &lt; 50))</para>
        /// </summary>
        public  Task<T> SqlScalarAsync<T>(ISqlExpression sqlExpression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalarAsync<T>(sqlExpression.ToSelectStatement(), sqlExpression.Params, token));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new[] { db.CreateParam("age",50) })</para>
        /// </summary>
        public  Task<T> SqlScalarAsync<T>(string sql, IEnumerable<IDbDataParameter> sqlParams, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalarAsync<T>(sql, sqlParams, token));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new { age = 50 })</para>
        /// </summary>
        public  Task<T> SqlScalarAsync<T>(string sql, object anonType = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalarAsync<T>(sql, anonType, token));
        }

        /// <summary>
        /// Returns a single Scalar value using a parameterized query. E.g:
        /// <para>db.SqlScalar&lt;int&gt;("SELECT COUNT(*) FROM Person WHERE Age &lt; @age", new Dictionary&lt;string, object&gt; { { "age", 50 } })</para>
        /// </summary>
        public  Task<T> SqlScalarAsync<T>(string sql, Dictionary<string, object> dict, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlScalarAsync<T>(sql, dict, token));
        }

        /// <summary>
        /// Executes a raw sql non-query using sql. E.g:
        /// <para>var rowsAffected = db.ExecuteNonQueryAsync("UPDATE Person SET LastName={0} WHERE Id={1}".SqlFormat("WaterHouse", 7))</para>
        /// </summary>
        /// <returns>number of rows affected</returns>
        public  Task<int> ExecuteNonQueryAsync(string sql, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQueryAsync(sql, null, token));
        }

        /// <summary>
        /// Executes a raw sql non-query using a parameterized query. E.g:
        /// <para>var rowsAffected = db.ExecuteNonQueryAsync("UPDATE Person SET LastName=@name WHERE Id=@id", new { name = "WaterHouse", id = 7 })</para>
        /// </summary>
        /// <returns>number of rows affected</returns>
        public  Task<int> ExecuteNonQueryAsync(string sql, object anonType, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQueryAsync(sql, anonType, token));
        }

        /// <summary>
        /// Executes a raw sql non-query using a parameterized query.
        /// </summary>
        /// <returns>number of rows affected</returns>
        public  Task<int> ExecuteNonQueryAsync(string sql, Dictionary<string, object> dict, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecNonQueryAsync(sql, dict, token));
        }

        /// <summary>
        /// Returns results from a Stored Procedure, using a parameterized query.
        /// </summary>
        public  Task<List<TOutputModel>> SqlProcedureAsync<TOutputModel>(object anonType, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlProcedureAsync<TOutputModel>(anonType, token));
        }

        /// <summary>
        /// Returns results from a Stored Procedure using an SqlFormat query. E.g:
        /// <para></para>
        /// </summary>
        public  Task<List<TOutputModel>> SqlProcedureFmtAsync<TOutputModel>(CancellationToken token,
            object anonType,
            string sqlFilter,
            params object[] filterParams)
            where TOutputModel : new()
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlProcedureFmtAsync<TOutputModel>(token,
                anonType, sqlFilter, filterParams));
        }

        /// <summary>
        /// Returns the scalar result as a long.
        /// </summary>
        public  Task<long> LongScalarAsync(CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecLongScalarAsync(null, token));
        }

        /// <summary>
        /// Returns the first result with all its references loaded, using a primary key id. E.g:
        /// <para>db.LoadSingleById&lt;Person&gt;(1)</para>
        /// </summary>
        public  Task<T> LoadSingleByIdAsync<T>(object idValue, string[] include = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSingleByIdAsync<T>(idValue, include, token));
        }

        /// <summary>
        /// Returns the first result with all its references loaded, using a primary key id. E.g:
        /// <para>db.LoadSingleById&lt;Person&gt;(1, include = x => new{ x.Address })</para>
        /// </summary>
        public  Task<T> LoadSingleByIdAsync<T>(object idValue, Func<T, object> include, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSingleByIdAsync<T>(idValue, include(typeof(T).CreateInstance<T>()).GetType().AllAnonFields(), token));
        }

        /// <summary>
        /// Loads all the related references onto the instance. E.g:
        /// <para>db.LoadReferencesAsync(customer)</para> 
        /// </summary>
        public  Task LoadReferencesAsync<T>(T instance, string[] include = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadReferencesAsync(instance, include, token));
        }
#endif
        #endregion

        #region OrmLiteReadExpressionsApiAsync
#if NET45
        /// <summary>
        /// Returns results from using a LINQ Expression. E.g:
        /// <para>db.Select&lt;Person&gt;(x =&gt; x.Age &gt; 40)</para>
        /// </summary>
        public  Task<List<T>> SelectAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync(predicate, token));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Select&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public  Task<List<T>> SelectAsync<T>(Func<SqlExpression<T>, SqlExpression<T>> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync(expression, token));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.Select(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public  Task<List<T>> SelectAsync<T>(SqlExpression<T> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync(expression, token));
        }

        /// <summary>
        /// Project results from a number of joined tables into a different model
        /// </summary>
        public  Task<List<Into>> SelectAsync<Into, From>(SqlExpression<From> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<Into, From>(expression, token));
        }

        /// <summary>
        /// Project results from a number of joined tables into a different model
        /// </summary>
        public  Task<List<Into>> SelectAsync<Into, From>(Func<SqlExpression<From>, SqlExpression<From>> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SelectAsync<Into, From>(expression, token));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.SelectAsync(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public  Task<List<T>> SelectAsync<T>(ISqlExpression expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SqlListAsync<T>(expression.SelectInto<T>(), expression.Params, token));
        }

        /// <summary>
        /// Returns a single result from using a LINQ Expression. E.g:
        /// <para>db.Single&lt;Person&gt;(x =&gt; x.Age == 42)</para>
        /// </summary>
        public  Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleAsync(predicate, token));
        }

        /// <summary>
        /// Returns a single result from using an SqlExpression lambda. E.g:
        /// <para>db.Single&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age == 42))</para>
        /// </summary>
        public  Task<T> SingleAsync<T>(Func<SqlExpression<T>, SqlExpression<T>> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleAsync(expression, token));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.SingleAsync&lt;Person&gt;(x =&gt; x.Age &gt; 40)</para>
        /// </summary>
        public  Task<T> SingleAsync<T>(SqlExpression<T> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleAsync(expression, token));
        }

        /// <summary>
        /// Returns results from using an SqlExpression lambda. E.g:
        /// <para>db.SingleAsync(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public  Task<T> SingleAsync<T>(ISqlExpression expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SingleAsync<T>(expression.SelectInto<T>(), expression.Params, token));
        }

        /// <summary>
        /// Returns a scalar result from using an SqlExpression lambda. E.g:
        /// <para>db.Scalar&lt;Person, int&gt;(x =&gt; Sql.Max(x.Age))</para>
        /// </summary>
        public  Task<TKey> ScalarAsync<T, TKey>(Expression<Func<T, TKey>> field, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ScalarAsync(field, token));
        }


        /// <summary>
        /// Returns the count of rows that match the LINQ expression, E.g:
        /// <para>db.Count&lt;Person&gt;(x =&gt; x.Age &lt; 50)</para>
        /// </summary>
        public  Task<long> CountAsync<T>(Expression<Func<T, bool>> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.CountAsync(expression, token));
        }

        /// <summary>
        /// Returns the count of rows that match the SqlExpression lambda, E.g:
        /// <para>db.Count&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        public  Task<long> CountAsync<T>(Func<SqlExpression<T>, SqlExpression<T>> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.CountAsync(expression, token));
        }

        /// <summary>
        /// Returns the count of rows that match the supplied SqlExpression, E.g:
        /// <para>db.Count(db.From&lt;Person&gt;().Where(x =&gt; x.Age &lt; 50))</para>
        /// </summary>
        public  Task<long> CountAsync<T>(SqlExpression<T> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.CountAsync(expression, token));
        }

        public  Task<long> CountAsync<T>(CancellationToken token = default(CancellationToken))
        {
            var expression = dbConn.GetDialectProvider().SqlExpression<T>();
            return dbConn.Exec(dbCmd => dbCmd.CountAsync(expression, token));
        }

        /// <summary>
        /// Return the number of rows returned by the supplied expression
        /// </summary>
        public  Task<long> RowCountAsync<T>(SqlExpression<T> expression, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.RowCountAsync(expression, token));
        }

        /// <summary>
        /// Return the number of rows returned by the supplied sql
        /// </summary>
        public  Task<long> RowCountAsync(string sql, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.RowCountAsync(sql, token));
        }

        /// <summary>
        /// Returns results with references from using a LINQ Expression. E.g:
        /// <para>db.LoadSelectAsync&lt;Person&gt;(x =&gt; x.Age &gt; 40)</para>
        /// </summary>
        public  Task<List<T>> LoadSelectAsync<T>(Expression<Func<T, bool>> predicate, string[] include = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelectAsync(predicate, include, token));
        }

        /// <summary>
        /// Returns results with references from using an SqlExpression lambda. E.g:
        /// <para>db.LoadSelectAsync&lt;Person&gt;(q =&gt; q.Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public  Task<List<T>> LoadSelectAsync<T>(Func<SqlExpression<T>, SqlExpression<T>> expression, string[] include = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelectAsync(expression, include, token));
        }

        /// <summary>
        /// Returns results with references from using an SqlExpression lambda. E.g:
        /// <para>db.LoadSelectAsync(db.From&lt;Person&gt;().Where(x =&gt; x.Age &gt; 40))</para>
        /// </summary>
        public  Task<List<T>> LoadSelectAsync<T>(SqlExpression<T> expression, string[] include = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelectAsync(expression, include, token));
        }

        /// <summary>
        /// Project results with references from a number of joined tables into a different model
        /// </summary>
        public  Task<List<Into>> LoadSelectAsync<Into, From>(SqlExpression<From> expression, string[] include = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.LoadSelectAsync<Into, From>(expression, include, token));
        }
#endif
        #endregion

        #region OrmLiteWriteApiAsync
#if NET45
        /// <summary>
        /// Execute any arbitrary raw SQL.
        /// </summary>
        /// <returns>number of rows affected</returns>
        public  Task<int> ExecuteSqlAsync(string sql, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteSqlAsync(sql, token));
        }

        /// <summary>
        /// Execute any arbitrary raw SQL with db params.
        /// </summary>
        /// <returns>number of rows affected</returns>
        public  Task<int> ExecuteSqlAsync(string sql, object dbParams, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteSqlAsync(sql, dbParams, token));
        }

        /// <summary>
        /// Insert 1 POCO, use selectIdentity to retrieve the last insert AutoIncrement id (if any). E.g:
        /// <para>var id = db.Insert(new Person { Id = 1, FirstName = "Jimi }, selectIdentity:true)</para>
        /// </summary>
        public  Task<long> InsertAsync<T>(T obj, bool selectIdentity = false, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAsync(obj, selectIdentity, token));
        }

        /// <summary>
        /// Insert 1 or more POCOs in a transaction. E.g:
        /// <para>db.Insert(new Person { Id = 1, FirstName = "Tupac", LastName = "Shakur", Age = 25 },</para>
        /// <para>          new Person { Id = 2, FirstName = "Biggie", LastName = "Smalls", Age = 24 })</para>
        /// </summary>
        public  Task InsertAsync<T>(CancellationToken token, params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAsync(token, objs));
        }
        public  Task InsertAsync<T>(params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAsync(default(CancellationToken), objs));
        }

        /// <summary>
        /// Insert a collection of POCOs in a transaction. E.g:
        /// <para>db.InsertAll(new[] { new Person { Id = 9, FirstName = "Biggie", LastName = "Smalls", Age = 24 } })</para>
        /// </summary>
        public  Task InsertAllAsync<T>(IEnumerable<T> objs, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertAllAsync(objs, token));
        }

        /// <summary>
        /// Updates 1 POCO. All fields are updated except for the PrimaryKey which is used as the identity selector. E.g:
        /// <para>db.Update(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
        /// </summary>
        public  Task<int> UpdateAsync<T>(T obj, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(obj, token));
        }

        /// <summary>
        /// Updates 1 or more POCOs in a transaction. E.g:
        /// <para>db.Update(new Person { Id = 1, FirstName = "Tupac", LastName = "Shakur", Age = 25 },</para>
        /// <para>new Person { Id = 2, FirstName = "Biggie", LastName = "Smalls", Age = 24 })</para>
        /// </summary>
        public  Task<int> UpdateAsync<T>(CancellationToken token, params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(objs, token));
        }
        public  Task<int> UpdateAsync<T>(params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(default(CancellationToken), objs));
        }

        /// <summary>
        /// Updates 1 or more POCOs in a transaction. E.g:
        /// <para>db.UpdateAll(new[] { new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 } })</para>
        /// </summary>
        public  Task<int> UpdateAllAsync<T>(IEnumerable<T> objs, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAllAsync(objs, token));
        }

        /// <summary>
        /// Delete rows using an anonymous type filter. E.g:
        /// <para>db.Delete&lt;Person&gt;(new { FirstName = "Jimi", Age = 27 })</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public  Task<int> DeleteAsync<T>(object anonFilter, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync<T>(anonFilter, token));
        }

        /// <summary>
        /// Delete 1 row using all fields in the filter. E.g:
        /// <para>db.Delete(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public  Task<int> DeleteAsync<T>(T allFieldsFilter, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(allFieldsFilter, token));
        }

        /// <summary>
        /// Delete 1 or more rows in a transaction using all fields in the filter. E.g:
        /// <para>db.Delete(new Person { Id = 1, FirstName = "Jimi", LastName = "Hendrix", Age = 27 })</para>
        /// </summary>
        public  Task<int> DeleteAsync<T>(CancellationToken token = default(CancellationToken), params T[] allFieldsFilters)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(token, allFieldsFilters));
        }
        public  Task<int> DeleteAsync<T>(params T[] allFieldsFilters)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(default(CancellationToken), allFieldsFilters));
        }

        /// <summary>
        /// Delete 1 or more rows using only field with non-default values in the filter. E.g:
        /// <para>db.DeleteNonDefaults(new Person { FirstName = "Jimi", Age = 27 })</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public  Task<int> DeleteNonDefaultsAsync<T>(T nonDefaultsFilter, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteNonDefaultsAsync(nonDefaultsFilter, token));
        }

        /// <summary>
        /// Delete 1 or more rows in a transaction using only field with non-default values in the filter. E.g:
        /// <para>db.DeleteNonDefaults(new Person { FirstName = "Jimi", Age = 27 }, 
        /// new Person { FirstName = "Janis", Age = 27 })</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public  Task<int> DeleteNonDefaultsAsync<T>(CancellationToken token, params T[] nonDefaultsFilters)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteNonDefaultsAsync(token, nonDefaultsFilters));
        }
        public  Task<int> DeleteNonDefaultsAsync<T>(params T[] nonDefaultsFilters)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteNonDefaultsAsync(default(CancellationToken), nonDefaultsFilters));
        }

        /// <summary>
        /// Delete 1 row by the PrimaryKey. E.g:
        /// <para>db.DeleteById&lt;Person&gt;(1)</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public  Task<int> DeleteByIdAsync<T>(object id, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteByIdAsync<T>(id, token));
        }

        /// <summary>
        /// Delete 1 row by the PrimaryKey where the rowVersion matches the optimistic concurrency field. 
        /// Will throw <exception cref="OptimisticConcurrencyException">RowModefiedExeption</exception> if the 
        /// row does not exist or has a different row version.
        /// E.g: <para>db.DeleteById&lt;Person&gt;(1)</para>
        /// </summary>
        public  Task DeleteByIdAsync<T>(object id, ulong rowVersion, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteByIdAsync<T>(id, rowVersion, token));
        }

        /// <summary>
        /// Delete all rows identified by the PrimaryKeys. E.g:
        /// <para>db.DeleteById&lt;Person&gt;(new[] { 1, 2, 3 })</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public  Task<int> DeleteByIdsAsync<T>(IEnumerable idValues, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteByIdsAsync<T>(idValues, token));
        }

        /// <summary>
        /// Delete all rows in the generic table type. E.g:
        /// <para>db.DeleteAll&lt;Person&gt;()</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public  Task<int> DeleteAllAsync<T>(CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAllAsync<T>(token));
        }

        /// <summary>
        /// Delete all rows in the runtime table type. E.g:
        /// <para>db.DeleteAll(typeof(Person))</para>
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public  Task<int> DeleteAll(Type tableType, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAllAsync(tableType, token));
        }

        /// <summary>
        /// Delete rows using a SqlFormat filter. E.g:
        /// </summary>
        /// <returns>number of rows deleted</returns>
        public  Task<int> DeleteFmtAsync<T>(CancellationToken token, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync<T>(token, sqlFilter, filterParams));
        }
        public  Task<int> DeleteFmtAsync<T>(string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync<T>(default(CancellationToken), sqlFilter, filterParams));
        }

        /// <summary>
        /// Delete rows from the runtime table type using a SqlFormat filter. E.g:
        /// </summary>
        /// <para>db.DeleteFmt(typeof(Person), "Age = {0}", 27)</para>
        /// <returns>number of rows deleted</returns>
        public  Task<int> DeleteFmtAsync(CancellationToken token, Type tableType, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync(token, tableType, sqlFilter, filterParams));
        }
        public  Task<int> DeleteFmtAsync(Type tableType, string sqlFilter, params object[] filterParams)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync(default(CancellationToken), tableType, sqlFilter, filterParams));
        }

        /// <summary>
        /// Insert a new row or update existing row. Returns true if a new row was inserted. 
        /// Optional references param decides whether to save all related references as well. E.g:
        /// <para>db.SaveAsync(customer, references:true)</para>
        /// </summary>
        /// <returns>true if a row was inserted; false if it was updated</returns>
        public  async Task<bool> SaveAsync<T>(T obj, bool references = false, CancellationToken token = default(CancellationToken))
        {
            if (!references)
                return await dbConn.Exec(dbCmd => dbCmd.SaveAsync(obj, token));

            return await dbConn.Exec(async dbCmd =>
            {
                var ret = await dbCmd.SaveAsync(obj, token);
                await dbCmd.SaveAllReferencesAsync(obj, token);
                return ret;
            });
        }

        /// <summary>
        /// Insert new rows or update existing rows. Return number of rows added E.g:
        /// <para>db.SaveAsync(new Person { Id = 10, FirstName = "Amy", LastName = "Winehouse", Age = 27 })</para>
        /// </summary>
        /// <returns>number of rows added</returns>
        public  Task<int> SaveAsync<T>(CancellationToken token, params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveAsync(token, objs));
        }
        public  Task<int> SaveAsync<T>(params T[] objs)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveAsync(default(CancellationToken), objs));
        }

        /// <summary>
        /// Insert new rows or update existing rows. Return number of rows added E.g:
        /// <para>db.SaveAllAsync(new [] { new Person { Id = 10, FirstName = "Amy", LastName = "Winehouse", Age = 27 } })</para>
        /// </summary>
        /// <returns>number of rows added</returns>
        public  Task<int> SaveAllAsync<T>(IEnumerable<T> objs, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveAllAsync(objs, token));
        }

        /// <summary>
        /// Populates all related references on the instance with its primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveAllReferences(customer)</para> 
        /// </summary>
        public  Task SaveAllReferencesAsync<T>(T instance, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveAllReferencesAsync(instance, token));
        }

        /// <summary>
        /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveReference(customer, customer.Orders)</para> 
        /// </summary>
        public  Task SaveReferencesAsync<T, TRef>(CancellationToken token, T instance, params TRef[] refs)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveReferencesAsync(token, instance, refs));
        }
        public  Task SaveReferencesAsync<T, TRef>(T instance, params TRef[] refs)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveReferencesAsync(default(CancellationToken), instance, refs));
        }

        /// <summary>
        /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveReference(customer, customer.Orders)</para> 
        /// </summary>
        public  Task SaveReferencesAsync<T, TRef>(T instance, List<TRef> refs, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveReferencesAsync(token, instance, refs.ToArray()));
        }

        /// <summary>
        /// Populates the related references with the instance primary key and saves them. Uses '(T)Id' naming convention. E.g:
        /// <para>db.SaveReferences(customer, customer.Orders)</para> 
        /// </summary>
        public  Task SaveReferencesAsync<T, TRef>(T instance, IEnumerable<TRef> refs, CancellationToken token)
        {
            return dbConn.Exec(dbCmd => dbCmd.SaveReferencesAsync(token, instance, refs.ToArray()));
        }

        // Procedures
        public  Task ExecuteProcedureAsync<T>(T obj, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.ExecuteProcedureAsync(obj, token));
        }
#endif
        #endregion

        #region  OrmLiteReadExpressionsApiAsync
#if NET45
        /// <summary>
        /// Use an SqlExpression to select which fields to update and construct the where expression, E.g: 
        /// 
        ///   db.UpdateOnly(new Person { FirstName = "JJ" }, ev => ev.Update(p => p.FirstName).Where(x => x.FirstName == "Jimi"));
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("FirstName" = 'Jimi')
        /// 
        ///   What's not in the update expression doesn't get updated. No where expression updates all rows. E.g:
        /// 
        ///   db.UpdateOnly(new Person { FirstName = "JJ", LastName = "Hendo" }, ev => ev.Update(p => p.FirstName));
        ///   UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        public  Task<int> UpdateOnlyAsync<T>(T model, Func<SqlExpression<T>, SqlExpression<T>> onlyFields, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyAsync(model, onlyFields, token));
        }

        /// <summary>
        /// Use an SqlExpression to select which fields to update and construct the where expression, E.g: 
        /// 
        ///   var q = db.From&gt;Person&lt;());
        ///   db.UpdateOnly(new Person { FirstName = "JJ" }, q.Update(p => p.FirstName).Where(x => x.FirstName == "Jimi"));
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("FirstName" = 'Jimi')
        /// 
        ///   What's not in the update expression doesn't get updated. No where expression updates all rows. E.g:
        /// 
        ///   db.UpdateOnly(new Person { FirstName = "JJ", LastName = "Hendo" }, ev.Update(p => p.FirstName));
        ///   UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        public  Task<int> UpdateOnlyAsync<T>(T model, SqlExpression<T> onlyFields, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyAsync(model, onlyFields, token));
        }

        /// <summary>
        /// Update record, updating only fields specified in updateOnly that matches the where condition (if any), E.g:
        /// 
        ///   db.UpdateOnly(new Person { FirstName = "JJ" }, p => p.FirstName, p => p.LastName == "Hendrix");
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        ///
        ///   db.UpdateOnly(new Person { FirstName = "JJ" }, p => p.FirstName);
        ///   UPDATE "Person" SET "FirstName" = 'JJ'
        /// </summary>
        public  Task<int> UpdateOnlyAsync<T, TKey>(T obj,
            Expression<Func<T, TKey>> onlyFields = null,
            Expression<Func<T, bool>> where = null,
            CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateOnlyAsync(obj, onlyFields, where, token));
        }

        /// <summary>
        /// Updates all non-default values set on item matching the where condition (if any). E.g
        /// 
        ///   db.UpdateNonDefaults(new Person { FirstName = "JJ" }, p => p.FirstName == "Jimi");
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("FirstName" = 'Jimi')
        /// </summary>
        public  Task<int> UpdateNonDefaultsAsync<T>(T item, Expression<Func<T, bool>> obj, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateNonDefaultsAsync(item, obj, token));
        }

        /// <summary>
        /// Updates all values set on item matching the where condition (if any). E.g
        /// 
        ///   db.Update(new Person { Id = 1, FirstName = "JJ" }, p => p.LastName == "Hendrix");
        ///   UPDATE "Person" SET "Id" = 1,"FirstName" = 'JJ',"LastName" = NULL,"Age" = 0 WHERE ("LastName" = 'Hendrix')
        /// </summary>
        public  Task<int> UpdateAsync<T>(T item, Expression<Func<T, bool>> where, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(item, where, token));
        }

        /// <summary>
        /// Updates all matching fields populated on anonymousType that matches where condition (if any). E.g:
        /// 
        ///   db.Update&lt;Person&gt;(new { FirstName = "JJ" }, p => p.LastName == "Hendrix");
        ///   UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        /// </summary>
        public  Task<int> UpdateAsync<T>(object updateOnly, Expression<Func<T, bool>> where = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateAsync(updateOnly, where, token));
        }

        /// <summary>
        /// Flexible Update method to succinctly execute a free-text update statement using optional params. E.g:
        /// 
        ///   db.Update&lt;Person&gt;(set:"FirstName = {0}".Params("JJ"), where:"LastName = {0}".Params("Hendrix"));
        ///   UPDATE "Person" SET FirstName = 'JJ' WHERE LastName = 'Hendrix'
        /// </summary>
        public  Task<int> UpdateFmtAsync<T>(string set = null, string where = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateFmtAsync<T>(set, where, token));
        }

        /// <summary>
        /// Flexible Update method to succinctly execute a free-text update statement using optional params. E.g.
        /// 
        ///   db.Update(table:"Person", set: "FirstName = {0}".Params("JJ"), where: "LastName = {0}".Params("Hendrix"));
        ///   UPDATE "Person" SET FirstName = 'JJ' WHERE LastName = 'Hendrix'
        /// </summary>
        public  Task<int> UpdateFmtAsync(string table = null, string set = null, string where = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.UpdateFmtAsync(table, set, where, token));
        }

        /// <summary>
        /// Insert only fields in POCO specified by the SqlExpression lambda. E.g:
        /// <para>db.InsertOnly(new Person { FirstName = "Amy", Age = 27 }, q =&gt; q.Insert(p =&gt; new { p.FirstName, p.Age }))</para>
        /// </summary>
        public  Task InsertOnlyAsync<T>(T obj, Func<SqlExpression<T>, SqlExpression<T>> onlyFields, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertOnlyAsync(obj, onlyFields, token));
        }

        /// <summary>
        /// Using an SqlExpression to only Insert the fields specified, e.g:
        /// 
        ///   db.InsertOnly(new Person { FirstName = "Amy" }, q => q.Insert(p => new { p.FirstName }));
        ///   INSERT INTO "Person" ("FirstName") VALUES ('Amy');
        /// </summary>
        public  Task InsertOnlyAsync<T>(T obj, SqlExpression<T> onlyFields, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.InsertOnlyAsync(obj, onlyFields, token));
        }

        /// <summary>
        /// Delete the rows that matches the where expression, e.g:
        /// 
        ///   db.Delete&lt;Person&gt;(p => p.Age == 27);
        ///   DELETE FROM "Person" WHERE ("Age" = 27)
        /// </summary>
        public  Task<int> DeleteAsync<T>(Expression<Func<T, bool>> where, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(where, token));
        }

        /// <summary>
        /// Delete the rows that matches the where expression, e.g:
        /// 
        ///   db.Delete&lt;Person&gt;(ev => ev.Where(p => p.Age == 27));
        ///   DELETE FROM "Person" WHERE ("Age" = 27)
        /// </summary>
        public  Task<int> DeleteAsync<T>(Func<SqlExpression<T>, SqlExpression<T>> where, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(where, token));
        }

        /// <summary>
        /// Delete the rows that matches the where expression, e.g:
        /// 
        ///   var q = db.From&gt;Person&lt;());
        ///   db.Delete&lt;Person&gt;(q.Where(p => p.Age == 27));
        ///   DELETE FROM "Person" WHERE ("Age" = 27)
        /// </summary>
        public  Task<int> DeleteAsync<T>(SqlExpression<T> where, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteAsync(where, token));
        }

        /// <summary>
        /// Flexible Delete method to succinctly execute a delete statement using free-text where expression. E.g.
        /// 
        ///   db.Delete&lt;Person&gt;(where:"Age = {0}".Params(27));
        ///   DELETE FROM "Person" WHERE Age = 27
        /// </summary>
        public  Task<int> DeleteFmtAsync<T>(string where, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync<T>(where, token));
        }
        public  Task<int> DeleteFmtAsync<T>(string where = null)
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync<T>(where, default(CancellationToken)));
        }

        /// <summary>
        /// Flexible Delete method to succinctly execute a delete statement using free-text where expression. E.g.
        /// 
        ///   db.Delete(table:"Person", where: "Age = {0}".Params(27));
        ///   DELETE FROM "Person" WHERE Age = 27
        /// </summary>
        public  Task<int> DeleteFmtAsync(string table = null, string where = null, CancellationToken token = default(CancellationToken))
        {
            return dbConn.Exec(dbCmd => dbCmd.DeleteFmtAsync(table, where, token));
        }
#endif
        #endregion
    }
}
