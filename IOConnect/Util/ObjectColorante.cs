using Percolore.Core.Logging;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectColorante
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Colorantes.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public int Circuito { get; set; }
        public string ColorRGBVisibleGrid { get; set; } = string.Empty;
        public string Nome { get; set; }
        public double MassaEspecifica { get; set; }
        public bool Habilitado { get; set; } = true;
        public double Volume { get; set; } = 0;
        public int Correspondencia { get; set; } = 0;

        public int Dispositivo { get; set; } = 1;

        public double NivelMinimo { get; set; } = 0;
        public double NivelMaximo { get; set; } = 0;

        public bool IsBase { get; set; } = false;

        public int Seguidor { get; set; } = -1;

        public int Step { get; set; } = 0;

        public double VolumePurga { get; set; } = 0;

        public string ColorRGB { get; set; }

        public Color corCorante { get; set; }

        public bool IsBicoIndividual { get; set; } = false;
        public double VolumeBicoIndividual { get; set; } = 0;



        public ObjectColorante() { }

        public static void CreateBD()
        {
            try
            {
                if (!File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CREATE TABLE IF NOT EXISTS [Corantes] (Motor TEXT NULL, Nome TEXT NULL, MassaEspecifica TEXT NULL, Habilitado TEXT NULL, ");
                    sb.Append("Volume TEXT NULL, Correspondencia TEXT NULL, Dispositivo TEXT NULL, NivelMinimo TEXT NULL, NivelMaximo TEXT NULL, IsBase TEXT NULL, ");
                    sb.Append("Seguidor TEXT NULL, Step TEXT NULL, VolumePurga TEXT NULL, ColorRGB TEXT NULL, IsBicoIndividual TEXT NULL, VolumeBicoIndividual TEXT NULL);");

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
				LogManager.LogError($"Erro no módulo {typeof(ObjectColorante).Name}: ", e);
			}
		}

        public static ObjectColorante Load(int Circuito)
        {
            ObjectColorante colorante = null;
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
                                colorante = new ObjectColorante();
                                colorante.Circuito = int.Parse(reader["Motor"].ToString());
                                colorante.Nome = reader["Nome"].ToString();
                                colorante.MassaEspecifica = double.Parse(reader["MassaEspecifica"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                colorante.Habilitado = Convert.ToBoolean(reader["Habilitado"].ToString());
                                colorante.Volume = double.Parse(reader["Volume"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                colorante.Correspondencia = Convert.ToInt32(reader["Correspondencia"].ToString());
                                colorante.Dispositivo = Convert.ToInt32(reader["Dispositivo"].ToString());
                                colorante.NivelMinimo = double.Parse(reader["NivelMinimo"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                colorante.NivelMaximo = double.Parse(reader["NivelMaximo"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                colorante.IsBase = Convert.ToBoolean(reader["IsBase"].ToString());
                                colorante.Seguidor = Convert.ToInt32(reader["Seguidor"].ToString());
                                colorante.Step = Convert.ToInt32(reader["Step"].ToString());
                                colorante.VolumePurga = double.Parse(reader["VolumePurga"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                colorante.VolumePurga = 1;
                                
                                try
                                {
                                    colorante.ColorRGB = reader["ColorRGB"].ToString();
                                    int red = 255;
                                    int green = 255;
                                    int blue = 255;
                                    
                                    if (colorante.ColorRGB.Contains(";"))
                                    {
                                        string[] _arr = colorante.ColorRGB.Split(';');
                                        if (_arr.Length > 2)
                                        {
                                            red = Convert.ToInt32(_arr[0]);
                                            green = Convert.ToInt32(_arr[1]);
                                            blue = Convert.ToInt32(_arr[2]);
                                        }
                                    }
                                    colorante.corCorante = Color.FromArgb(red, green, blue);
                                }
                                catch
                                {
                                    colorante.ColorRGB = "255;255;255;";
                                    colorante.corCorante = Color.White;
                                }

                                colorante.VolumeBicoIndividual = double.Parse(reader["VolumeBicoIndividual"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                colorante.IsBicoIndividual = Convert.ToBoolean(reader["IsBicoIndividual"].ToString());
                                break;
                            }
                        }
                    }
                    conn.Close();
                }

                return colorante;
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectColorante).Name}: ", e);
                throw;
			}
		}

        public static List<ObjectColorante> List()
        {
            List<ObjectColorante> list = new List<ObjectColorante>();

            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Corantes;";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ObjectColorante colorante = new ObjectColorante();
                                colorante.Circuito = int.Parse(reader["Motor"].ToString());
                                colorante.Nome = reader["Nome"].ToString();
                                colorante.MassaEspecifica = double.Parse(reader["MassaEspecifica"].ToString(),  System.Globalization.CultureInfo.InvariantCulture);

                                colorante.Habilitado = Convert.ToBoolean(reader["Habilitado"].ToString());

                                colorante.Volume = double.Parse(reader["Volume"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                                colorante.Correspondencia = Convert.ToInt32(reader["Correspondencia"].ToString());

                                colorante.Dispositivo = Convert.ToInt32(reader["Dispositivo"].ToString());

                                colorante.NivelMinimo = double.Parse(reader["NivelMinimo"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                                colorante.NivelMaximo = double.Parse(reader["NivelMaximo"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                colorante.IsBase = Convert.ToBoolean(reader["IsBase"].ToString());
                                colorante.Seguidor = Convert.ToInt32(reader["Seguidor"].ToString());
                                colorante.Step = Convert.ToInt32(reader["Step"].ToString());
                                try
                                {
                                    colorante.VolumePurga = double.Parse(reader["VolumePurga"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                }
                                catch
                                { 
                                    colorante.VolumePurga = 1; 
                                }

                                try
                                {
                                    colorante.ColorRGB = reader["ColorRGB"].ToString();
                                    int red = 255;
                                    int green = 255;
                                    int blue = 255;
                                    if (colorante.ColorRGB.Contains(";"))
                                    {
                                        string[] _arr = colorante.ColorRGB.Split(';');
                                        if(_arr.Length > 2)
                                        {
                                            red = Convert.ToInt32(_arr[0]);
                                            green = Convert.ToInt32(_arr[1]);
                                            blue = Convert.ToInt32(_arr[2]);
                                        }

                                    }

                                    colorante.corCorante = Color.FromArgb(red, green, blue);
                                }
                                catch
                                {
                                    colorante.ColorRGB = "255;255;255;";
                                    colorante.corCorante = Color.White;
                                }

                                colorante.VolumeBicoIndividual = double.Parse(reader["VolumeBicoIndividual"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                colorante.IsBicoIndividual = Convert.ToBoolean(reader["IsBicoIndividual"].ToString());
                                
                                list.Add(colorante);
                            }
                        }
                    }

                    conn.Close();
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectColorante).Name}: ", e);
			}

			return list.OrderBy(o=>o.Circuito).ToList();
        }

        public static bool Validate(List<ObjectColorante> lista, out string outMsg)
        {
            if (lista == null)
                throw new ArgumentNullException();

            StringBuilder validaItem = new StringBuilder();
            StringBuilder validaLista = new StringBuilder();

            //Seleciona somente habilitados
            lista = lista.Where(w => w.Habilitado == true).ToList();
            foreach (ObjectColorante colorante in lista)
            {
                validaItem.Clear();

                //[Valida apenas colorantes habilitados]
                if (!colorante.Habilitado)
                    continue;

                if (string.IsNullOrEmpty(colorante.Nome))
                    validaItem.AppendLine(Negocio.IdiomaResxExtensao.Colorantes_NomeObrigatorio);

                if (colorante.MassaEspecifica == 0)
                    validaItem.AppendLine(Negocio.IdiomaResxExtensao.Colorantes_MassaEspecificaObrigatoria);

                int count =
                    lista.Count(s => s.Correspondencia == colorante.Correspondencia);
                if (count > 1)
                    validaItem.AppendLine(Negocio.IdiomaResxExtensao.Colorantes_CorrespondenciaRepetida);

                if (validaItem.Length > 0)
                {
                    string texto =
                        string.Format(Negocio.IdiomaResxExtensao.Colorantes_Circuito, colorante.Circuito.ToString());
                    validaLista.AppendLine(texto);
                    validaLista.AppendLine(validaItem.ToString());
                }
            }

            outMsg = validaLista.ToString();
            return
                (validaLista.Length == 0);
        }

        public static void Persist(ObjectColorante colorante)
        {
            try
            {
                ObjectColorante objc = Load(colorante.Circuito);
                //Insert
                if (objc == null)
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            sb.Append("INSERT INTO Corantes (Motor, Nome, MassaEspecifica, Habilitado, Volume, Correspondencia, Dispositivo, NivelMinimo, NivelMaximo, IsBase, Seguidor, Step, VolumePurga) VALUES (");
                            sb.Append("'" + colorante.Circuito.ToString() + "', ");
                            sb.Append("'" + colorante.Nome.ToString() + "', ");
                            sb.Append("'" + colorante.MassaEspecifica.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + (colorante.Habilitado ? "True" : "False") + "', ");
                            sb.Append("'" + colorante.Volume.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + colorante.Correspondencia.ToString() + "', ");
                            sb.Append("'" + colorante.Dispositivo.ToString() + "', ");
                            sb.Append("'" + colorante.NivelMinimo.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + colorante.NivelMaximo.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + (colorante.IsBase ? "True" : "False") + "', ");
                            sb.Append("'" + colorante.Seguidor.ToString() + "', ");
                            sb.Append("'" + colorante.Step.ToString() + "', ");
                            sb.Append("'" + colorante.VolumePurga.ToString().Replace(",", ".") + "' ");
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
                            sb.Append("UPDATE Corantes SET "); // (Motor, Nome, MassaEspecifica, Habilitado, Volume, Correspondencia, Dispositivo, NivelMinimo, NivelMaximo) VALUES (");
                            sb.Append("Motor = '" + colorante.Circuito.ToString() + "', ");
                            sb.Append("Nome = '" + colorante.Nome.ToString() + "', ");
                            sb.Append("MassaEspecifica = '" + colorante.MassaEspecifica.ToString().Replace(",", ".") + "', ");
                            sb.Append("Habilitado ='" + (colorante.Habilitado ? "True" : "False") + "', ");
                            sb.Append("Volume = '" + colorante.Volume.ToString().Replace(",", ".") + "', ");
                            sb.Append("Correspondencia = '" + colorante.Correspondencia.ToString() + "', ");
                            sb.Append("Dispositivo = '" + colorante.Dispositivo.ToString() + "', ");
                            sb.Append("NivelMinimo = '" + colorante.NivelMinimo.ToString().Replace(",", ".") + "', ");
                            sb.Append("NivelMaximo = '" + colorante.NivelMaximo.ToString().Replace(",", ".") + "', ");
                            sb.Append("IsBase = '" + (colorante.IsBase ? "True" : "False") + "', ");
                            sb.Append("Seguidor = '" + colorante.Seguidor.ToString() + "', ");
                            sb.Append("Step = '" + colorante.Step.ToString() + "', ");
                            sb.Append("VolumePurga = '" + colorante.VolumePurga.ToString().Replace(",", ".") + "', ");
                            sb.Append("ColorRGB = '" + colorante.ColorRGB + "', ");
                            sb.Append("IsBicoIndividual = '" + (colorante.IsBicoIndividual ? "True" : "False") + "', ");
                            sb.Append("VolumeBicoIndividual = '" + colorante.VolumeBicoIndividual.ToString().Replace(",", ".") + "' ");
                            sb.Append(" WHERE Motor = '" + colorante.Circuito.ToString() + "';");

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }
                }
               
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectColorante).Name}: ", e);
                throw;
			}
		}

        public static void Persist(List<ObjectColorante> lista)
        {
            try
            {
                if (lista != null && lista.Count > 0)
                {
                    foreach (ObjectColorante colorante in lista)
                    {
                        ObjectColorante objc = Load(colorante.Circuito);
                        //Insert
                        if (objc == null)
                        {
                            StringBuilder sb = new StringBuilder();
                            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                {
                                    conn.Open();
                                    sb.Append("INSERT INTO Corantes (Motor, Nome, MassaEspecifica, Habilitado, Volume, Correspondencia, Dispositivo, NivelMinimo, NivelMaximo, IsBase, Seguidor, Step, VolumePurga) VALUES (");
                                    sb.Append("'" + colorante.Circuito.ToString() + "', ");
                                    sb.Append("'" + colorante.Nome.ToString() + "', ");
                                    sb.Append("'" + colorante.MassaEspecifica.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + (colorante.Habilitado ? "True" : "False") + "', ");
                                    sb.Append("'" + colorante.Volume.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + colorante.Correspondencia.ToString() + "', ");
                                    sb.Append("'" + colorante.Dispositivo.ToString() + "', ");
                                    sb.Append("'" + colorante.NivelMinimo.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + colorante.NivelMaximo.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + (colorante.IsBase ? "True" : "False") + "', ");
                                    sb.Append("'" + colorante.Seguidor.ToString() + "', ");
                                    sb.Append("'" + colorante.Step.ToString() + "', ");
                                    sb.Append("'" + colorante.VolumePurga.ToString().Replace(",", ".") + "' ");
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
                                    sb.Append("UPDATE Corantes SET "); // (Motor, Nome, MassaEspecifica, Habilitado, Volume, Correspondencia, Dispositivo, NivelMinimo, NivelMaximo) VALUES (");
                                    sb.Append("Motor = '" + colorante.Circuito.ToString() + "', ");
                                    sb.Append("Nome = '" + colorante.Nome.ToString() + "', ");
                                    sb.Append("MassaEspecifica = '" + colorante.MassaEspecifica.ToString().Replace(",", ".") + "', ");
                                    sb.Append("Habilitado ='" + (colorante.Habilitado ? "True" : "False") + "', ");
                                    sb.Append("Volume = '" + colorante.Volume.ToString().Replace(",", ".") + "', ");
                                    sb.Append("Correspondencia = '" + colorante.Correspondencia.ToString() + "', ");
                                    sb.Append("Dispositivo = '" + colorante.Dispositivo.ToString() + "', ");
                                    sb.Append("NivelMinimo = '" + colorante.NivelMinimo.ToString().Replace(",", ".") + "', ");
                                    sb.Append("NivelMaximo = '" + colorante.NivelMaximo.ToString().Replace(",", ".") + "', ");
                                    sb.Append("IsBase = '" + (colorante.IsBase ? "True" : "False") + "', ");
                                    sb.Append("Seguidor = '" + colorante.Seguidor.ToString() + "', ");
                                    sb.Append("Step = '" + colorante.Step.ToString() + "', ");
                                    sb.Append("VolumePurga = '" + colorante.VolumePurga.ToString().Replace(",", ".") + "', ");

                                    sb.Append("ColorRGB = '" + colorante.ColorRGB + "', ");
                                    sb.Append("IsBicoIndividual = '" + (colorante.IsBicoIndividual ? "True" : "False") + "', ");
                                    sb.Append("VolumeBicoIndividual = '" + colorante.VolumeBicoIndividual.ToString().Replace(",", ".") + "' ");
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
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectColorante).Name}: ", e);
                throw;
			}
		}

        public static DataSet getTables()
        {
            DataSet ds = new DataSet();
            if (File.Exists(PathFile))
            {
                using (SQLiteConnection con = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    con.Open();
                    DataTable dt = con.GetSchema("Tables");
                    foreach (DataRow row in dt.Rows)
                    {
                        string tablename = (string)row[2];
                        DataTable tbl = new DataTable(tablename);
                        DataTable columnsTable = con.GetSchema("Columns", new[] { null, null, tablename, null });

                        bool incluir = false;

                        foreach (DataRow rownm in columnsTable.Rows)
                        {
                            string colname = rownm["COLUMN_NAME"].ToString();
                            DataColumn col = new DataColumn(colname);
                            string data_type = rownm["DATA_TYPE"].ToString();
                            if (data_type != "")
                            {
                                if (data_type == "integer")
                                {
                                    col.DataType = typeof(System.Int32);
                                }
                                else if (data_type == "text")
                                {
                                    col.DataType = typeof(System.String);
                                }
                                incluir = true;
                                tbl.Columns.Add(col);
                            }
                        }
                        if (incluir)
                        {
                            ds.Tables.Add(tbl);
                        }

                    }

                    con.Close();
                }
            }

            return ds;
        }

        public static bool Delete(int motor)
        {
            bool retorno = false;
            try
            {
                var commands = new[] {
                            "DELETE FROM Corantes WHERE Motor = '" + motor +"';"
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
                            cmd.ExecuteNonQuery();
                            retorno = true;
                        }
                        conn.Close();
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectColorante).Name}: ", e);
			}

			return retorno;
        }
    }   
}