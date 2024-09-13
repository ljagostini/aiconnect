using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Modbus
{
    public interface IMover
    {
        void ReadSensores_Mover(); 

        void Connect_Mover();

        void Disconnect_Mover();

        //isForward true avanca, false recuar 
        void MovimentarManual(int motor, bool isForward);

        void MovimentarAutomatico();

    }
}
