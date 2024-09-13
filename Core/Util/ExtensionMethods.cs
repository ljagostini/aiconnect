using System.Windows.Forms;

namespace Percolore.Core.Util
{
	public static class ExtensionMethods
    {
        public static string TextNoFormatting(this MaskedTextBox _mask)
        {
            _mask.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            String retString = _mask.Text;
            _mask.TextMaskFormat = MaskFormat.IncludePromptAndLiterals;
            return retString;
        }
    }
}