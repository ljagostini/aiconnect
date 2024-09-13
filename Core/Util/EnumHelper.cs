using System.Resources;

/// <summary>
/// Rotinas de apoio referete a enums
/// </summary>
namespace Percolore.Core.Util
{
	public static class EnumHelper
    {
        /// <summary>
        /// Retorna lista com valores do enum
        /// </summary>
        public static IList<T> ToList<T>()
        {
            if (!typeof(T).IsEnum)
                throw new Exception("T não é do tipo enumerador.");

            IList<T> list = new List<T>();
            Type type = typeof(T);
            if (type != null)
            {
                Array enumValues = Enum.GetValues(type);
                foreach (T value in enumValues)
                {
                    list.Add(value);
                }
            }

            return list;
        }

        /// <summary>
        /// Retorna lista com valores do enum exceto valores contidos na lista passada como parâmetro
        /// </summary>
        /// <param name="disabled">ista de objetos que não serão adicionados à lista final retornada</param>
        public static IList<T> ToList<T>(IList<T> disabled)
        {
            if (!typeof(T).IsEnum)
                throw new Exception("T não é do tipo enumerador.");

            IList<T> list = new List<T>();
            Type type = typeof(T);
            if (type != null)
            {
                Array enumValues = Enum.GetValues(type);
                foreach (T value in enumValues)
                {
                    if (!disabled.Contains(value))
                        list.Add(value);
                }
            }

            return list;
        }

        /// <summary>
        /// Retorna lista com descrição referente ao resource inidicado em cada item do enum.
        /// </summary>
        /// <param name="disabled">ista de objetos que não serão adicionados à lista final retornada</param>
        public static IList<ComboBoxItem> ToComboItemList<T>(ResourceManager resource, IList<T> disabled = null)
        {
            if (!typeof(T).IsEnum)
                throw new Exception("T não é do tipo enumerador.");

            if (resource == null)
                throw new Exception("O resource não foi informado.");

            List<ComboBoxItem> lista = new List<ComboBoxItem>();
            Type type = typeof(T);
            if (type != null)
            {
                Array enumValues = type.GetEnumValues();
                foreach (T enumValue in enumValues)
                {
                    if (disabled != null)
                    {
                        if (disabled.Contains(enumValue))
                            continue;
                    }

                    /* No description dos enumeradores está armazenado o nome do resource correspondente.
                     * Alteração realiza em 30/01/2016 (Refatorar) */
                    var field = type.GetField(enumValue.ToString());

                    string displayString = string.Empty;

                    //ResourceNameAtributte
                    Array fds = field.GetCustomAttributes(typeof(ResourceNameAtributte), true);
                    if (fds.GetLength(0) > 0)
                    {
                        ResourceNameAtributte r = (ResourceNameAtributte)fds.GetValue(0);
                        displayString = resource.GetString(r.ResourceName);
                    }

                    //ResourceNameAtributteComplement
                    fds = field.GetCustomAttributes(typeof(ResourceNameComplementAtributte), true);
                    if (fds.GetLength(0) > 0)
                    {
                        ResourceNameComplementAtributte r = (ResourceNameComplementAtributte)fds.GetValue(0);
                        displayString += $" ({resource.GetString(r.ResourceName)})";
                    }

                    if (string.IsNullOrWhiteSpace(displayString))
                    {
                        displayString = enumValue.ToString();
                    };

                    lista.Add(
                        new ComboBoxItem(Convert.ToInt32(enumValue), displayString));
                }
            }

            return lista;
        }
    }
}