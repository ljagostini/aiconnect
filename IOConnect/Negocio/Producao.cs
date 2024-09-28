using Percolore.Core.Logging;
using System.Data.SQLite;

namespace Percolore.IOConnect.Negocio
{
	public class Producao
    {
        private string percProducao = "percProducao.db";
        private string percHistProd = "percHistProducao.DB";

        public Producao()
        {            
            CreateBD();
        }

        public void CreateBD()
        {
            try
            {
                string createQuery =
                        @"CREATE TABLE IF NOT EXISTS [Producao] (
                            [Id]     INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [DATAHORA] TEXT NULL,
                            [CONTEUDO]   TEXT NULL,
                            [INTEGRADO] INTEGER NULL)";

                if (!File.Exists("percProducao.db"))
                {
                    SQLiteConnection connectCreate = Util.SQLite.CreateSQLiteConnection(this.percProducao, false);
                    connectCreate.Open();
                    // Open connection to create DB if not exists.
                    connectCreate.Close();
                    Thread.Sleep(2000);
                }
                if (File.Exists("percProducao.db"))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(this.percProducao, false))
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

                if (!File.Exists("percHistProducao.db"))
                {
                    SQLiteConnection connectCreate = Util.SQLite.CreateSQLiteConnection(this.percHistProd, false);
                    connectCreate.Open();
					// Open connection to create DB if not exists.
					connectCreate.Close();
                    Thread.Sleep(2000);
                }
                if (File.Exists("percHistProducao.db"))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(this.percHistProd, false))
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
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}
		}
        
        public int insertProducao(string msg, string DATAHORA)
        {
            int retorno = 0;
            try
            {
                if(!File.Exists("percProducao.db"))
                {
                    CreateBD();
                }

                if (File.Exists("percProducao.db"))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(this.percProducao, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {  
                            conn.Open();
                            cmd.CommandText = "INSERT INTO Producao(DATAHORA, CONTEUDO, INTEGRADO)values('" + DATAHORA  +"', '" + msg  + " ','0');";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "SELECT last_insert_rowid();";
                            Int64 LastRowID64 = (Int64)cmd.ExecuteScalar();

                            // Then grab the bottom 32-bits as the unique ID of the row.
                            retorno = (int)LastRowID64;
                            conn.Close();
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }

        public bool upDateProducaoIntegracao(string id, bool integraddo)
        {
            bool retorno = false;
            try
            {
                if (!File.Exists("percProducao.db"))
                {
                    CreateBD();
                }
                if (File.Exists("percProducao.db"))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(this.percProducao, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();

                            if (integraddo)
                            {
                                cmd.CommandText = "UPDATE Producao SET INTEGRADO = '1' WHERE Id = '" + id + "';";
                            }
                            else
                            {
                                cmd.CommandText = "UPDATE Producao SET INTEGRADO = '0' WHERE Id = '" + id + "';";
                            }
                            cmd.ExecuteNonQuery();

                            conn.Close();
                            retorno = true;
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }

        public bool upDateProducaoIntegracao(string id, int integraddo)
        {
            bool retorno = false;
            try
            {
                if (!File.Exists("percProducao.db"))
                {
                    CreateBD();
                }
                if (File.Exists("percProducao.db"))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(this.percProducao, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();

                            cmd.CommandText = "UPDATE Producao SET INTEGRADO = '" + integraddo.ToString() + "' WHERE Id = '" + id + "';";
                            
                            cmd.ExecuteNonQuery();

                            conn.Close();
                            retorno = true;
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }

        public List<ProducaoBD> getListProducaoIntegrado(bool integrado)
        {
            List<ProducaoBD> retorno = new List<ProducaoBD>();
            try
            {
                if (!File.Exists("percProducao.db"))
                {
                    CreateBD();
                }
                if (File.Exists("percProducao.db"))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(this.percProducao, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();

                            if (integrado)
                            {
                                cmd.CommandText = "SELECT * FROM Producao WHERE INTEGRADO = '1'";
                            }
                            else
                            {
                                cmd.CommandText = "SELECT * FROM Producao WHERE INTEGRADO = '0'";
                            }
                            

                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    
                                    ProducaoBD reg = new ProducaoBD();
                                    reg.id = reader["ID"].ToString();
                                    reg.dataHora = reader["DATAHORA"].ToString();
                                    reg.conteudo = reader["CONTEUDO"].ToString();
                                    reg.integrado = reader["INTEGRADO"].ToString() == "0" ? false : true;
                                    retorno.Add(reg);
                                }
                            }

                            conn.Close();
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }

        public List<ProducaoBD> getListProducaoIntegrado(int integrado)
        {
            List<ProducaoBD> retorno = new List<ProducaoBD>();
            try
            {
                if (!File.Exists("percProducao.db"))
                {
                    CreateBD();
                }
                if (File.Exists("percProducao.db"))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(this.percProducao, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();

                            cmd.CommandText = "SELECT * FROM Producao WHERE INTEGRADO = '" + integrado.ToString() + "'";
                           


                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {

                                    ProducaoBD reg = new ProducaoBD();
                                    reg.id = reader["ID"].ToString();
                                    reg.dataHora = reader["DATAHORA"].ToString();
                                    reg.conteudo = reader["CONTEUDO"].ToString();
                                    reg.integrado = reader["INTEGRADO"].ToString() == "1" ? true : false;
                                    retorno.Add(reg);
                                }
                            }

                            conn.Close();
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }

        public bool deleteProducaoIntegrado(string id)
        {
            bool retorno = false;
            try
            {
                if (!File.Exists("percProducao.db"))
                {
                    CreateBD();
                }

                if (File.Exists("percProducao.db"))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(this.percProducao, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();

                            cmd.CommandText = "DELETE FROM Producao WHERE Id = '" + id + "';";
                            cmd.ExecuteNonQuery();

                            conn.Close();
                            retorno = true;
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }

        public int insertHistProducao(string msg, string DATAHORA)
        {
            int retorno = 0;
            try
            {
                if (!File.Exists("percHistProducao.db"))
                {
                    CreateBD();
                }
                if (File.Exists("percHistProducao.db"))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(this.percHistProd, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            cmd.CommandText = "INSERT INTO Producao(DATAHORA, CONTEUDO, INTEGRADO)values('" + DATAHORA  + "','"+ msg + " ','1');";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "SELECT last_insert_rowid();";
                            Int64 LastRowID64 = (Int64)cmd.ExecuteScalar();

                            // Then grab the bottom 32-bits as the unique ID of the row.
                            retorno = (int)LastRowID64;
                            conn.Close();
                        }
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {this.GetType().Name}: ", e);
			}

			return retorno;
        }


        public class ProducaoBD
        {
            public string id;
            public string dataHora;
            public string conteudo;
            public bool integrado;
            public ProducaoBD()
            {
                integrado = false;
            }
        }

    }
}