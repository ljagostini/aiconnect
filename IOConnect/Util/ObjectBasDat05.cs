using Percolore.Core.Logging;
using System.Data.SQLite;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectBasDat05
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "BasDat05.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public string Name { get; set; }
        public double Volume { get; set; } = 0;
        public int Circuito { get; set; }


        public ObjectBasDat05()
        {

        }


        public static void CreateBD()
        {
            try
            {
                if (!File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CREATE TABLE IF NOT EXISTS [BasDat05] (Name TEXT NULL, Volume TEXT NULL, Circuito TEXT NULL);");

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
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat05).Name}: ", e);
			}
		}

        public static ObjectBasDat05 Load(int Circuito)
        {
            ObjectBasDat05 basDat05 = null;
            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM BasDat05 WHERE Circuito = '" + Circuito.ToString() + "';";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                basDat05 = new ObjectBasDat05();
                                basDat05.Circuito = int.Parse(reader["Circuito"].ToString());
                                basDat05.Volume = double.Parse(reader["Volume"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                basDat05.Name = reader["Name"].ToString();

                                break;
                            }
                        }
                    }
                    conn.Close();
                }


                return basDat05;
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat05).Name}: ", e);
                throw;
			}
		}

        public static List<ObjectBasDat05> List()
        {
            List<ObjectBasDat05> list = new List<ObjectBasDat05>();

            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM BasDat05;";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ObjectBasDat05 basDat05 = new ObjectBasDat05();
                                basDat05.Circuito = int.Parse(reader["Circuito"].ToString());
                                basDat05.Volume = double.Parse(reader["Volume"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                basDat05.Name = reader["Name"].ToString();
                                list.Add(basDat05);
                            }
                        }
                    }
                    conn.Close();
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat05).Name}: ", e);
			}

			return list;
        }

        public static void Persist(ObjectBasDat05 bDat05)
        {
            try
            {
                ObjectBasDat05 objc = Load(bDat05.Circuito);
                //Insert
                if (objc == null)
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {

                            conn.Open();
                            sb.Append("INSERT INTO BasDat05 (Name, Volume, Circuito) VALUES (");
                            sb.Append("'" + bDat05.Name.ToString() + "', ");
                            sb.Append("'" + bDat05.Volume.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + bDat05.Circuito.ToString() + "'");
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
                            sb.Append("UPDATE BasDat05 SET "); // Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, DtInicio;
                            sb.Append("Name = '" + bDat05.Name + "', ");
                            sb.Append("Volume = '" + bDat05.Volume.ToString().Replace(",", ".") + "' ");
                            sb.Append(" WHERE Circuito = '" + bDat05.Circuito.ToString() + "';");

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }
                }

            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat05).Name}: ", e);
                throw;
			}
		}

        public static void Persist(List<ObjectBasDat05> lista)
        {
            try
            {
                if (lista != null && lista.Count > 0)
                {
                    foreach (ObjectBasDat05 bDat05 in lista)
                    {
                        ObjectBasDat05 objc = Load(bDat05.Circuito);
                        //Insert
                        if (objc == null)
                        {
                            StringBuilder sb = new StringBuilder();
                            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                {

                                    conn.Open();
                                    sb.Append("INSERT INTO BasDat05 (Name, Volume, Circuito) VALUES (");
                                    sb.Append("'" + bDat05.Name.ToString() + "', ");
                                    sb.Append("'" + bDat05.Volume.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + bDat05.Circuito.ToString() + "'");
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
                                    sb.Append("UPDATE BasDat05 SET "); // Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, DtInicio;
                                    sb.Append("Name = '" + bDat05.Name + "', ");
                                    sb.Append("Volume = '" + bDat05.Volume.ToString().Replace(",", ".") + "' ");
                                    sb.Append(" WHERE Circuito = '" + bDat05.Circuito.ToString() + "';");

                                    cmd.CommandText = sb.ToString();

                                    cmd.ExecuteNonQuery();

                                    conn.Close();
                                }
                            }
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat05).Name}: ", e);
                throw;
			}
		}

        public static bool Validate(List<ObjectBasDat05> lista, out string outMsg)
        {
            bool retorno = true;
            outMsg = "";
            List<Util.ObjectColorante> listC = Util.ObjectColorante.List();
            foreach (ObjectBasDat05 _rec in lista)
            {
                if (_rec.Name == "")
                {
                    outMsg += "BasDat05, Não possui Nome";
                    retorno = false;
                }
                if (_rec.Volume < 0)
                {
                    outMsg += string.Format("BasDat05, Não possui Volume, circuito {0}", _rec.Circuito);
                    retorno = false;
                }
                Util.ObjectColorante _col = listC.Find(o => o.Circuito == _rec.Circuito);
                if (_col == null || !_col.Habilitado)
                {
                    outMsg += string.Format("BasDat05, Não possui Produto Habilitado, circuito {0}", _rec.Circuito);
                    retorno = false;
                }

                if (!retorno)
                {
                    break;
                }
            }

            return retorno;
        }

        public static bool Remove(ObjectBasDat05 bDat05)
        {
            bool retorno = false;
            try
            {
                StringBuilder sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        sb.Append("DELETE FROM BasDat05 WHERE Circuito = '" + bDat05.Circuito.ToString() + "';");

                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        conn.Close();
                        retorno = true;
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat05).Name}: ", e);
                throw;
			}

			return retorno;
        }
    }
}