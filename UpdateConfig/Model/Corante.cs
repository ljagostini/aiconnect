namespace UpdateConfig.Model
{
	public class Corante
    {
        public Corante(string motor, string isbase)
        {
            this.Motor = motor;
            this.IsBase = isbase;
        }
        public string Motor { get; set; }
        public string IsBase { get; set; }

    }
}