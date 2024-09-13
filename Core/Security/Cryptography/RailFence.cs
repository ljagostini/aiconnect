namespace Percolore.Core.Security.Cryptography
{
	public static class RailFence
    {
        public static string Encrypt(string plainText, out int rail)
        {
            //Os valores que surtiram efeito foram entre 3 e 9
            rail = new Random().Next(3, 9);

            if (string.IsNullOrWhiteSpace(plainText))
            {
                return plainText;
            }

            List<string> railFence = new List<string>();
            for (int i = 0; i < rail; i++)
            {
                railFence.Add("");
            }

            int number = 0;
            int increment = 1;
            foreach (char c in plainText)
            {
                if (number + increment == rail)
                {
                    increment = -1;
                }
                else if (number + increment == -1)
                {
                    increment = 1;
                }
                railFence[number] += c;
                number += increment;
            }

            string buffer = "";
            foreach (string s in railFence)
            {
                buffer += s;
            }
            return buffer;
        }

        public static string Decrypt(string cipherText, int rail)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
            {
                return cipherText;
            }

            //Os valoes permitidos estão entre 1 e 9
            if (rail < 3 || rail > 9)
            {
                return cipherText;
            }
                
            int cipherLength = cipherText.Length;
            List<List<int>> railFence = new List<List<int>>();
            for (int i = 0; i < rail; i++)
            {
                railFence.Add(new List<int>());
            }

            int number = 0;
            int increment = 1;
            for (int i = 0; i < cipherLength; i++)
            {
                if (number + increment == rail)
                {
                    increment = -1;
                }
                else if (number + increment == -1)
                {
                    increment = 1;
                }
                railFence[number].Add(i);
                number += increment;
            }

            int counter = 0;
            char[] buffer = new char[cipherLength];
            for (int i = 0; i < rail; i++)
            {
                for (int j = 0; j < railFence[i].Count; j++)
                {
                    buffer[railFence[i][j]] = cipherText[counter];
                    counter++;
                }
            }

            return new string(buffer);
        }
    }
}