using System.Reflection;
using System.IO;

namespace AtsPlugin
{
    public static class AtsModule
    {
        public static string ModulePathWithFileName
        {
            get
            {
                return Assembly.GetExecutingAssembly().Location;
            }
        }

        public static string ModuleName
        {
            get
            {
                return Path.GetFileName(ModulePathWithFileName);
            }
        }

        public static string ModuleDirectoryPath
        {
            get
            {
                return Path.GetDirectoryName(ModulePathWithFileName);
            }
        }
    }
}
