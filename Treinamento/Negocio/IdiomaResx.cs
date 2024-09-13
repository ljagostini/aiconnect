namespace Percolore.Treinamento.Negocio
{
	public static class IdiomaResx
    {
        #region parametros
        public static string Abastecimento_001_Texto;
        public static string Abastecimento_001_Titulo;
        public static string Abastecimento_002_Texto;
        public static string Abastecimento_002_Titulo;
        public static string Abertura_Texto;
        public static string Abertura_Titulo;
        public static string CuidadosBasicos_001_Texto;
        public static string CuidadosBasicos_001_Titulo;
        public static string CuidadosBasicos_002_Texto;
        public static string CuidadosBasicos_002_Titulo;
        public static string CuidadosBasicos_003_Texto;
        public static string CuidadosBasicos_003_Titulo;
        public static string Dosagem_001_Texto;
        public static string Dosagem_001_Titulo;
        public static string Encerramento_001_Texto;
        public static string Encerramento_001_Titulo;
        public static string Purga_001_Texto;
        public static string Purga_001_Titulo;
        public static string RegistroAbastecimento_001_Texto;
        public static string RegistroAbastecimento_001_Titulo;
        public static string UI_Titulo;
        #endregion
        //Idioma = 1 Portugues
        //Idioma = 2 Espanhol
        //Idioma = 3 Ingles
        public static void GetIDiomaREsx(int idIdioma)
        {
            if(idIdioma == 2)
            {
                Abastecimento_001_Texto = Treinamento.Properties.Treinamento_String_esp.Abastecimento_001_Texto;
                Abastecimento_001_Titulo = Treinamento.Properties.Treinamento_String_esp.Abastecimento_001_Titulo;
                Abastecimento_002_Texto = Treinamento.Properties.Treinamento_String_esp.Abastecimento_002_Texto;
                Abastecimento_002_Titulo = Treinamento.Properties.Treinamento_String_esp.Abastecimento_002_Titulo;
                Abertura_Texto = Treinamento.Properties.Treinamento_String_esp.Abertura_Texto;
                Abertura_Titulo = Treinamento.Properties.Treinamento_String_esp.Abertura_Titulo;
                CuidadosBasicos_001_Texto = Treinamento.Properties.Treinamento_String_esp.CuidadosBasicos_001_Texto;
                CuidadosBasicos_001_Titulo = Treinamento.Properties.Treinamento_String_esp.CuidadosBasicos_001_Titulo;
                CuidadosBasicos_002_Texto = Treinamento.Properties.Treinamento_String_esp.CuidadosBasicos_002_Texto;
                CuidadosBasicos_002_Titulo = Treinamento.Properties.Treinamento_String_esp.CuidadosBasicos_002_Titulo;
                CuidadosBasicos_003_Texto = Treinamento.Properties.Treinamento_String_esp.CuidadosBasicos_003_Texto;
                CuidadosBasicos_003_Titulo = Treinamento.Properties.Treinamento_String_esp.CuidadosBasicos_003_Titulo;
                Dosagem_001_Texto = Treinamento.Properties.Treinamento_String_esp.Dosagem_001_Texto;
                Dosagem_001_Titulo = Treinamento.Properties.Treinamento_String_esp.Dosagem_001_Titulo;
                Encerramento_001_Texto = Treinamento.Properties.Treinamento_String_esp.Encerramento_001_Texto;
                Encerramento_001_Titulo = Treinamento.Properties.Treinamento_String_esp.Encerramento_001_Titulo;
                Purga_001_Texto = Treinamento.Properties.Treinamento_String_esp.Purga_001_Texto;
                Purga_001_Titulo = Treinamento.Properties.Treinamento_String_esp.Purga_001_Titulo;
                RegistroAbastecimento_001_Texto = Treinamento.Properties.Treinamento_String_esp.RegistroAbastecimento_001_Texto;
                RegistroAbastecimento_001_Titulo = Treinamento.Properties.Treinamento_String_esp.RegistroAbastecimento_001_Titulo;
                UI_Titulo = Treinamento.Properties.Treinamento_String_esp.UI_Titulo;
            }
            else if (idIdioma == 3)
            {
                Abastecimento_001_Texto = Treinamento.Properties.Treinamento_String_eng.Abastecimento_001_Texto;
                Abastecimento_001_Titulo = Treinamento.Properties.Treinamento_String_eng.Abastecimento_001_Titulo;
                Abastecimento_002_Texto = Treinamento.Properties.Treinamento_String_eng.Abastecimento_002_Texto;
                Abastecimento_002_Titulo = Treinamento.Properties.Treinamento_String_eng.Abastecimento_002_Titulo;
                Abertura_Texto = Treinamento.Properties.Treinamento_String_eng.Abertura_Texto;
                Abertura_Titulo = Treinamento.Properties.Treinamento_String_eng.Abertura_Titulo;
                CuidadosBasicos_001_Texto = Treinamento.Properties.Treinamento_String_eng.CuidadosBasicos_001_Texto;
                CuidadosBasicos_001_Titulo = Treinamento.Properties.Treinamento_String_eng.CuidadosBasicos_001_Titulo;
                CuidadosBasicos_002_Texto = Treinamento.Properties.Treinamento_String_eng.CuidadosBasicos_002_Texto;
                CuidadosBasicos_002_Titulo = Treinamento.Properties.Treinamento_String_eng.CuidadosBasicos_002_Titulo;
                CuidadosBasicos_003_Texto = Treinamento.Properties.Treinamento_String_eng.CuidadosBasicos_003_Texto;
                CuidadosBasicos_003_Titulo = Treinamento.Properties.Treinamento_String_eng.CuidadosBasicos_003_Titulo;
                Dosagem_001_Texto = Treinamento.Properties.Treinamento_String_eng.Dosagem_001_Texto;
                Dosagem_001_Titulo = Treinamento.Properties.Treinamento_String_eng.Dosagem_001_Titulo;
                Encerramento_001_Texto = Treinamento.Properties.Treinamento_String_eng.Encerramento_001_Texto;
                Encerramento_001_Titulo = Treinamento.Properties.Treinamento_String_eng.Encerramento_001_Titulo;
                Purga_001_Texto = Treinamento.Properties.Treinamento_String_eng.Purga_001_Texto;
                Purga_001_Titulo = Treinamento.Properties.Treinamento_String_eng.Purga_001_Titulo;
                RegistroAbastecimento_001_Texto = Treinamento.Properties.Treinamento_String_eng.RegistroAbastecimento_001_Texto;
                RegistroAbastecimento_001_Titulo = Treinamento.Properties.Treinamento_String_eng.RegistroAbastecimento_001_Titulo;
                UI_Titulo = Treinamento.Properties.Treinamento_String_eng.UI_Titulo;
            }
            else
            {
                Abastecimento_001_Texto = Treinamento.Properties.Treinamento_String.Abastecimento_001_Texto;
                Abastecimento_001_Titulo = Treinamento.Properties.Treinamento_String.Abastecimento_001_Titulo;
                Abastecimento_002_Texto = Treinamento.Properties.Treinamento_String.Abastecimento_002_Texto;
                Abastecimento_002_Titulo = Treinamento.Properties.Treinamento_String.Abastecimento_002_Titulo;
                Abertura_Texto = Treinamento.Properties.Treinamento_String.Abertura_Texto;
                Abertura_Titulo = Treinamento.Properties.Treinamento_String.Abertura_Titulo;
                CuidadosBasicos_001_Texto = Treinamento.Properties.Treinamento_String.CuidadosBasicos_001_Texto;
                CuidadosBasicos_001_Titulo = Treinamento.Properties.Treinamento_String.CuidadosBasicos_001_Titulo;
                CuidadosBasicos_002_Texto = Treinamento.Properties.Treinamento_String.CuidadosBasicos_002_Texto;
                CuidadosBasicos_002_Titulo = Treinamento.Properties.Treinamento_String.CuidadosBasicos_002_Titulo;
                CuidadosBasicos_003_Texto = Treinamento.Properties.Treinamento_String.CuidadosBasicos_003_Texto;
                CuidadosBasicos_003_Titulo = Treinamento.Properties.Treinamento_String.CuidadosBasicos_003_Titulo;
                Dosagem_001_Texto = Treinamento.Properties.Treinamento_String.Dosagem_001_Texto;
                Dosagem_001_Titulo = Treinamento.Properties.Treinamento_String.Dosagem_001_Titulo;
                Encerramento_001_Texto = Treinamento.Properties.Treinamento_String.Encerramento_001_Texto;
                Encerramento_001_Titulo = Treinamento.Properties.Treinamento_String.Encerramento_001_Titulo;
                Purga_001_Texto = Treinamento.Properties.Treinamento_String.Purga_001_Texto;
                Purga_001_Titulo = Treinamento.Properties.Treinamento_String.Purga_001_Titulo;
                RegistroAbastecimento_001_Texto = Treinamento.Properties.Treinamento_String.RegistroAbastecimento_001_Texto;
                RegistroAbastecimento_001_Titulo = Treinamento.Properties.Treinamento_String.RegistroAbastecimento_001_Titulo;
                UI_Titulo = Treinamento.Properties.Treinamento_String.UI_Titulo;
            }
        }
    }
}