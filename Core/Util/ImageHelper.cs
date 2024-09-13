using System.Drawing;
using System.Drawing.Imaging;

namespace Percolore.Core.Util
{
	/// <summary>
	/// Rotinas de apoio referentes à imagens.
	/// </summary>
	public static class ImageHelper
    {
        /// <summary>
        /// Converte uma imagem em stringBase64.
        /// </summary>
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //Recupera array de bytes da imagem
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                //Convert array de bytes em Base64String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        /// <summary>
        /// Converte uma stringBase64 em imagem.
        /// </summary>
        public static Image Base64ToImage(string base64String)
        {
            //Extrai buytes da string
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            //Converte bytes em imagem
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);

            return image;
        }
    }
}