namespace Percolore.Core
{
	public class ResourceNameAtributte : Attribute
    {

        #region Properties

        /// <summary>
        /// Holds the stringvalue for a value in an enum.
        /// </summary>
        public string ResourceName { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor used to init a StringValue Attribute
        /// </summary>
        /// <param name="value"></param>
        public ResourceNameAtributte(string value)
        {
            this.ResourceName = value;
        }

        #endregion
    }
}