using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Percolore.Core
{
	/// <summary>
	/// Disponibiliza informações de um assembly especificado.
	/// </summary>
	public class AssemblyInfo
    {
        private static Assembly _assembly;

        public AssemblyInfo(Assembly assembly)
        {
            _assembly = assembly;
        }

        private T CustomAttributes<T>() where T : Attribute
        {
            object[] customAttributes = _assembly.GetCustomAttributes(typeof(T), false);

            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return ((T)customAttributes[0]);
            }

            throw new InvalidOperationException();
        }

        public string Title
        {
            get
            {
                return CustomAttributes<AssemblyTitleAttribute>().Title;
            }
        }

        public string Description
        {
            get
            {
                return CustomAttributes<AssemblyDescriptionAttribute>().Description;
            }
        }

        public string Company
        {
            get
            {
                return CustomAttributes<AssemblyCompanyAttribute>().Company;
            }
        }

        public string Product
        {
            get
            {
                return CustomAttributes<AssemblyProductAttribute>().Product;
            }
        }

        public string Copyright
        {
            get
            {
                return CustomAttributes<AssemblyCopyrightAttribute>().Copyright;
            }
        }

        public string Trademark
        {
            get
            {
                return CustomAttributes<AssemblyTrademarkAttribute>().Trademark;
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return _assembly.GetName().Version.ToString();
            }
        }

        public string AssemblyComercialVersion
        {
            get
            {
                Version version = _assembly.GetName().Version;
                return
                    $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }


        public string FileVersion
        {
            get
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(_assembly.Location);
                return fvi.FileVersion;
            }
        }

        public string Guid
        {
            get
            {
                return CustomAttributes<GuidAttribute>().Value;
            }
        }

        public string FileName
        {
            get
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(_assembly.Location);
                return fvi.OriginalFilename;
            }
        }

        public string FilePath
        {
            get
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(_assembly.Location);
                return fvi.FileName;
            }
        }
    }
}