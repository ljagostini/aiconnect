using System.Data.Common;
using System.Data.SQLite;

namespace Percolore.IOConnect.Util
{
	/// <summary>
	/// Utility class for SQLite databases.
	/// </summary>
	public class SQLite
	{
		/// <summary>
		/// Gets the connection string for SQLite connection with the given values.
		/// </summary>
		/// <param name="dataSource">Data source name or the path to database.</param>
		/// <param name="failIfMissing">
		/// Indicates whether the database file must already exist.
		/// If set to false, a new database file will be created if missing.
		/// If set to true, an exception is thrown if the file does not exist.
		/// </param>
		/// <param name="licenseKey">License key for SQLiteCrypt.</param>
		/// <returns>A connection string for SQLite.</returns>
		/// <exception cref="ArgumentNullException">The datasource parameter is null or an empty string.</exception>
		public static string GetSQLiteConnectionString(string dataSource, bool failIfMissing, string licenseKey = "00000-000-0000000-00000")
		{
			if (string.IsNullOrWhiteSpace(dataSource))
				throw new ArgumentNullException(nameof(dataSource));

			return $"data source ={dataSource}; Encryption=SQLiteCrypt; SQLiteCrypt License Key={licenseKey}; FailIfMissing = {failIfMissing};";
		}

		/// <summary>
		/// Creates a new instance of <see cref="SQLiteConnection"/> class with the given connection string.
		/// </summary>
		/// <param name="connectionString">The connection string used for database connection.</param>
		/// <returns>A new instance of <see cref="SQLiteConnection"/> class.</returns>
		/// <exception cref="ArgumentNullException">The given connection string is null or an empty string.</exception>
		public static SQLiteConnection CreateSQLiteConnection(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentNullException(nameof(connectionString));

			return new SQLiteConnection(connectionString);
		}

		/// <summary>
		/// Creates a new instance of <see cref="SQLiteConnection"/> class.
		/// </summary>
		/// <param name="dataSource">Data source name or the path to database.</param>
		/// <param name="failIfMissing">
		/// Indicates whether the database file must already exist.
		/// If set to false, a new database file will be created if missing.
		/// If set to true, an exception is thrown if the file does not exist.
		/// </param>
		/// <param name="cryptLicenseKey">License key for SQLiteCrypt</param>
		/// <returns>A new instance of <see cref="SQLiteConnection"/> class.</returns>
		/// <exception cref="ArgumentNullException">The datasource parameter is null or an empty string.</exception>
		public static SQLiteConnection CreateSQLiteConnection(string dataSource, bool failIfMissing, string licenseKey = "00000-000-0000000-00000")
		{
			string connString = GetSQLiteConnectionString(dataSource, failIfMissing, licenseKey);
			return CreateSQLiteConnection(connString);
		}

		/// <summary>
		/// Checks if a column exists in a given SQLite table.
		/// </summary>
		/// <param name="connection"><see cref="SQLiteConnection"/> object.</param>
		/// <param name="tableName">The table name to check.</param>
		/// <param name="columnName">The given column name to check.</param>
		/// <returns>True, if the column exists; otherwise, false.</returns>
		public static bool IsColumnExists(SQLiteConnection connection, string tableName, string columnName)
		{
			if (connection is null)
				throw new ArgumentNullException(nameof(connection));

			if (string.IsNullOrWhiteSpace(tableName))
				throw new ArgumentException(nameof(tableName));

			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentException(nameof(columnName));

			string query = $"PRAGMA table_info({tableName});";
			var existingColumns = new HashSet<string>();

			using var command = new SQLiteCommand(query, connection);

			if (connection.State != System.Data.ConnectionState.Open)
				connection.Open();

			using var reader = command.ExecuteReader();
			
			while (reader.Read())
				existingColumns.Add(reader["name"].ToString());

			connection.Close();

			return existingColumns.Contains(columnName);
		}
	}
}