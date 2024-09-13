namespace Percolore.Core
{
    public enum Dispositivo
    {
        [ResourceNameAtributte("Global_Dispositivo_Simulador")]
        Simulador = 1,
        [ResourceNameAtributte("Global_Dispositivo_Placa_1")]
        Placa_1 = 2,
        [ResourceNameAtributte("Global_Dispositivo_Placa_2")]
        Placa_2 = 3,
        [ResourceNameAtributte("Global_Dispositivo_Placa_3")]
        Placa_3 = 4
        //[ResourceNameAtributte("Global_Dispositivo_Placa_4")]
        //Placa_4 = 5
    }

    public enum Dispositivo2
    {
        [ResourceNameAtributte("Global_Dispositivo_Nenhum")]
        Nenhum = 0,
        [ResourceNameAtributte("Global_Dispositivo_Simulador")]
        Simulador = 1,
        [ResourceNameAtributte("Global_Dispositivo_Placa_2")]
        Placa_2 = 3
        //[ResourceNameAtributte("Global_Dispositivo_Placa_4")]
        //Placa_4 = 5
    }
}