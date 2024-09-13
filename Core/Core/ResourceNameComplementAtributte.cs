namespace Percolore.Core
{
	public class ResourceNameComplementAtributte : Attribute
    {

        #region Properties

        /// <summary>
        /// Nome de resource de complemento
        /// </summary>
        public string ResourceName { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor used to init a StringValue Attribute
        /// </summary>
        /// <param name="value"></param>
        public ResourceNameComplementAtributte(string value)
        {
            this.ResourceName = value;
        }

        #endregion
    }
}