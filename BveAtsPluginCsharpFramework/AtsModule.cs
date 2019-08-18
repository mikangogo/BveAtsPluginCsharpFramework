using System.Reflection;
using System.IO;

namespace AtsPlugin
{
    public static class AtsModule
    {
        static AtsModule()
        {
            AtsDebug.LogInfo($"Assembly: {ModuleName}");
            AtsDebug.LogInfo($"Path: {ModuleDirectoryPath}");
        }

        public static string ModulePathWithFileName => Assembly.GetExecutingAssembly().Location;
        public static string ModuleName => Path.GetFileName(ModulePathWithFileName);
        public static string ModuleDirectoryPath => Path.GetDirectoryName(ModulePathWithFileName);
    }
}
