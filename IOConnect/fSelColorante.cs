using System.Data;

namespace Percolore.IOConnect
{
	public partial class fSelColorante : Form
    {
        private Util.ObjectColorante _colorante { get; set; }
        private List<ListItemSel> _ListItemSel = new List<ListItemSel>();
        public List<int> listCircuitos { get; set; } 

        public fSelColorante(Util.ObjectColorante colorante)
        {
            InitializeComponent();
            this._colorante = colorante;
            this.listCircuitos = new List<int>();
        }

        private void fSelColorante_Load(object sender, EventArgs e)
        {
            try
            {
                lblNomeColorante.Text = "Colorante: " + this._colorante.Nome;
                List<Util.ObjectColorante> _listCol = Util.ObjectColorante.List();

                _listCol = _listCol.Where(c => c.Habilitado == true && c.Seguidor < 0).ToList();
                for(int i = 0; i < _listCol.Count; i++)
                {
                    if(_listCol[i].Circuito == this._colorante.Circuito)
                    {
                        _listCol.RemoveAt(i);
                        break;
                    }
                }
                foreach(Util.ObjectColorante _col in _listCol)
                {
                    ListItemSel li = new ListItemSel();
                    li.Circuito = _col.Circuito;
                    li.Descricao = _col.Nome;
                    li.IsSelecionado = false;
                    this._ListItemSel.Add(li);
                }
                if (chlColorantes.Items != null && chlColorantes.Items.Count > 0)
                {
                    chlColorantes.Items.Clear();
                }
                foreach(ListItemSel li in this._ListItemSel)
                {
                    chlColorantes.Items.Add(li.Descricao, li.IsSelecionado);
                }


            }
            catch
            { }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            foreach (ListItemSel li in this._ListItemSel)
            {
                if(li.IsSelecionado)
                {
                    this.listCircuitos.Add(li.Circuito);
                }
            }
            if (this.listCircuitos.Count > 0)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                using (fMensagem m = new fMensagem(fMensagem.TipoMensagem.Informacao))
                {
                    m.ShowDialog("Usuário deve Selecionar Colorantes.", Negocio.IdiomaResxExtensao.Global_Sim);
                }
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void chlColorantes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {
                if (e.NewValue == CheckState.Checked)
                {
                    this._ListItemSel[e.Index].IsSelecionado = true;
                }
                else
                {
                    this._ListItemSel[e.Index].IsSelecionado = false;
                }
            }
            catch
            { }
        }

        private class ListItemSel
        {
            public int Circuito { get; set; }
            public string Descricao { get; set; }
            public bool IsSelecionado { get; set; }
        }
    }
    
}
