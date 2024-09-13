using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Util
{
    public class ObjectPlacaMovimentacao
    {
        public List<Util.ObjectMotorPlacaMovimentacao> _pMotor = null;
        public int sensorCOPO { get; set; } = 0;            /*Sensor Copo Recipiente */
        public int sensorESPONJA { get; set; } = 0;         /*Sensor Copo Esponja*/
        public int sensorHighBICOS { get; set; } = 0;       /*Sensor Bicos Superior*/
        public int sensorLowBICOS { get; set; } = 0;        /*Sensor Bicos Inferior*/
        public int sensorAbertaGAVETA { get; set; } = 0;    /*Sensor Gaveta Aberta*/
        public int sensorFechadaGAVETA { get; set; } = 0;   /*Sensor Gaveta Fechada*/
        public int sensorAbertaVALVULA { get; set; } = 0;   /*Sensor Válvula Dosagem */
        public int sensorFechadaVALVULA { get; set; } = 0;  /*Sensor Válvula Recirculacão */

        public string PortaSerial { get; set; }
        public string AddresModbus { get; set; }

        public ObjectPlacaMovimentacao()
        {
            this._pMotor = new List<ObjectMotorPlacaMovimentacao>();
        }

        
    }
}
