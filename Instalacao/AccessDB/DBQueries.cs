using System.Data;

namespace Percolore.Instalacao
{
	public class Queries
    {
        AccessDatabase db;
        public Queries(string dbfile)
        {
            db = new AccessDatabase(dbfile);
        }

        public DbParametro GetParametro()
        {
            DataTable dt;
            DbParametro model = new DbParametro();

            try
            {
                dt = db.QueryDatatable("SELECT * FROM Parametros");

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    model.ComPort = byte.Parse(row["ComPort"].ToString());
                    model.ReverseDelay = int.Parse(row["ReverseDelay"].ToString());
                    model.ReverseSteps = int.Parse(row["DripControlVolume"].ToString());
                }

                return model;
            }
            catch
            {
                throw;
            }
        }

        public List<DbPigmento> GetPigmentos()
        {
            DataTable dt = null;
            List<DbPigmento> lista = new List<DbPigmento>();

            try
            {
                dt = db.QueryDatatable("SELECT * FROM Pigmentos ORDER BY Posicao ASC");

                /* Testa se as informações referentes ao reverse existem na tabela Pigmentos
                 * e, caso não existam, busca na tabela Parametros. */
                bool REVERSE_PARAMETROS = false;
                DbParametro dbParametro = null;
                if (dt.Columns["ReverseDelay"] == null)
                {
                    REVERSE_PARAMETROS = true;
                    dbParametro = this.GetParametro();
                }

                if ((dt != null) && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DbPigmento model = new DbPigmento();
                        model.Nome = row["Descrição"].ToString();
                        model.PesoEspecificao = (double)row["PesoEspecifico"];
                        model.Velocidade = (int)row["Velocidade"];
                        model.Aceleracao = (int)row["Aceleracao"];
                        model.Posicao = (int)row["Posicao"];
                        model.FatorEscalaCorrente = (int)row["FatorEscalaCorrente"];

                        if (REVERSE_PARAMETROS)
                        {
                            model.ReverseDelay = dbParametro.ReverseDelay;
                            model.ReverseSteps = dbParametro.ReverseSteps;
                        }
                        else
                        {
                            model.ReverseDelay = (int)row["ReverseDelay"];
                            model.ReverseSteps = (int)row["ReverseSteps"];
                        }

                        lista.Add(model);
                    }
                }

                return lista;
            }
            catch
            {
                throw;
            }
        }
    }
}