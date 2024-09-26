using System.Data.SQLite;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectEventos
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Eventos.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public int Id { get; set; }

        public string NUMERO_SERIE { get; set; }
        public int COD_EVENTO { get; set; }

        public string DETALHES { get; set; } = "";

        public DateTime DATAHORA { get; set; }

        public bool INTEGRADO { get; set; }

        public ObjectEventos() 
        {
            //CreateBD();
        }

        public static void CreateBD()
        {
            string createQuery =
                @"CREATE TABLE IF NOT EXISTS Eventos (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    NUMERO_SERIE TEXT NULL,
                    COD_EVENTO INTEGER NULL,
                    DETALHES TEXT NULL,
                    DATAHORA   TEXT NULL,
                    INTEGRADO INTEGER NULL);";

            if (!File.Exists(PathFile))
            {
                StringBuilder sb = new StringBuilder();

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
                if(File.Exists(PathFile))
                {
                    List<string> lquery = new List<string>();
                    lquery.Add("CREATE TABLE IF NOT EXISTS TIPO_EVENTO (ID_TIPO_EVENTO INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, COD_EVENTO INTEGER NULL, DESCRICAO TEXT NULL);");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('1', 'Iniciar Sistema');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('2', 'Fechar Sistema');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('3', 'Alterada Calibracao');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('4', 'Purga');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('5', 'Purga Individual');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('6', 'Recircular');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('7', 'Inicializar Circuitos');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('8', 'Monitoramento Circuitos');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('9', 'Formula Personalizada');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('10', 'Abastecimento');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('11', 'OnLine MSP');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('12', 'OffLine MSP');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('13', 'Alterada Configuacao');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('14', 'Falha Comunicacao Placa');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('15', 'Nivel Canister Baixo');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('16', 'Resset Placa');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('17', 'Maquina Ligada');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('18', 'Maquina Desligada');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('19', 'Alterado Produto');");
                    lquery.Add("INSERT INTO TIPO_EVENTO (COD_EVENTO, DESCRICAO) VALUES ('20', 'Dispensa de Produto');");

                    for(int i = 0; i < lquery.Count; i++)
                    {   
                        using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {
                                conn.Open();
                                cmd.CommandText = lquery[i];
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                    }
                }
            }
        }

        public static int InsertEvento(ObjectEventos objEvt)
        {
            int retorno = 0;
            
            CreateBD();

            StringBuilder sb = new StringBuilder();
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    sb.Append("INSERT INTO Eventos (NUMERO_SERIE, COD_EVENTO, DETALHES, DATAHORA, INTEGRADO) VALUES (");
                    sb.Append("'" + objEvt.NUMERO_SERIE + "', ");
                    sb.Append("'" + objEvt.COD_EVENTO.ToString() + "', ");
                    sb.Append("'" + objEvt.DETALHES + "', ");
                    sb.Append("'" + string.Format("{0:yyyy-MM-dd HH:mm:ss}", objEvt.DATAHORA) + "', ");                        
                    sb.Append("'" + (objEvt.INTEGRADO ? "1" : "0") + "'");
                    sb.Append(");");
                    cmd.CommandText = sb.ToString();

                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT last_insert_rowid();";
                    Int64 LastRowID64 = (Int64)cmd.ExecuteScalar();
                    retorno = (int)LastRowID64;

                    conn.Close();
                }
            }

            return retorno;
        }

        public static bool UpdateEventoIntegrado(int idEvento, bool integrado)
        {
            bool retorno = false;
            
            if (File.Exists(PathFile))
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        if (integrado)
                        {
                            cmd.CommandText = "UPDATE Eventos SET INTEGRADO = '1' WHERE Id = '" + idEvento.ToString() + "';";
                        }
                        else
                        {
                            cmd.CommandText = "UPDATE Eventos SET INTEGRADO = '0' WHERE Id = '" + idEvento.ToString() + "';";
                        }
                        cmd.ExecuteNonQuery();

                        conn.Close();
                        retorno = true;
                    }
                }
            }

            return retorno;
        }

        public static bool UpdateEventoIntegrado(int idEvento, int integrado)
        {
            bool retorno = false;
            
            if (File.Exists(PathFile))
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "UPDATE Eventos SET INTEGRADO = '" + integrado.ToString() + "' WHERE Id = '" + idEvento.ToString() + "';";

                        cmd.ExecuteNonQuery();

                        conn.Close();
                        retorno = true;
                    }
                }
            }

            return retorno;
        }

        public static List<ObjectEventos> getListEventosIntegrado(bool integrado)
        {
            List<ObjectEventos> retorno = null;
            
            if (File.Exists(PathFile))
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        if (integrado)
                        {
                            cmd.CommandText = "SELECT * FROM Eventos WHERE INTEGRADO = '1'";
                        }
                        else
                        {
                            cmd.CommandText = "SELECT * FROM Eventos WHERE INTEGRADO = '0'";
                        }


                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (retorno == null)
                                {
                                    retorno = new List<ObjectEventos>();
                                }
                                ObjectEventos reg = new ObjectEventos();
                                reg.Id = Convert.ToInt32(reader["ID"].ToString());
                                reg.DATAHORA = Convert.ToDateTime(reader["DATAHORA"].ToString());
                                reg.NUMERO_SERIE = reader["NUMERO_SERIE"].ToString();
                                reg.COD_EVENTO = Convert.ToInt32(reader["COD_EVENTO"].ToString());
                                reg.DETALHES = reader["DETALHES"] != DBNull.Value ? reader["DETALHES"].ToString() : "";
                                reg.INTEGRADO = reader["INTEGRADO"].ToString() == "0" ? false : true;
                                retorno.Add(reg);
                            }
                        }

                        conn.Close();
                    }
                }
            }

            return retorno;
        }

        public static bool UpdateEventosNumeroSerie(string numeroSerie)
        {
            bool retorno = false;
            
            if (File.Exists(PathFile))
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "UPDATE Eventos SET NUMERO_SERIE = '" + numeroSerie + "';";

                        cmd.ExecuteNonQuery();

                        conn.Close();
                        retorno = true;
                    }
                }
            }

            return retorno;
        }

        public static void GenerateBkp()
        {
            if (File.Exists(PathFile))
            {
                Util.ObjectParametros _par = Util.ObjectParametros.Load();
                bool existeFile = true;
                if (_par.LogAutomateBackup)
                {
                    FileInfo info = new FileInfo(PathFile);
                    double length = (info.Length / 1048576.0);
                    if (length > 20.0)
                    {
                        string[] arrayPath = PathFile.Split(Path.DirectorySeparatorChar);
                        if (arrayPath.Length > 0)
                        {
                            string nameFile = string.Format("{0:dd-MM-yyyy}", DateTime.Now) + "_" + arrayPath[arrayPath.Length - 1];
                            string nPathFile = "";
                            for (int i = 0; i < arrayPath.Length - 1; i++)
                            {
                                if (arrayPath[i].Length > 0)
                                {
                                    nPathFile += arrayPath[i] + Path.DirectorySeparatorChar;
                                }
                            }
                            if (nPathFile.Length > 0)
                            {
                                nPathFile += nameFile;
                            }
                            File.Move(PathFile, nPathFile);
                            existeFile = false;
                        }
                    }
                }
                if (!existeFile)
                {
                    CreateBD();
                }
            }
        }
    }
}