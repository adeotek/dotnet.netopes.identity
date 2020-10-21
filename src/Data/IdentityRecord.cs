using System.Data;
using Netopes.Core.Helpers.Database;

namespace Netopes.Identity.Data
{
    /// <summary>
    ///     A base class for all identity tables.
    /// </summary>
    public class IdentityRecord : RecordBase
    {
        /// <summary>
        ///     Creates a new instance of <see cref="IdentityRecord" />.
        /// </summary>
        /// <param name="dbConnectionFactory"></param>
        public IdentityRecord(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnection = dbConnectionFactory.Create();
            DbObjectsEscapeChar = dbConnectionFactory.DbObjectsEscapeChar;
            DbObjectsPrefix = dbConnectionFactory.DbObjectsPrefix;
            DbSchema = dbConnectionFactory.DbSchema;
            GuidConverter = dbConnectionFactory.GuidConverter;
        }

        /// <summary>
        ///     The type of the database connection class used to access the store.
        /// </summary>
        protected IDbConnection DbConnection { get; }

        /// <summary>
        ///     Database objects names escape character.
        /// </summary>
        protected string DbObjectsEscapeChar { get; set; }

        /// <summary>
        ///     Database objects names prefix (null or empty no pefix).
        /// </summary>
        protected string DbObjectsPrefix { get; set; }

        /// <summary>
        ///     Database schema name (null for default or no schema).
        /// </summary>
        protected string DbSchema { get; set; }

        /// <summary>
        ///     Database GUIDs fields values converter (null or empty for none).
        /// </summary>
        public string GuidConverter { get; set; }

        protected string TN(string name)
        {
            var escapeChar = string.IsNullOrEmpty(DbObjectsEscapeChar) ? string.Empty : DbObjectsEscapeChar;
            var schema = string.IsNullOrEmpty(DbSchema) ? string.Empty : $"{escapeChar}{DbSchema}{escapeChar}.";
            return string.IsNullOrEmpty(DbObjectsPrefix) 
                ? $"{schema}{escapeChar}{name}{escapeChar}"
                : $"{schema}{escapeChar}{DbObjectsPrefix}{name}{escapeChar}";
        }

        protected string CN(string name) => string.IsNullOrEmpty(DbObjectsEscapeChar) ? name : $"{DbObjectsEscapeChar}{name}{DbObjectsEscapeChar}";

        protected string GID(string name) => string.IsNullOrEmpty(GuidConverter) ? $"@{name}" : $"{GuidConverter}(@{name})";

        /// <inheritdoc />
        protected override void OnDispose()
        {
            DbConnection?.Dispose();
        }
    }
}