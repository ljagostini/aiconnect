using Percolore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Core
{
    public class UnidadeMedidaHelper
    {
        #region Conversão

        /// <summary>
        /// Converte mililitros para 48 partes de onça
        /// </summary>
        public static double MililitroToOnca(double mililitros)
        {
            return
                mililitros / Metrica.FatorOncaToMl;
        }

        /// <summary>
        /// Recebe mililitros e converte para onça fracionada
        /// </summary>
        public static void MililitroToOncaFracionada(double mililitros, out int y, out double _48)
        {
            double oncas = MililitroToOnca(mililitros);
            OncaToOncaFracionada(oncas, out y, out _48);
        }

        /// <summary>
        /// Recebe mililitros e converte para shot
        /// </summary>
        /// <param name="fatorShotToMl">Fator de conversão de shot para mililitros. 
        /// Este fator é definido nas configurações do software.</param>
        public static double MililitroToShot(double mililitros, double fatorShotToMl)
        {
            return mililitros / fatorShotToMl;
        }

        /// <summary>
        /// Recebe mililitros e converte para gramas
        /// </summary>
        public static double MililitroToGrama(double mililitros, double massa_especifica)
        {
            return mililitros * massa_especifica;
        }

        /// <summary>
        /// Recebe 48 partes de onça e converte para mililitros
        /// </summary>
        public static double OncaToMililitro(double onca_48)
        {
            return
                onca_48 * Metrica.FatorOncaToMl;
        }

        /// <summary>
        /// Recebe valor fracionado e converte para mililitros
        /// </summary>
        public static double OncaFracionadaToMililitro(int y, double _48)
        {
            double onca = OncaFracionadaToOnca(y, _48);
            return
                OncaToMililitro(onca);
        }

        /// <summary>
        /// Recebe valor fracionado e converte para 48 partes de onca
        /// </summary>
        public static double OncaFracionadaToOnca(int y, double _48)
        {
            return (y * 48) + _48;
        }

        /// <summary>
        /// Recebe valor de onças em 48 pates e convete para fração
        /// </summary>
        public static void OncaToOncaFracionada(double oncas, out int y, out double _48)
        {
            y = (int)Math.Floor(oncas / 48);
            _48 = oncas % 48;
        }

        /// <summary>
        /// Converte gramas para mililitros
        /// </summary>
        public static double GramaToMililitro(double grama, double massa_especifica)
        {
            return
                grama / massa_especifica;
        }

        /// <summary>
        /// Converte shot para mililitros
        /// </summary>
        public static double ShotToMililitro(double shot, double fatorShotToMl)
        {
            return
                shot * fatorShotToMl;
        }

        #endregion

        #region Formatação

        /// <summary>
        /// Formata onça com número inteiro e 48 partes decimais. ex.: 9Y + 9/48
        /// </summary>
        public static string FormatarOnca(int y, double _48, IFormatProvider formatProvider = null)
        {
            string onca = string.Empty;

            if (y > 0)
                onca = y.ToString("0Y");

            if (_48 > 0)
            {
                if (y > 0)
                    onca += " + ";

                onca += _48.ToString("0.###/48", formatProvider);
            }

            return onca;
        }

        /// <summary>
        /// Formata shot com 3 casas decimais
        /// </summary>
        public static string FormatShot(
            double shot, bool abreviacao = false, IFormatProvider formatProvider = null)
        {
            string format =
                shot.ToString("0.###", formatProvider);

            if (abreviacao)
                format += " shot";

            return format;
        }

        /// <summary>
        /// Formata mililitro com 3 casas decimais
        /// </summary>
        public static string FormatMililitro(
            double mililitro, bool abreviacao, IFormatProvider formatProvider = null)
        {
            string format =
                mililitro.ToString("0.###", formatProvider);

            if (abreviacao)
                format += " ml";

            return format;
        }

        /// <summary>
        /// Formata mililitro com 3 casas decimais
        /// </summary>
        public static string FormatGrama(
            double grama, bool abreviacao, IFormatProvider formatProvider = null)
        {
            string format =
                grama.ToString("0.###", formatProvider);

            if (abreviacao)
                format += " g";

            return format;
        }

        #endregion
    }
}
