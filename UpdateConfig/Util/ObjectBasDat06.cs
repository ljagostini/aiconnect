using System.Data.SQLite;
using System.Text;

namespace UpdateConfig.Util
{
	public class ObjectBasDat06
    {
        public static readonly string PathFile = Path.Combine(@"C:\Percolore\IOConnect\", "BasDat06.db");
        public string Name { get; set; }
        public double Volume { get; set; } = 0;
        public int Circuito { get; set; }

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
            catch
            {
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
            catch
            {
                throw;
            }
        }
    }
}
