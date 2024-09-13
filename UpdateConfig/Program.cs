using Newtonsoft.Json;
using System.Data;

namespace UpdateConfig
{
	class Program
    {
        static void Main()
        {
            try
            {
                Console.WriteLine("Wait for the mode update to finish.");
                
                string strReader = string.Empty;
                if (File.Exists("obj.json"))
                {
					using StreamReader sr = new("obj.json");
					strReader = sr.ReadToEnd();
					sr.Close();
				}
                if (!string.IsNullOrEmpty( strReader))
                {
                    Model.ObjectGlobal obj = JsonConvert.DeserializeObject<Model.ObjectGlobal>(strReader);
                    if (obj != null)
                    {
                        if (obj.parametros != null && obj.parametros.Count > 0 && File.Exists(Util.ObjectParametros.PathFile))
                        {
                            DataSet dsTable = Util.ObjectParametros.GetTables();
                            for (int i = 0; i < obj.parametros.Count; i++)
                            {
                                Util.ObjectParametros.SetExecucaoParameter(obj.parametros[i].Campo, "'" + obj.parametros[i].Valor + "'", dsTable);
                            }
                        }

                        if (obj.mode6 != null && obj.mode6.Count > 0 && File.Exists(Util.ObjectBasDat06.PathFile))
                        {
                            List<Util.ObjectBasDat06> lmd = [];
                            for (int i = 0; i < obj.mode6.Count; i++)
                            {
								Util.ObjectBasDat06 md = new()
								{
									Circuito = Convert.ToInt32(obj.mode6[i].Circuito),
									Volume = double.Parse(obj.mode6[i].Volume, System.Globalization.CultureInfo.InvariantCulture),
									Name = obj.mode6[i].Name
								};
								lmd.Add(md);
                            }
                            if (lmd.Count > 0)
                            {
                                Util.ObjectBasDat06.Persist(lmd);
                            }
                        }

                        if (obj.mode5 != null && obj.mode5.Count > 0 && File.Exists(Util.ObjectBasDat05.PathFile))
                        {
                            List<Util.ObjectBasDat05> lmd = [];
                            for (int i = 0; i < obj.mode5.Count; i++)
                            {
								Util.ObjectBasDat05 md = new()
								{
									Circuito = Convert.ToInt32(obj.mode5[i].Circuito),
									Volume = double.Parse(obj.mode5[i].Volume, System.Globalization.CultureInfo.InvariantCulture),
									Name = obj.mode5[i].Name
								};
								lmd.Add(md);
                            }
                            if (lmd.Count > 0)
                            {
                                Util.ObjectBasDat05.Persist(lmd);
                            }
                        }

                        if (obj.corantes != null && obj.corantes.Count > 0 && File.Exists(Util.ObjectCorante.PathFile))
                        {
                            List<Util.ObjectCorante> lcor = [];
                            for (int i = 0; i < obj.corantes.Count; i++)
                            {
								Util.ObjectCorante cor = new()
								{
									Circuito = Convert.ToInt32(obj.corantes[i].Motor),
									IsBase = obj.corantes[i].IsBase == "True"
								};

								lcor.Add(cor);
                            }
                            if (lcor.Count > 0)
                            {
                                Util.ObjectCorante.Persist(lcor);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Um erro ocorreu: {e}");
            }
        }
    }
}