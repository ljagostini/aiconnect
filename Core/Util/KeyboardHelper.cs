using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace Percolore.Core.Util
{
	/// <summary>
	/// Tipos de layout do teclado virtual.
	/// </summary>
	public enum KeyboardLayoutMode { Default, ThumbLayout, Handwriting }

    /// <summary>
    /// Rotinas de maniulação do teclado virtual.
    /// </summary>
    public static class KeyboardHelper
    {
        /* Caminho do executável do teclado virtual de acordo com 
         * a arquitetura do sistema opercional*/
        static string pathKeyboardFile =
            @"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe";

        /// <summary>
        /// The registry key which holds the keyboard settings.
        /// </summary>
        private static readonly RegistryKey registryKey =
            Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\TabletTip\\1.7");

        public static void SetKeyboardDockedMode(bool isDocked)
        {
            registryKey.SetValue(
                "EdgeTargetDockedState", Convert.ToInt32(isDocked), RegistryValueKind.DWord);
        }

        static bool IsDocked()
        {
            object obj =
                 registryKey.GetValue("EdgeTargetDockedState", 0, RegistryValueOptions.None);

            bool value = false;
            if (obj != null)
                value = Convert.ToBoolean(obj);

            return value;
        }

        /// <summary>
        /// Sets if the keyboard is in docked or floating mode.
        /// </summary>
        /// <param name="isDocked">If true set to docked, if false set to floating.</param>
        public static void SetKeyboardYPosition(int value)
        {
            registryKey.SetValue(
                "OptimizedKeyboardRelativeYPositionOnScreen", value, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Changes the layout mode of the keyboard.
        /// </summary>
        /// <param name="mode">The layout mode to use.</param>
        public static void SetKeyboardLayoutMode(KeyboardLayoutMode mode)
        {
            switch (mode)
            {
                case KeyboardLayoutMode.Handwriting:
                    registryKey.SetValue("KeyboardLayoutPreference", 0, RegistryValueKind.DWord);
                    registryKey.SetValue("LastUsedModalityWasHandwriting", 1, RegistryValueKind.DWord);
                    break;
                case KeyboardLayoutMode.ThumbLayout:
                    registryKey.SetValue("KeyboardLayoutPreference", 1, RegistryValueKind.DWord);
                    registryKey.SetValue("LastUsedModalityWasHandwriting", 0, RegistryValueKind.DWord);
                    // 0 = small, 1 = medium, 2 = large
                    registryKey.SetValue("ThumbKeyboardSizePreference", 2, RegistryValueKind.DWord);
                    break;
                default:
                    registryKey.SetValue("KeyboardLayoutPreference", 0, RegistryValueKind.DWord);
                    registryKey.SetValue("LastUsedModalityWasHandwriting", 0, RegistryValueKind.DWord);
                    break;
            }
        }

        public static void Show()
        {
            if (!File.Exists(pathKeyboardFile))
            {
                return;
            }

            using var p = new Process();
            p.StartInfo = new ProcessStartInfo
			{
				FileName = pathKeyboardFile,
				UseShellExecute = true,
				Verb = "runas"
			};
			p.Start();
        }

        public static bool IsOpen()
        {
            bool retorno = false;
            string processName = "TabTip";
            Process teclado =
                  Process.GetProcesses().FirstOrDefault(p => p.ProcessName == processName);
            if (teclado != null)
            {
                retorno = true;
            }
            return retorno;
        }

        public static void Show(Rectangle rectanguleToScreen)
        {   
            if (!File.Exists(pathKeyboardFile))
            {
                return;
            }

            int screenCenter = Screen.PrimaryScreen.Bounds.Height / 2;
            int controlCenter =
                rectanguleToScreen.Top + (rectanguleToScreen.Height / 2);

            KeyboardHelper.SetKeyboardLayoutMode(KeyboardLayoutMode.Default);
            if (controlCenter < screenCenter)
            {
                if (!IsDocked())
                {
                    Kill();
                }

                SetKeyboardDockedMode(true);
            }
            else
            {
                if (IsDocked())
                {
                    Kill();
                }

                SetKeyboardYPosition(500);
                SetKeyboardDockedMode(false);
            }

            using (Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo(pathKeyboardFile);
                p.Start();
            }
        }

        public static void Kill()
        {
            string processName = "TabTip";
            Process keyboard =
                  Process.GetProcesses().FirstOrDefault(p => p.ProcessName == processName);
            if (keyboard != null)
            {
                try
                {
                    keyboard.Kill();
                }
                catch
                { }            
            }
        }

        public static bool GetKeyboardPresent()
        {
            bool keyboardPresent = false;
            SelectQuery Sq = new SelectQuery("Win32_Keyboard");
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
            ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
            StringBuilder sb = new StringBuilder();
            foreach (ManagementObject mo in osDetailsCollection)
            {
                keyboardPresent = true;
            }

            return keyboardPresent;
        }
    }
}