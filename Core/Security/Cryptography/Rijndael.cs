using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Percolore.Core.Security.Cryptography
{
    public class AES
    {
        /// <summary>     
        /// Vetor de bytes utilizados para a criptografia (Chave Externa)
        /// </summary>     
        private static byte[] bIV = {
            0x50, 0x08, 0xF1, 0xDD, 0xDE, 0x3C, 0xF2, 0x18, 0x44, 0x74, 0x19, 0x2C, 0x53, 0x49, 0xAB, 0xBC };

        /// <summary>     
        /// Representação de valor em base 64 (Chave Interna)    
        /// O Valor representa a transformação para base64 de     
        /// um conjunto de 32 caracteres (8 * 32 = 256bits)    
        /// A chave é um GUID: "cd283210ca4147f081ef28a96e34708c"     
        /// </summary>     
        private const string cryptoKey =
            "Y2QyODMyMTBjYTQxNDdmMDgxZWYyOGE5NmUzNDcwOGM=";

		/// <summary>     
		/// Metodo de criptografia de valor     
		/// </summary>     
		/// <param name="text">valor a ser criptografado</param>     
		/// <returns>valor criptografado</returns>
		public static string Encrypt(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				byte[] bKey = Convert.FromBase64String(cryptoKey);
				byte[] bText = new UTF8Encoding().GetBytes(text);

				using (Aes aes = Aes.Create())
				{
					aes.Padding = PaddingMode.PKCS7;
					aes.Mode = CipherMode.CBC;
					aes.KeySize = 128;
					aes.BlockSize = 128;

					using (MemoryStream mStream = new MemoryStream())
					using (CryptoStream encryptor = new CryptoStream(mStream, aes.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write))
					{
						encryptor.Write(bText, 0, bText.Length);
						encryptor.FlushFinalBlock();
						return Convert.ToBase64String(mStream.ToArray());
					}
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>     
		/// Pega um valor previamente criptografado e retorna o valor inicial 
		/// </summary>     
		/// <param name="text">texto criptografado</param>     
		/// <returns>valor descriptografado</returns>     
		public static string Decrypt(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				byte[] bKey = Convert.FromBase64String(cryptoKey);
				byte[] bText = Convert.FromBase64String(text);

				using (Aes aes = Aes.Create())
				{
					aes.Padding = PaddingMode.PKCS7;
					aes.Mode = CipherMode.CBC;
					aes.KeySize = 128;
					aes.BlockSize = 128;

					using (MemoryStream mStream = new MemoryStream())
					using (CryptoStream decryptor = new CryptoStream(mStream, aes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write))
					{
						decryptor.Write(bText, 0, bText.Length);
						decryptor.FlushFinalBlock();

						UTF8Encoding utf8 = new UTF8Encoding();
						return utf8.GetString(mStream.ToArray());
					}
				}
			}
			else
			{
				return null;
			}
		}
	}
}
