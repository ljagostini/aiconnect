using System.Runtime.InteropServices;
using System.Security;

namespace Percolore.Core.Windows
{
	/// <summary>
	/// API methods in user32.dll.
	/// </summary>
	public static class User32
    {
        [SuppressUnmanagedCodeSecurity]
        class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern int GetSystemMetrics(SystemMetric metric);
        }

        /// <summary>
        /// Retrieves the specified system metric or system configuration setting.
        /// </summary>
        /// <param name="metric">The metric.</param>
        /// <returns></returns>
        public static int GetSystemMetrics(SystemMetric metric)
        {
            return NativeMethods.GetSystemMetrics(metric);
        }
    }    
}