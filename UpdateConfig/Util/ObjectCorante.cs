using System.Data.SQLite;
using System.Text;

namespace UpdateConfig.Util
{
	public class ObjectCorante
    {
        public static readonly string PathFile = Path.Combine(@"C:\Percolore\IOConnect\", "Colorantes.db");

        public int Circuito { get; set; }

        public bool IsBase { get; set; } = false;


        public static ObjectCorante Load(int Circuito)
        {
            ObjectCorante colorante = null;
            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Corantes WHERE Motor = '" + Circuito.ToString() + "';";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                colorante = new ObjectCorante();
                                colorante.Circuito = int.Parse(reader["Motor"].ToString());                                
                                colorante.IsBase = Convert.ToBoolean(reader["IsBase"].ToString());
                                break;
                            }
                        }
                    }
                    conn.Close();
                }


                return colorante;
            }
            catch
            {
                throw;
            }
        }

        public static void Persist(List<ObjectCorante> lista)
        {
            try
            {
                if (lista != null && lista.Count > 0)
                {
                    foreach (ObjectCorante colorante in lista)
                    {
                        ObjectCorante objc = Load(colorante.Circuito);
                        //Update
                        if (objc != null)
                        {                            
                            StringBuilder sb = new StringBuilder();
                            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                {
                                    conn.Open();
                                    sb.Append("UPDATE Corantes SET "); // (Motor, Nome, MassaEspecifica, Habilitado, Volume, Correspondencia, Dispositivo, NivelMinimo, NivelMaximo) VALUES (");
                                    sb.Append("IsBase = '" + (colorante.IsBase ? "True" : "False") + "' ");
                                    sb.Append(" WHERE Motor = '" + colorante.Circuito.ToString() + "';");
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
