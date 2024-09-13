namespace Percolore.Core.Util
{
	/// <summary>
	/// Rotinas de poaio referentes a arquivos.
	/// </summary>
	public static class FileHelper
    {
        static readonly HashSet<char> invalidFileNameChars =
            new HashSet<char>(Path.GetInvalidFileNameChars());

        public static bool ContainsInvalidChars(string name)
        {
            return
                name.Any(c => invalidFileNameChars.Contains(c));
        }

        public static string RemoveInvalidChars(string name)
        {
            return new string(
                name.Where(c => !invalidFileNameChars.Contains(c)).ToArray());
        }

        /// <summary>
        /// Renomeia arquivo de acordo com parâmetros
        /// </summary>
        /// <param name="path">Caminho do arquivo.</param>
        /// <param name="moveDirectory">Diretório de destino do arquivo</param>
        /// <param name="includeDatetime">Define se deve ser incluído carimbo de data e hor no nome do arquivo</param>
        /// <param name="description">Descrição inclusa no nome do arquivo</param>
        /// <param name="extension">Extensão utilizada no nome do arquivo.</param>
        public static void Rename(
            string path, string moveDirectory, bool includeDatetime, string description, string extension)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            string datetime = DateTime.Now.ToString("yyyy-MM-dd hhmmss");

            //Valida descrição
            string descricao = RemoveInvalidChars(description);

            //Remove ponto do final da descrição
            char c = descricao.LastOrDefault(f => f.Equals('.'));
            if (c != char.MinValue)
                descricao = descricao.Remove(descricao.Length - 1, 1);

            //Valida extensão
            string extensao = string.Empty;
            if (extension.Length == 0)
            {
                extension = Path.GetExtension(path);
            }
            else
            {
                if (!extension.Contains("."))
                    extensao = ".";

                extensao = extension;
            }

            //Constrói nome do arquivo
            string newName = filename;

            if (includeDatetime)
                newName = datetime;

            if (descricao.Length > 0)
            {
                if (includeDatetime)
                    newName += " - ";

                newName += descricao;
            }

            newName += extension;

            string newPath;
            if (string.IsNullOrWhiteSpace(moveDirectory))
            {
                newPath = Path.Combine(Path.GetDirectoryName(path), newName);
            }
            else
            {
                newPath = Path.Combine(moveDirectory, newName);
            }

            File.Move(path, newPath);
        }        

        public static void Delete(string path)
        {
            FileInfo fileinfo = new FileInfo(path);

            //Remove atributo somente leitura
            if (fileinfo.IsReadOnly)
                fileinfo.IsReadOnly = false;

            File.Delete(path);
            fileinfo = null;
        }

        public static void SetReadOnly(string path)
        {
            //Recupera atributos do arquivo
            FileAttributes atributos = File.GetAttributes(path);

            //Adiciona atributo somente leitura
            File.SetAttributes(
                path, atributos | FileAttributes.ReadOnly);
        }

        public static void SetNotReadOnly(string path)
        {
            //Recupera atributos do arquivo
            FileAttributes atributos = File.GetAttributes(path);

            if ((atributos & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                //Remove atributo somente leitura do arquivo
                atributos = atributos & ~FileAttributes.ReadOnly;
                File.SetAttributes(path, atributos);
            }
        }
    }
}