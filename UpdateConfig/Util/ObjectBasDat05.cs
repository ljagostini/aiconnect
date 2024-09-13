using System.Data.SQLite;
using System.Text;

namespace UpdateConfig.Util
{
	public class ObjectBasDat05
    {
        public static readonly string PathFile = Path.Combine(@"C:\Percolore\IOConnect\", "BasDat05.db");

        public string Name { get; set; }
        public double Volume { get; set; } = 0;
        public int Circuito { get; set; }

        public static ObjectBasDat05 Load(int Circuito)
        {
            ObjectBasDat05 basDat05 = null;
            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM BasDat05 WHERE Circuito = '" + Circuito.ToString() + "';";

						using SQLiteDataReader reader = cmd.ExecuteReader();
						while (reader.Read())
						{
							basDat05 = new ObjectBasDat05
							{
								Circuito = int.Parse(reader["Circuito"].ToString()),
								Volume = double.Parse(reader["Volume"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
								Name = reader["Name"].ToString()
							};

							break;
						}
					}
                    conn.Close();
                }


                return basDat05;
            }
            catch
            {
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
                            StringBuilder sb = new();
							using SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false);
							using SQLiteCommand cmd = new(conn);

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
                        //Update
                        else
                        {
                            StringBuilder sb = new();
							using SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false);
							using SQLiteCommand cmd = new(conn);
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
            catch
            {
                throw;
            }
        }

    }
}
