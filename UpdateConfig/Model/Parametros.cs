namespace UpdateConfig.Model
{
	public class Parametros
    {
        public Parametros(string campo, string valor)
        {
            this.Campo = campo;
            this.Valor = valor;
        }
        public string Campo { get; set; }
        public string Valor { get; set; }
    }
}
