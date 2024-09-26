using System.Data.SQLite;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectUser
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Users.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public bool Tecnico { get; set; }
		/// <summary>
		/// Tipo de usuario: 0 = Operador | 1 = Gerente | 2 = Tecnico
		/// </summary>
		public int Tipo { get; set; }

        public ObjectUser() { }

        #region Métodos
        public static void CreateBD()
        {
            if (!File.Exists(PathFile))
            {

                SQLiteConnection connectCreate = Util.SQLite.CreateSQLiteConnection(PathFile, false);
                connectCreate.Open();
				// Open connection to create DB if not exists.
				connectCreate.Close();
                Thread.Sleep(2000);
                if (File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CREATE TABLE IF NOT EXISTS [User] (Id INTEGER PRIMARY KEY, Nome TEXT NULL, Senha TEXT NULL, Tecnico TEXT NULL, Tipo TEXT NULL);");
                    string createQuery = sb.ToString();
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

        public static ObjectUser Load(int id)
        {
            ObjectUser user = null;
            
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM User WHERE Id = " + id.ToString() + ";";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new ObjectUser();
                            user.Id = int.Parse(reader["Id"].ToString());
                            user.Nome = reader["Nome"].ToString();
                            user.Senha = reader["Senha"].ToString();
                            user.Tecnico = Convert.ToBoolean(reader["Tecnico"].ToString());
                            user.Tipo = int.Parse(reader["Tipo"].ToString());
                            break;
                        }
                    }
                }
                conn.Close();
            }

            return user;
        }

        public static ObjectUser Load(string nome, string senha)
        {
            ObjectUser user = null;
            
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM User WHERE Nome = '" + nome + "' AND Senha = '" + senha +"';";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new ObjectUser();
                            user.Id = int.Parse(reader["Id"].ToString());
                            user.Nome = reader["Nome"].ToString();
                            user.Senha = reader["Senha"].ToString();
                            user.Tecnico = Convert.ToBoolean(reader["Tecnico"].ToString());
                            user.Tipo = int.Parse(reader["Tipo"].ToString());
                            break;
                        }
                    }
                }
                conn.Close();
            }

            return user;
        }

        public static List<ObjectUser> List()
        {
            List<ObjectUser> list = new List<ObjectUser>();

            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM User;";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ObjectUser user = new ObjectUser();
                            user.Id = int.Parse(reader["Id"].ToString());
                            user.Nome = reader["Nome"].ToString();
                            user.Senha = reader["Senha"].ToString();
                            user.Tecnico = Convert.ToBoolean(reader["Tecnico"].ToString());
                            user.Tipo = int.Parse(reader["Tipo"].ToString());
                            list.Add(user);
                        }
                    }
                }
                conn.Close();
            }

            return list;
        }

        public static bool Validate(List<ObjectUser> lista, out string outMsg)
        {
            if (lista == null)
                throw new ArgumentNullException();

            StringBuilder validaItem = new StringBuilder();
            StringBuilder validaLista = new StringBuilder();

            //Seleciona somente habilitados           
            foreach (ObjectUser user in lista)
            {
                validaItem.Clear();

                //[Valida apenas colorantes habilitados]               

                if (string.IsNullOrEmpty(user.Nome))
                    validaItem.AppendLine(Negocio.IdiomaResxExtensao.Usuario_NomeObrigatorio);

                if (string.IsNullOrEmpty(user.Senha))
                    validaItem.AppendLine(Negocio.IdiomaResxExtensao.Usuario_SenhaObrigatorio);

                if (validaItem.Length > 0)
                {
                    string texto = string.Format(Negocio.IdiomaResxExtensao.Usuario_NomeMsg, user.Nome);
                    validaLista.AppendLine(texto);
                    validaLista.AppendLine(validaItem.ToString());
                }
            }

            outMsg = validaLista.ToString();
            return
                (validaLista.Length == 0);
        }

        public static void Persist(ObjectUser user)
        {
            ObjectUser objc = null;
            
            if (user.Id > 0)
            {
                objc = Load(user.Id);
            }
            
            //Insert
            if (objc == null)
            {
                StringBuilder sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        sb.Append("INSERT INTO User (Nome, Senha, Tecnico, Tipo) VALUES (");                           
                        sb.Append("'" + user.Nome.ToString() + "', ");
                        sb.Append("'" + user.Senha + "', ");
                        sb.Append("'" + (user.Tecnico ? "True" : "False") + "', ");
                        sb.Append("'" + user.Tipo.ToString() + "' ");
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
                        sb.Append("UPDATE User SET "); // (Motor, Nome, MassaEspecifica, Habilitado, Volume, Correspondencia, Dispositivo, NivelMinimo, NivelMaximo) VALUES (");
                        sb.Append("Nome = '" + user.Nome + "', ");
                        sb.Append("Senha = '" + user.Senha + "', ");
                        sb.Append("Tecnico ='" + (user.Tecnico ? "True" : "False") + "', ");
                        sb.Append("Tipo = '" + user.Tipo.ToString() + "' ");
                        sb.Append(" WHERE Id = " + user.Id.ToString() + ";");

                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                }
            }
        }

        public static void Persist(List<ObjectUser> lista)
        {
            if (lista != null && lista.Count > 0)
            {
                foreach (ObjectUser user in lista)
                {
                    ObjectUser objc = null;
                    if (user.Id > 0)
                    {
                        objc = Load(user.Id);
                    }
                    
                    //Insert
                    if (objc == null)
                    {
                        StringBuilder sb = new StringBuilder();
                        using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {
                                conn.Open();
                                sb.Append("INSERT INTO User  Nome, Senha, Tecnico, Tipo) VALUES (");                                    
                                sb.Append("'" + user.Nome.ToString() + "', ");
                                sb.Append("'" + user.Senha + "', ");
                                sb.Append("'" + (user.Tecnico ? "True" : "False") + "', ");
                                sb.Append("'" + user.Tipo.ToString() + "' ");
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
                                sb.Append("UPDATE User SET "); // (Motor, Nome, MassaEspecifica, Habilitado, Volume, Correspondencia, Dispositivo, NivelMinimo, NivelMaximo) VALUES (");
                                sb.Append("Nome = '" + user.Nome + "', ");
                                sb.Append("Senha = '" + user.Senha + "', ");
                                sb.Append("Tecnico ='" + (user.Tecnico ? "True" : "False") + "', ");
                                sb.Append("Tipo = '" + user.Tipo.ToString() + "' ");
                                sb.Append(" WHERE Id = " + user.Id.ToString() + ";");

                                cmd.CommandText = sb.ToString();

                                cmd.ExecuteNonQuery();

                                conn.Close();
                            }
                        }
                    }
                }
            }
        }

        public static bool User_Delete(int id)
        {
            bool retorno = false;
            
            StringBuilder sb = new StringBuilder();
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    sb.Append("Delete From User WHERE Id = "  + id.ToString() + ";");
                    cmd.CommandText = sb.ToString();

                    retorno = cmd.ExecuteNonQuery() > 0;
                    conn.Close();
                }
            }

            return retorno;
        }
        #endregion

    }
}