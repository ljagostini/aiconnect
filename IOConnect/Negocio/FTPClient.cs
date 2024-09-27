using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Negocio
{
    public static class FTPClient
    {
        /// <summary>
        /// Upload de arquivo
        /// </summary>
        /// <param name="arquivo"></param>
        /// <param name="url"></param>
        /// <param name="usuario"></param>
        /// <param name="senha"></param>
        public static bool EnviarArquivoFTP(string arquivo, string url, string usuario, string senha, ref string strError)
        {
            bool retorno = false;
            try
            {
                strError = "";
                FileInfo arquivoInfo = new FileInfo(arquivo);

                using var ftpClient = new FtpClient(new Uri(url).Host, usuario, senha);
				ftpClient.Connect();

                using FileStream fs = arquivoInfo.OpenRead();
				FtpStatus status = ftpClient.UploadStream(fs, url, FtpRemoteExists.Overwrite);
				retorno = status == FtpStatus.Success;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }
            return retorno;
        }

        /// <summary>
        /// Find arquivo
        /// </summary>
        /// <param name="url"></param>
        /// <param name="usuario"></param>
        /// <param name="senha"></param>
        public static bool ExisteArquivoFTP(string url, string usuario, string senha)
        {
            bool retorno = false;

            using var ftpClient = new FtpClient(new Uri(url).Host, usuario, senha);
			ftpClient.Connect();
			retorno = ftpClient.FileExists(url);

            return retorno;
        }
    }
}
