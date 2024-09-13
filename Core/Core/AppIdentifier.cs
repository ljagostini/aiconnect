namespace Percolore.Core
{
    /// <summary>
    /// Identificador de aplicação
    /// </summary>
    public class AppIdentifier
    {
        string _name;
        string _guid;

        /// <summary>
        /// Nome da aplicação
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Guid de identificação da aplicação. É o mesmo Guid de identificação
        /// existente no AssemblyInfo.cs
        /// </summary>
        public string Guid
        {
            get { return _guid; }
        }

        public AppIdentifier(string nome, string guid)
        {
            _name = nome;
            _guid = guid;
        }
    }
}