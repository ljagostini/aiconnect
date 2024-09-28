using Percolore.Core.Logging;
using System.Data.SQLite;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectFormula
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Formulas.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public int Id { get; set; }
        public string Nome { get; set; }
        public List<ObjectFormulaItem> Itens { get; set; }

        #region Contrutores

        public ObjectFormula()
        {
            Itens = new List<ObjectFormulaItem>();
        }
        #endregion

        #region Métodos públicos  

        public static void CreateBD()
        {
            try
            {
                if (!File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CREATE TABLE IF NOT EXISTS [Formula] (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Nome TEXT NULL);");

                    string createQuery = sb.ToString();

                    SQLiteConnection connectCreate = Util.SQLite.CreateSQLiteConnection(PathFile, false);
                    connectCreate.Open();
					// Open connection to create DB if not exists.
					connectCreate.Close();
                    Thread.Sleep(2000);
                    if (File.Exists(PathFile))
                    {
                        using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {
                                conn.Open();
                                cmd.CommandText = createQuery;
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                        sb = new StringBuilder();
                        sb.Append("CREATE TABLE IF NOT EXISTS [FormulaItem] (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, IdFormula INTEGER, IdCorante INTEGER, Volume TEXT NULL);");
                        createQuery = sb.ToString();
                        using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {
                                conn.Open();
                                cmd.CommandText = createQuery;
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectFormula).Name}: ", e);
			}
		}

        public static void Persist(ObjectFormula formula)
        {
            try
            {
                if (formula.Id == 0)
                {                    
                    Add(formula);
                }
                else
                {
                    Update(formula);
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectFormula).Name}: ", e);
			}
		}

        public static void Delete(int id)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        sb.Append("DELETE FROM FormulaItem WHERE IdFormula = '" + id + "'; ");
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        sb.Append("DELETE FROM Formula WHERE Id = '" + id + "'; ");
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectFormula).Name}: ", e);
                throw;
			}
		}

        public static ObjectFormula Load(int id)
        {
            ObjectFormula formula = null;
            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Formula WHERE Id = '" + id.ToString() + "';";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                formula = new ObjectFormula();
                                formula.Id = int.Parse(reader["Id"].ToString());
                                formula.Nome = reader["Nome"].ToString();
                                   
                                break;
                            }
                            reader.Close();
                        }
                    }
                    conn.Close();
                }

                if (formula != null)
                {
                    formula.Itens = new List<ObjectFormulaItem>();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();

                            cmd.CommandText = "SELECT * FROM FormulaItem WHERE IdFormula = '" + id.ToString() + "';";

                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ObjectFormulaItem item = new ObjectFormulaItem();
                                    item.IdColorante = int.Parse(reader["IdCorante"].ToString());
                                    item.Mililitros = double.Parse(reader["Volume"].ToString(),  System.Globalization.CultureInfo.InvariantCulture);
                                    item.Colorante = Util.ObjectColorante.Load(item.IdColorante);
                                    formula.Itens.Add(item);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
               
                return formula;
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectFormula).Name}: ", e);
                throw;
			}
			finally { formula = null; }
        }

        public static List<ObjectFormula> List()
        {
            List<ObjectFormula> list = new List<ObjectFormula>();

            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Formula;;";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ObjectFormula formula = new ObjectFormula();
                                formula.Id = int.Parse(reader["Id"].ToString());
                                formula.Nome = reader["Nome"].ToString();
                                list.Add(formula);
                            }
                            reader.Close();
                        }
                    }
                    conn.Close();
                }

                if (list != null && list.Count > 0)
                {
                    foreach (ObjectFormula formula in list)
                    {
                        formula.Itens = new List<ObjectFormulaItem>();
                        using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {
                                conn.Open();

                                cmd.CommandText = "SELECT * FROM FormulaItem WHERE IdFormula = '" + formula.Id.ToString() + "';";

                                using (SQLiteDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        ObjectFormulaItem item = new ObjectFormulaItem();
                                        item.IdColorante = int.Parse(reader["IdCorante"].ToString());
                                        item.Mililitros = double.Parse(reader["Volume"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                        item.Colorante = Util.ObjectColorante.Load(item.IdColorante);
                                        formula.Itens.Add(item);
                                    }
                                    reader.Close();
                                }
                            }
                            conn.Close();
                        }
                    }
                }

                return list;
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectFormula).Name}: ", e);
                throw;
			}
			finally { list = null; }
        }
        #endregion

        #region Métodos privados

        static void Add(ObjectFormula formula)
        {
            StringBuilder sb = new StringBuilder();
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    sb = new StringBuilder();
                    sb.Append("INSERT INTO Formula (Nome) VALUES (");
                    sb.Append("'" + formula.Nome + "'); ");
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT last_insert_rowid();";
                    Int64 LastRowID64 = (Int64)cmd.ExecuteScalar();
                    conn.Close();

                    //[FormulaItem] (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, IdFormula INTEGER, IdCorante INTEGER, Volume TEXT NULL);");
                    foreach (ObjectFormulaItem item in formula.Itens)
                    {
                        using (SQLiteConnection conn2 = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd2 = new SQLiteCommand(conn2))
                            {
                                conn2.Open();
                                sb = new StringBuilder();
                                sb.Append("INSERT INTO FormulaItem (IdFormula, IdCorante, Volume) VALUES (");
                                sb.Append("'" + LastRowID64 + "', ");
                                sb.Append("'" + item.IdColorante + "', ");
                                sb.Append("'" + item.Mililitros.ToString().Replace(",", ".") + "'); ");
                                cmd2.CommandText = sb.ToString();
                                cmd2.ExecuteNonQuery();
                                conn2.Close();
                            }
                        }
                    }
                }
            }
        }

        static void Update(ObjectFormula formula)
        {
            StringBuilder sb = new StringBuilder();

            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    sb.Append("UPDATE Formula SET Nome = '" + formula.Nome + "' WHERE Id = '" + formula.Id.ToString()  + "'; ");
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();                    
                    conn.Close();
                }
            }

            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    sb.Append("DELETE FROM FormulaItem WHERE IdFormula = '" + formula.Id + "'; ");
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    //[FormulaItem] (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, IdFormula INTEGER, IdCorante INTEGER, Volume TEXT NULL);");
                    foreach (ObjectFormulaItem item in formula.Itens)
                    {
                        using (SQLiteConnection conn2 = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd2 = new SQLiteCommand(conn2))
                            {
                                conn2.Open();
                                sb.Append("INSERT INTO FormulaItem (IdFormula, IdCorante, Volume) VALUES (");
                                sb.Append("'" + formula.Id + "', ");
                                sb.Append("'" + item.IdColorante + "', ");
                                sb.Append("'" + item.Mililitros.ToString().Replace(",", ".") + "'); ");
                                cmd2.CommandText = sb.ToString();
                                cmd2.ExecuteNonQuery();
                                conn2.Close();
                            }
                        }
                    }
                }
            }
        }

        #endregion

    }
}