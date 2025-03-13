namespace Percolore.Core.Exceptions
{
	/// <summary>
	/// Represents an exception that is thrown when a Bluetooth operation fails.
	/// </summary>
	public class BluetoothException : Exception
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="BluetoothException"/> class.
		/// </summary>
		/// <param name="message">An exception description message.</param>
		public BluetoothException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BluetoothException"/> class.
		/// </summary>
		/// <param name="message">An exception description message.</param>
		/// <param name="innerException">An inner exception that caused this exception.</param>
		public BluetoothException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
