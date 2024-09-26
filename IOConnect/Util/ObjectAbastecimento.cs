using System.Data.SQLite;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectAbastecimento
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Abastecimento.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Conteudo { get; set; }
        public int UnMed { get; set; }

        public ObjectAbastecimento()
        {

        }

        #region Métodos
        public static void CreateBD()
        {
            if (!File.Exists(PathFile))
            {

                SQLiteConnection connectCreate = Util.SQLite.CreateSQLiteConnection(PathFile, false);
                connectCreate.Open();
				// Open connection to create DB if not exists.
				connectCreate.Close();
                Thread.Sleep(2000);
                if (File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CREATE TABLE IF NOT EXISTS [Abastecimento] (Id INTEGER PRIMARY KEY, Nome TEXT NULL, Conteudo TEXT NULL, UnMed TEXT NULL);");
                    string createQuery = sb.ToString();
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

        public static ObjectAbastecimento Load(int id)
        {
            ObjectAbastecimento aux = null;
            
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM Abastecimento WHERE Id = " + id.ToString() + ";";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            aux = new ObjectAbastecimento();
                            aux.Id = int.Parse(reader["Id"].ToString());
                            aux.Nome = reader["Nome"].ToString();
                            aux.Conteudo = reader["Conteudo"].ToString();
                            aux.UnMed = int.Parse(reader["UnMed"].ToString());
                            break;
                        }
                    }
                }
                conn.Close();
            }

            return aux;
        }

        public static List<ObjectAbastecimento> List()
        {
            List<ObjectAbastecimento> list = new List<ObjectAbastecimento>();

            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM Abastecimento;";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ObjectAbastecimento abast = new ObjectAbastecimento();
                            abast.Id = int.Parse(reader["Id"].ToString());
                            abast.Nome = reader["Nome"].ToString();
                            abast.Conteudo = reader["Conteudo"].ToString();
                            abast.UnMed = int.Parse(reader["UnMed"].ToString());
                            list.Add(abast);
                        }
                    }
                }
                conn.Close();
            }

            return list;
        }

        public static void Persist(ObjectAbastecimento abast)
        {
            ObjectAbastecimento objc = null;
            if (abast.Id > 0)
            {
                objc = Load(abast.Id);
            }

            //Insert
            if (objc == null)
            {
                StringBuilder sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        sb.Append("INSERT INTO Abastecimento (Nome, Conteudo, UnMed) VALUES (");
                        sb.Append("'" + abast.Nome.ToString() + "', ");
                        sb.Append("'" + abast.Conteudo + "', ");
                        sb.Append("'" + abast.UnMed.ToString() + "' ");
                        sb.Append(");");

                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                }
            }
            //Update
            else
            {
                StringBuilder sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        sb.Append("UPDATE Abastecimento SET "); // (Motor, Nome, MassaEspecifica, Habilitado, Volume, Correspondencia, Dispositivo, NivelMinimo, NivelMaximo) VALUES (");
                        sb.Append("Nome = '" + abast.Nome + "', ");
                        sb.Append("Conteudo = '" + abast.Conteudo + "', ");
                        sb.Append("UnMed = '" + abast.UnMed.ToString() + "' ");
                        sb.Append(" WHERE Id = " + abast.Id.ToString() + ";");

                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                }
            }
        }

        public static bool Abastecimento_Delete(int id)
        {
            bool retorno = false;
            
            StringBuilder sb = new StringBuilder();
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    sb.Append("Delete From Abastecimento WHERE Id = " + id.ToString() + ";");
                    cmd.CommandText = sb.ToString();

                    retorno = cmd.ExecuteNonQuery() > 0;

                    conn.Close();
                }
            }

            return retorno;
        }

        #endregion
    }
}
