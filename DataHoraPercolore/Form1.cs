using Percolore.Core.Persistence.WindowsRegistry;

namespace DataHoraPercolore
{
	public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAjustaDataHoraPerc_Click(object sender, EventArgs e)
        {
            PercoloreRegistry percRegistry = new PercoloreRegistry();
            try
            {
                //grava data e hora locais no registro
                percRegistry.SetDataHoraMaquina();
                MessageBox.Show("Data Hora Ajustada corretamente!");
            }
            catch(Exception exc)
            {
                MessageBox.Show("Data Hora erro: " + exc.Message);
            }
            finally
            {
                percRegistry.Dispose();
            }
        }
    }
}