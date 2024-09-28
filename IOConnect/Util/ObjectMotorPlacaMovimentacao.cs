using Percolore.Core.Logging;
using System.Data.SQLite;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectMotorPlacaMovimentacao
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "PlMov.db");
        public static readonly string FileName = Path.GetFileName(PathFile);
        public int Circuito { get; set; }
        public int TipoMotor { get; set; } = 0;                     //0 -> Motor de Passo | 1 -> Motor DC 
        public bool Habilitado { get; set; } = true;
        public string NameTag { get; set; } = "";
        public int Pulsos { get; set; } = 0;
        public int Velocidade { get; set; } = 0;  
        public int Aceleracao { get; set; } = 0;
        public int Delay { get; set; } = 0;        

        public ObjectMotorPlacaMovimentacao()
        {

        }

        public static void CreateBD()
        {
            try
            {
                if (!File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CREATE TABLE IF NOT EXISTS [PlacaMovimentacao] (Circuito TEXT NULL, Habilitado TEXT NULL, NameTag TEXT NULL, Pulsos TEXT NULL, Velocidade TEXT NULL, Aceleracao TEXT NULL, Delay TEXT NULL, TipoMotor TEXT NULL);");

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
				LogManager.LogError($"Erro no módulo {typeof(ObjectMotorPlacaMovimentacao).Name}: ", e);
			}
		}

        public static ObjectMotorPlacaMovimentacao Load(int Circuito)
        {
            ObjectMotorPlacaMovimentacao pMPMOV = null;
            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM PlacaMovimentacao WHERE Circuito = '" + Circuito.ToString() + "';";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pMPMOV = new ObjectMotorPlacaMovimentacao();
                                pMPMOV.Circuito = int.Parse(reader["Circuito"].ToString());
                                pMPMOV.Habilitado = Convert.ToBoolean(reader["Habilitado"].ToString());
                                pMPMOV.Aceleracao = Convert.ToInt32(reader["Aceleracao"].ToString());
                                pMPMOV.Delay = Convert.ToInt32(reader["Delay"].ToString());
                                pMPMOV.NameTag = reader["NameTag"].ToString();
                                pMPMOV.Pulsos = Convert.ToInt32(reader["Pulsos"].ToString());
                                pMPMOV.TipoMotor = Convert.ToInt32(reader["TipoMotor"].ToString());
                                pMPMOV.Velocidade = Convert.ToInt32(reader["Velocidade"].ToString());


                                break;
                            }
                        }
                    }
                    conn.Close();
                }

                return pMPMOV;
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectMotorPlacaMovimentacao).Name}: ", e);
                throw;
			}
		}

        public static List<ObjectMotorPlacaMovimentacao> List()
        {
            List<ObjectMotorPlacaMovimentacao> list = new List<ObjectMotorPlacaMovimentacao>();

            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM PlacaMovimentacao;";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ObjectMotorPlacaMovimentacao pMPMOV = new ObjectMotorPlacaMovimentacao();
                                pMPMOV = new ObjectMotorPlacaMovimentacao();
                                pMPMOV.Circuito = int.Parse(reader["Circuito"].ToString());
                                pMPMOV.Habilitado = Convert.ToBoolean(reader["Habilitado"].ToString());
                                pMPMOV.Aceleracao = Convert.ToInt32(reader["Aceleracao"].ToString());
                                pMPMOV.Delay = Convert.ToInt32(reader["Delay"].ToString());
                                pMPMOV.NameTag = reader["NameTag"].ToString();
                                pMPMOV.Pulsos = Convert.ToInt32(reader["Pulsos"].ToString());
                                pMPMOV.TipoMotor = Convert.ToInt32(reader["TipoMotor"].ToString());
                                pMPMOV.Velocidade = Convert.ToInt32(reader["Velocidade"].ToString());
                                list.Add(pMPMOV);
                            }
                        }
                    }
                    conn.Close();
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectMotorPlacaMovimentacao).Name}: ", e);
			}

			return list.OrderBy(o=>o.Circuito).ToList();
        }

        public static void Persist(ObjectMotorPlacaMovimentacao pMMOV)
        {
            try
            {
                ObjectMotorPlacaMovimentacao objc = Load(pMMOV.Circuito);
                //Insert
                if (objc == null)
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            sb.Append("INSERT INTO PlacaMovimentacao (Circuito, Habilitado, NameTag, Pulsos, Velocidade, Aceleracao, Delay, TipoMotor) VALUES (");
                            sb.Append("'" + pMMOV.Circuito.ToString() + "', ");
                            sb.Append("'" + (pMMOV.Habilitado ? "True" : "False") + "', ");
                            sb.Append("'" + pMMOV.NameTag + "', ");
                            sb.Append("'" + pMMOV.Pulsos.ToString() + "', ");
                            sb.Append("'" + pMMOV.Velocidade.ToString() + "', ");
                            sb.Append("'" + pMMOV.Aceleracao.ToString() + "', ");
                            sb.Append("'" + pMMOV.Delay.ToString() + "', ");
                            sb.Append("'" + pMMOV.TipoMotor.ToString() + "' ");
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
                            sb.Append("UPDATE PlacaMovimentacao SET "); // Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, DtInicio;
                            sb.Append("Habilitado = '" + (pMMOV.Habilitado ? "True" : "False") + "', ");
                            sb.Append("NameTag = '" + pMMOV.NameTag + "', ");
                            sb.Append("Pulsos ='" + pMMOV.Pulsos.ToString() + "', ");
                            sb.Append("Velocidade = '" + pMMOV.Velocidade.ToString() + "', ");
                            sb.Append("Aceleracao = '" + pMMOV.Aceleracao.ToString() + "', ");
                            sb.Append("Delay = '" + pMMOV.Delay.ToString() + "', ");
                            sb.Append("TipoMotor = '" + pMMOV.TipoMotor.ToString() + "' ");
                            sb.Append(" WHERE Circuito = '" + pMMOV.Circuito.ToString() + "';");

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }
                }

            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectMotorPlacaMovimentacao).Name}: ", e);
                throw;
			}
		}

        public static void Persist(List<ObjectMotorPlacaMovimentacao> lista)
        {
            try
            {
                if (lista != null && lista.Count > 0)
                {
                    foreach (ObjectMotorPlacaMovimentacao pMMOV in lista)
                    {
                        ObjectMotorPlacaMovimentacao objc = Load(pMMOV.Circuito);
                        //Insert
                        if (objc == null)
                        {
                            StringBuilder sb = new StringBuilder();
                            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                {
                                    conn.Open();
                                    sb.Append("INSERT INTO PlacaMovimentacao (Circuito, Habilitado, NameTag, Pulsos, Velocidade, Aceleracao, Delay, TipoMotor) VALUES (");
                                    sb.Append("'" + pMMOV.Circuito.ToString() + "', ");
                                    sb.Append("'" + (pMMOV.Habilitado ? "True" : "False") + "', ");
                                    sb.Append("'" + pMMOV.NameTag + "', ");
                                    sb.Append("'" + pMMOV.Pulsos.ToString() + "', ");
                                    sb.Append("'" + pMMOV.Velocidade.ToString() + "', ");
                                    sb.Append("'" + pMMOV.Aceleracao.ToString() + "', ");
                                    sb.Append("'" + pMMOV.Delay.ToString() + "', ");
                                    sb.Append("'" + pMMOV.TipoMotor.ToString() + "' ");
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
                                    sb.Append("UPDATE PlacaMovimentacao SET "); // Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, DtInicio;
                                    sb.Append("Habilitado = '" + (pMMOV.Habilitado ? "True" : "False") + "', ");
                                    sb.Append("NameTag = '" + pMMOV.NameTag + "', ");
                                    sb.Append("Pulsos ='" + pMMOV.Pulsos.ToString() + "', ");
                                    sb.Append("Velocidade = '" + pMMOV.Velocidade.ToString() + "', ");
                                    sb.Append("Aceleracao = '" + pMMOV.Aceleracao.ToString() + "', ");
                                    sb.Append("Delay = '" + pMMOV.Delay.ToString() + "', ");
                                    sb.Append("TipoMotor = '" + pMMOV.TipoMotor.ToString() + "' ");
                                    sb.Append(" WHERE Circuito = '" + pMMOV.Circuito.ToString() + "';");

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
				LogManager.LogError($"Erro no módulo {typeof(ObjectMotorPlacaMovimentacao).Name}: ", e);
                throw;
			}
		}

        public static bool Validate(List<ObjectMotorPlacaMovimentacao> lista, out string outMsg)
        {
            bool retorno = true;
            outMsg = "";
            foreach (ObjectMotorPlacaMovimentacao _rec in lista)
            {
                if (_rec.Habilitado)
                {
                    if (_rec.TipoMotor == 0)
                    {
                        if (_rec.Aceleracao < 1)
                        {
                            outMsg += string.Format("MotorPlacaMovimentacao, circuito {0} possui Aceleracao inferior a 1", _rec.Circuito);
                            retorno = false;

                        }
                        if (_rec.Pulsos < 1)
                        {
                            outMsg += string.Format("MotorPlacaMovimentacao, circuito {0} possui Pulsos inferior a 1", _rec.Circuito);
                            retorno = false;
                        }
                        if (_rec.Velocidade < 1)
                        {
                            outMsg += string.Format("MotorPlacaMovimentacao, circuito {0} possui Velocidade inferior a 1", _rec.Circuito);
                            retorno = false;
                        }
                        if (_rec.Delay < 1)
                        {
                            outMsg += string.Format("MotorPlacaMovimentacao, circuito {0} possui Delay inferior a 1", _rec.Circuito);
                            retorno = false;
                        }
                    }

                    if (!retorno)
                    {
                        break;
                    }
                }
            }

            return retorno;
        }
    }
}