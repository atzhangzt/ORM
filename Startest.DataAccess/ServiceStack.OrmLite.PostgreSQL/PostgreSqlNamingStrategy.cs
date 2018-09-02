using ServiceStack.Text;

namespace ServiceStack.OrmLite.PostgreSQL
{
    public class PostgreSqlNamingStrategy : OrmLiteNamingStrategyBase
    {
        public override string GetTableName(string name)
        {
            return name.ToLower(); ;
        }

        public override string GetColumnName(string name)
        {
            return name.ToLower();
        }
    }
}