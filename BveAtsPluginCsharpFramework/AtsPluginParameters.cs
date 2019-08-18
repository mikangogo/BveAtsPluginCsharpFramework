using System.IO;
using AtsPlugin.Importing;
using AtsPlugin.Parametrics;

namespace AtsPlugin
{
    public class AtsPluginParameters
    {
        public static AtsPluginParameters Instance { get; } = new AtsPluginParameters();

        public const string PluginParametersFileName = @"PluginParameters.txt";

        private AtsIni ParametersBody { get; set; }


        public AtsPluginParameters()
        {
            try
            {
                ParametersBody = AtsParser.ParseIni(PluginParametersFileName, AtsStorage.ReadText);
            }
            catch (FileNotFoundException e)
            {
                AtsDebug.LogWarning("Continue running: " + e.Message);
                ParametersBody = null;
            }
        }

        public string GetParameterAsString(string keyName, string sectionName = null)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                sectionName = AtsModule.ModuleName;
            }

            return ParametersBody[sectionName][keyName];
        }

        public bool GetParameterAsBoolean(string keyName, string sectionName = null)
        {
            return bool.Parse(GetParameterAsString(keyName, sectionName));
        }

        public int GetParameterAsInt32(string keyName, string sectionName = null)
        {
            return int.Parse(GetParameterAsString(keyName, sectionName));
        }

        public float GetParameterAsFp32(string keyName, string sectionName = null)
        {
            return float.Parse(GetParameterAsString(keyName, sectionName));
        }

        public double GetParameterAsFp64(string keyName, string sectionName = null)
        {
            return int.Parse(GetParameterAsString(keyName, sectionName));
        }
    }
}
