using Percolore.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Percolore.IOConnect
{
    public partial class fHistoricoValores : Form
    {
        private List<ValoresVO> _lista;
        private ValoresVO _valores;

        public ValoresVO Valores
        {
            get { return _valores; }
        }

        public fHistoricoValores(List<ValoresVO> lista)
        {
            InitializeComponent();
            _lista = lista;
        }

        #region Eventos

        private void Historico_Load(object sender, EventArgs e)
        {
            this.Text = Negocio.IdiomaResxExtensao.Historico_Titulo;
            this.lblHistoricoLegendaVolume.Text = Negocio.IdiomaResxExtensao.Historico_lblHistoricoLegendaVolume;

            lblHistoricoVolume.Text = _lista[0].Volume.ToString() + " ml";

            #region Configura ListView

            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.View = View.Details;
            listView.Font = new Font("Segoe UI Light", 16);
            listView.HeaderStyle = ColumnHeaderStyle.Clickable;

            listView.Columns.Add("", 0);
            listView.Columns.Add(Negocio.IdiomaResxExtensao.Historico_ColunaPulsos, 115, HorizontalAlignment.Center);
            listView.Columns.Add(Negocio.IdiomaResxExtensao.Historico_ColunaVelocidade, 115, HorizontalAlignment.Center);
            listView.Columns.Add(Negocio.IdiomaResxExtensao.Historico_ColunaDelay, 115, HorizontalAlignment.Center);
            listView.Columns.Add(Negocio.IdiomaResxExtensao.Historico_ColunaMassa, 115, HorizontalAlignment.Center);
            listView.Columns.Add(Negocio.IdiomaResxExtensao.Historico_ColunaDesvio, 115, HorizontalAlignment.Center);

            #endregion

            int index = 0;
            foreach (ValoresVO valores in _lista)
            {
                //Insere na lista
                ListViewItem item;
                item = new ListViewItem(new string[]
                    {
                        index.ToString(),
                        valores.PulsoHorario.ToString(),
                        valores.Velocidade.ToString(),
                        valores.Delay.ToString(),
                        valores.MassaMedia.ToString("N3"),
                        string.Format("{0:P2}", valores.DesvioMedio)
                    });

                listView.Items.Add(item);
                index++;
            }
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = listView.SelectedItems[0];
            int index = int.Parse(item.SubItems[0].Text);
            _valores = _lista[index];

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion     
    }
}



