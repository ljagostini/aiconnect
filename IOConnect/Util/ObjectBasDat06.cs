using Percolore.Core.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Util
{
    public class ObjectBasDat06
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "BasDat06.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public string Name { get; set; }
        public double Volume { get; set; } = 0;
        public int Circuito { get; set; }
       

        public ObjectBasDat06()
        {

        }


        public static void CreateBD()
        {
            try
            {
                if (!File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CREATE TABLE IF NOT EXISTS [BasDat06] (Name TEXT NULL, Volume TEXT NULL, Circuito TEXT NULL);");

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
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat06).Name}: ", e);
			}
		}

        public static ObjectBasDat06 Load(int Circuito)
        {
            ObjectBasDat06 basDat06 = null;
            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM BasDat06 WHERE Circuito = '" + Circuito.ToString() + "';";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                basDat06 = new ObjectBasDat06();
                                basDat06.Circuito = int.Parse(reader["Circuito"].ToString());
                                basDat06.Volume = double.Parse(reader["Volume"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                basDat06.Name = reader["Name"].ToString();
                               
                                break;
                            }
                        }
                    }
                    conn.Close();
                }


                return basDat06;
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat06).Name}: ", e);
                throw;
			}
		}


        public static List<ObjectBasDat06> List()
        {
            List<ObjectBasDat06> list = new List<ObjectBasDat06>();

            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM BasDat06;";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ObjectBasDat06 basDat06 = new ObjectBasDat06();
                                basDat06.Circuito = int.Parse(reader["Circuito"].ToString());
                                basDat06.Volume = double.Parse(reader["Volume"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                basDat06.Name = reader["Name"].ToString();
                                list.Add(basDat06);
                            }
                        }
                    }
                    conn.Close();
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat06).Name}: ", e);
			}

			return list;
        }

        public static void Persist(ObjectBasDat06 bDat06)
        {
            try
            {
                ObjectBasDat06 objc = Load(bDat06.Circuito);
                //Insert
                if (objc == null)
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {

                            conn.Open();
                            sb.Append("INSERT INTO BasDat06 (Name, Volume, Circuito) VALUES (");
                            sb.Append("'" + bDat06.Name.ToString() + "', ");
                            sb.Append("'" + bDat06.Volume.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + bDat06.Circuito.ToString() + "'");
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
                            sb.Append("UPDATE BasDat06 SET "); // Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, DtInicio;
                            sb.Append("Name = '" + bDat06.Name  + "', ");
                            sb.Append("Volume = '" + bDat06.Volume.ToString().Replace(",", ".") + "' ");
                            sb.Append(" WHERE Circuito = '" + bDat06.Circuito.ToString() + "';");

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }
                }

            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat06).Name}: ", e);
                throw;
			}
		}

        public static void Persist(List<ObjectBasDat06> lista)
        {
            try
            {
                if (lista != null && lista.Count > 0)
                {
                    foreach (ObjectBasDat06 bDat06 in lista)
                    {
                        ObjectBasDat06 objc = Load(bDat06.Circuito);
                        //Insert
                        if (objc == null)
                        {
                            StringBuilder sb = new StringBuilder();
                            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                {

                                    conn.Open();
                                    sb.Append("INSERT INTO BasDat06 (Name, Volume, Circuito) VALUES (");
                                    sb.Append("'" + bDat06.Name.ToString() + "', ");
                                    sb.Append("'" + bDat06.Volume.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + bDat06.Circuito.ToString() + "'");
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
                                    sb.Append("UPDATE BasDat06 SET "); // Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, DtInicio;
                                    sb.Append("Name = '" + bDat06.Name + "', ");
                                    sb.Append("Volume = '" + bDat06.Volume.ToString().Replace(",", ".") + "' ");
                                    sb.Append(" WHERE Circuito = '" + bDat06.Circuito.ToString() + "';");

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
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat06).Name}: ", e);
                throw;
			}
		}

        public static bool Validate(List<ObjectBasDat06> lista, out string outMsg)
        {
            bool retorno = true;
            outMsg = "";
            List<Util.ObjectColorante> listC = Util.ObjectColorante.List();
            foreach (ObjectBasDat06 _rec in lista)
            {
                if (_rec.Name == "")
                {
                    outMsg += "BasDat06, Não possui Nome";
                    retorno = false;                                      
                }
                if(_rec.Volume < 0)
                {
                    outMsg += string.Format("BasDat06, Não possui Volume, circuito {0}", _rec.Circuito);
                    retorno = false;
                }
                Util.ObjectColorante _col = listC.Find(o => o.Circuito == _rec.Circuito);
                if(_col == null || !_col.Habilitado)
                {
                    outMsg += string.Format("BasDat06, Não possui Produto Habilitado, circuito {0}", _rec.Circuito);
                    retorno = false;
                }

                if (!retorno)
                {
                    break;
                }
            }

            return retorno;
        }

        public static bool Remove(ObjectBasDat06 bDat06)
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
                        sb.Append("DELETE FROM BasDat06 WHERE Circuito = '" + bDat06.Circuito.ToString() + "';");

                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        conn.Close();
                        retorno = true;
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectBasDat06).Name}: ", e);
                throw;
			}

			return retorno;
        }
    }
}