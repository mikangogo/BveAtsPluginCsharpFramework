using System.Reflection;
using System.IO;

namespace AtsPlugin
{
    public static class AtsModule
    {
        public static string ModulePathWithFileName => Assembly.GetExecutingAssembly().Location;
        public static string ModuleName => Path.GetFileName(ModulePathWithFileName);
        public static string ModuleDirectoryPath => Path.GetDirectoryName(ModulePathWithFileName);
    }
}
