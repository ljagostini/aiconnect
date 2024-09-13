
using PaintMixer;
using Percolore.Core;
using Percolore.Core.Persistence.Xml;
using Percolore.IOConnect.Modbus;
using System;
using System.Collections.Generic;

namespace Percolore.IOConnect
{
    public class ModBusDispenser_P1 : IDispenser, IDisposable
    {
        private PaintMixerInterface_P1 mixer;
        private Util.ObjectParametros parametros = null;
        private bool isBusy = false;
        private bool mayValve = false;

        public bool MayValve
        {
            get { return mayValve; }
            set { mayValve = value; }
        }
        private int idDispositivo;

        public int IdDispositivo
        {
            get { return idDispositivo; }
            set { idDispositivo = value; }
        }

        public ModBusDispenser_P1()
        {
            parametros = Util.ObjectParametros.Load();
            mixer = new PaintMixerInterface_P1(parametros.Slave);
        }

        #region Propriedades

        public bool IsReady
        {
            get
            {
                return !mixer.Status.Busy;
            }
        }

        public bool IsBusy
        {
            get { return isBusy; }
        }

        public bool FirstInput
        {
            get { return true; }
        }

        public bool IdentityPot
        {
            get { return false; }
        }

        public bool IdentitySponge
        {
            get { return false; }
        }

        public bool Connected
        {
            get { return false; }
        }     

        #endregion

        #region Métodos públicos

        public void Connect()
        {
            mixer.Connect(
                this.parametros.ResponseTimeout);
        }

        public void Disconnect()
        {
            mixer.Disconnect();
        }

        public void Halt()
        {
            mixer.Halt();
        }

        public void UnHalt()
        {
            mixer.Unhalt();
        }

        /// <summary>
        /// Mapeia objeto com valores de calibragem para array e envia para a placa
        /// </summary>
        public void Dispensar(ValoresVO[] valores, int i_Step = 0)
        {
            throw new NotImplementedException();

            //bool t = true;

            //int[] dist = new int[12];
            //int[] vel = new int[12];
            //int[] ace = new int[12];
            //int[] revDelay = new int[12];
            //int[] revPulsos = new int[12];

            ///*Percorre posições do vetor com valores de calibragem que não sejam null 
            //e preenche array de parâmetros do mixer*/
            //for (int i = 0; i <= valores.GetUpperBound(0); i++)
            //{
            //    ValoresVO v = valores[i];
            //    if (v != null)
            //    {
            //        dist[i] = v.PulsoHorario;
            //        vel[i] = v.Velocidade;
            //        ace[i] = v.Aceleracao;
            //        revDelay[i] = v.Delay;
            //        revPulsos[i] = v.PulsoReverso;
            //    }
            //}

            //mixer.RunReverse(dist, vel, ace, revDelay, revPulsos);
        }

        /// <summary>
        /// Envia para a placa todas as posições do array passado como parâmetro
        /// </summary>
        public void Dispensar(
            int[] pulsoHorario, int[] velocidade, int[] aceleracao, int[] delay, int[] pulsoReverso, int i_Step = 0)
        {
            //bool t = true;

            //mixer.RunReverse(
            //    pulsoHorario, velocidade, aceleracao, delay, pulsoReverso);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Envia para a placa apenas os valores da posição informada em motor
        /// </summary>
        public void Dispensar(int motor, int pulso, int velocidade, int aceleracao, int delay, int pulso_reverso, int i_Step = 0)
        {            
            int[] vPulso = new int[12];
            int[] vVelocidade = new int[12];
            int[] vAceleracao = new int[12];
            int[] vDelay = new int[12];
            int[] vPulsoReverso = new int[12];

            //Informa apenas para motor passado como parâmetro
            vPulso[motor] = pulso;
            vVelocidade[motor] = velocidade;
            vAceleracao[motor] = aceleracao;
            //vDelay[motor] = delay;
            //vPulsoReverso[motor] = pulso_reverso;

            mixer.RunReverse(
                vPulso, vVelocidade, vAceleracao, delay, pulso_reverso);
        }

        /// <summary>
        /// Envia para a placa apenas os valores da posição informada em motor
        /// </summary>
        public void Dispensar(int[] motor, int pulso, int velocidade, int aceleracao, int delay, int pulso_reverso, int i_Step = 0)
        {
            int[] vPulso = new int[12];
            int[] vVelocidade = new int[12];
            int[] vAceleracao = new int[12];
            int[] vDelay = new int[12];
            int[] vPulsoReverso = new int[12];

            for (int i = 0; i < motor.Length; i++)
            {
                //Informa apenas para motor passado como parâmetro
                vPulso[motor[i]] = pulso;
                vVelocidade[motor[i]] = velocidade;
                vAceleracao[motor[i]] = aceleracao;
                //vDelay[motor] = delay;
                //vPulsoReverso[motor] = pulso_reverso;
            }
            mixer.RunReverse(
                vPulso, vVelocidade, vAceleracao, delay, pulso_reverso);
        }

        /// <summary>
        /// Envia para a placa apenas as posições do array passado como parâmetro em grupos de 5.
        /// A utilidade desse métodos é executar uma dispensa sequencial de grupos.
        /// </summary>
        /// <param name="iniciar_reverso">Determina se o primeiro movimento é reverso</param>       
        public void Dispensar(
            int[] pulso, int velocidade, int aceleracao, bool iniciar_reverso, int qtdeElementosGrupo, int i_Step = 0)
        {
            throw new NotImplementedException();
        }

        public void RessetDosed()
        {

        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                mixer.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //Metodos criados somente para sinalizar, não existe valvulas
        public StatusValvulas getStatusValvulas()
        {
            StatusValvulas retorno = new StatusValvulas();

            return retorno;
        }

        public void AcionaValvula(bool aVal, int circuito)
        {

        }

        public void AcionaValvulasRecirculacao(List<int> lcircuito)
        {
        }

        public void AcionaValvulas(bool allVal)
        {

        }

        public void AcionaValvulas(int nval)
        {

        }
    }
}
