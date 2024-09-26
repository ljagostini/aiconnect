using System.Data.SQLite;
using System.Text;

namespace Percolore.IOConnect.Util
{
	//Classe mantida para atualizar as antigas recirculações automáticas para a Classe ObjectRecircular
	public class ObjectRecircularAuto
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "RecircularAuto.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public int Circuito { get; set; }
        public bool Habilitado { get; set; } = true;
        public double VolumeRecircular { get; set; } = 0;
        public double VolumeDosado { get; set; } = 0;
        public DateTime DtInicio { get; set; } = DateTime.Now;

        public ObjectColorante _colorante;

        public ObjectRecircularAuto()
        {

        }

        public static void CreateBD()
        {
            if (!File.Exists(PathFile))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("CREATE TABLE IF NOT EXISTS [RecircularAuto] (Circuito TEXT NULL, Habilitado TEXT NULL, VolumeRecircular TEXT NULL, VolumeDosado TEXT NULL, DtInicio TEXT NULL);");

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

        public static ObjectRecircularAuto Load(int Circuito)
        {
            ObjectRecircularAuto recircular = null;
            
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM RecircularAuto WHERE Circuito = '" + Circuito.ToString() + "';";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recircular = new ObjectRecircularAuto();
                            recircular.Circuito = int.Parse(reader["Circuito"].ToString());
                            recircular.Habilitado = Convert.ToBoolean(reader["Habilitado"].ToString());
                            recircular.VolumeRecircular = double.Parse(reader["VolumeRecircular"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            recircular.VolumeDosado = double.Parse(reader["VolumeDosado"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            recircular.DtInicio = Convert.ToDateTime(reader["DtInicio"].ToString());
                            recircular._colorante = Util.ObjectColorante.Load(recircular.Circuito);
                            if (recircular._colorante != null && !recircular._colorante.Habilitado)
                            {
                                recircular.Habilitado = false;
                            }
                            break;
                        }
                    }
                }
                conn.Close();
            }

            return recircular;
        }

        public static List<ObjectRecircularAuto> List()
        {
            List<ObjectRecircularAuto> list = new List<ObjectRecircularAuto>();

            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM RecircularAuto;";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ObjectRecircularAuto recircular = new ObjectRecircularAuto();
                            recircular.Circuito = int.Parse(reader["Circuito"].ToString());
                            recircular.Habilitado = Convert.ToBoolean(reader["Habilitado"].ToString());
                            recircular.VolumeRecircular = double.Parse(reader["VolumeRecircular"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            recircular.VolumeDosado = double.Parse(reader["VolumeDosado"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            recircular.DtInicio = Convert.ToDateTime(reader["DtInicio"].ToString());
                            recircular._colorante = Util.ObjectColorante.Load(recircular.Circuito);
                            if (recircular._colorante != null && !recircular._colorante.Habilitado)
                            {
                                recircular.Habilitado = false;
                            }
                            list.Add(recircular);
                        }
                    }
                }
                conn.Close();
            }

            return list.OrderBy(o => o.Circuito).ToList();
        }

        public static void Persist(ObjectRecircularAuto recircular)
        {
            ObjectRecircularAuto objc = Load(recircular.Circuito);
            
            //Insert
            if (objc == null)
            {
                StringBuilder sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {

                        conn.Open();
                        sb.Append("INSERT INTO RecircularAuto (Circuito, Habilitado, VolumeRecircular, VolumeDosado, DtInicio) VALUES (");
                        sb.Append("'" + recircular.Circuito.ToString() + "', ");
                        sb.Append("'" + (recircular.Habilitado ? "True" : "False") + "', ");
                        sb.Append("'" + recircular.VolumeRecircular.ToString().Replace(",", ".") + "', ");
                        sb.Append("'" + recircular.VolumeDosado.ToString().Replace(",", ".") + "', ");
                        sb.Append("'" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", recircular.DtInicio) + "' ");
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
                        sb.Append("UPDATE RecircularAuto SET "); // Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, DtInicio;
                        sb.Append("Habilitado = '" + (recircular.Habilitado ? "True" : "False") + "', ");
                        sb.Append("VolumeRecircular = '" + recircular.VolumeRecircular.ToString().Replace(",", ".") + "', ");
                        sb.Append("VolumeDosado = '" + recircular.VolumeDosado.ToString().Replace(",", ".") + "', ");
                        sb.Append("DtInicio = '" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", recircular.DtInicio) + "' ");
                        sb.Append(" WHERE Circuito = '" + recircular.Circuito.ToString() + "';");

                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                }
            }
        }

        public static void Persist(List<ObjectRecircularAuto> lista)
        {
            if (lista != null && lista.Count > 0)
            {
                foreach (ObjectRecircularAuto recircular in lista)
                {
                    ObjectRecircularAuto objc = Load(recircular.Circuito);
                    //Insert
                    if (objc == null)
                    {
                        StringBuilder sb = new StringBuilder();
                        using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {
                                conn.Open();
                                sb.Append("INSERT INTO RecircularAuto (Circuito, Habilitado, VolumeRecircular, VolumeDosado, DtInicio) VALUES (");
                                sb.Append("'" + recircular.Circuito.ToString() + "', ");
                                sb.Append("'" + (recircular.Habilitado ? "True" : "False") + "', ");
                                sb.Append("'" + recircular.VolumeRecircular.ToString().Replace(",", ".") + "', ");
                                sb.Append("'" + recircular.VolumeDosado.ToString().Replace(",", ".") + "', ");
                                sb.Append("'" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", recircular.DtInicio) + "' ");
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
                                sb.Append("UPDATE RecircularAuto SET "); // Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, DtInicio;
                                sb.Append("Habilitado = '" + (recircular.Habilitado ? "True" : "False") + "', ");
                                sb.Append("VolumeRecircular = '" + recircular.VolumeRecircular.ToString().Replace(",", ".") + "', ");
                                sb.Append("VolumeDosado = '" + recircular.VolumeDosado.ToString().Replace(",", ".") + "', ");
                                sb.Append("DtInicio = '" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", recircular.DtInicio) + "' ");
                                sb.Append(" WHERE Circuito = '" + recircular.Circuito.ToString() + "';");

                                cmd.CommandText = sb.ToString();

                                cmd.ExecuteNonQuery();

                                conn.Close();
                            }
                        }
                    }
                }
            }
        }

        public static bool Validate(List<ObjectRecircularAuto> lista, out string outMsg)
        {
            bool retorno = true;
            outMsg = "";
            foreach (ObjectRecircularAuto _rec in lista)
            {
                if (_rec.Habilitado)
                {
                    if (_rec.VolumeRecircular == 0)
                    {
                        outMsg += string.Format("Recirculação, circuito {0} possui Volume para Recirculação com valor igual ou abaixo de zero", _rec.Circuito);
                        retorno = false;
                    }

                    if (!retorno)
                    {
                        break;
                    }
                }
            }

            return retorno;
        }

        public static void UpdateVolumeDosado(int circuito, double volumeDos)
        {
            ObjectRecircularAuto _rec = Load(circuito);
            if (_rec.Habilitado)
            {
                _rec.VolumeDosado += volumeDos;
                ObjectRecircularAuto.Persist(_rec);
            }
        }

        public static bool UpdateResetDate(DateTime data_hora)
        {
            bool retorno = false;
            
            if (File.Exists(PathFile))
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "UPDATE RecircularAuto SET DtInicio = '" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", data_hora) + "';";
                        retorno = cmd.ExecuteNonQuery() > 0;
                        conn.Close();
                    }
                }
            }

            return retorno;
        }

        public static bool Delete(int circuito)
        {
            bool retorno = false;
            
            var commands = new[]
            {
                "DELETE FROM RecircularAuto WHERE Circuito = '" + circuito +"';"
            };

            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    foreach (var command in commands)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(command);
                        string createQuery = sb.ToString();
                        cmd.CommandText = createQuery;
                        retorno = cmd.ExecuteNonQuery() > 0;
                    }
                    conn.Close();
                }
            }

            return retorno;
        }
    }
}