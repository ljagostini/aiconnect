namespace UpdateConfig.Model
{
	public class Mode06
    {
        public Mode06(string name, string volume, string circuito)
        {
            this.Name = name;
            this.Volume = volume;
            this.Circuito = circuito;
        }
        public string Name { get; set; }
        public string Volume { get; set; }
        public string Circuito { get; set; }
        
    }
}
