using System.IO;
using System.Linq;
using AtsPlugin.Importing;
using AtsPlugin.Parametrics;

namespace AtsPlugin
{
    public class AtsPluginParameters
    {
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

        public string[] GetParameterAsStringArray(string keyName, string sectionName = null)
        {
            var value = GetParameterAsString(keyName, sectionName);
            var values = value.Replace(" ", "").Split(',');
            return values;
        }

        public bool GetParameterAsBoolean(string keyName, string sectionName = null)
        {
            return bool.Parse(GetParameterAsString(keyName, sectionName));
        }

        public bool[] GetParameterAsBooleanArray(string keyName, string sectionName = null)
        {
            return GetParameterAsStringArray(keyName, sectionName).Select(bool.Parse).ToArray();
        }

        public int GetParameterAsInt32(string keyName, string sectionName = null)
        {
            return int.Parse(GetParameterAsString(keyName, sectionName));
        }

        public int[] GetParameterAsInt32Array(string keyName, string sectionName = null)
        {
            return GetParameterAsStringArray(keyName, sectionName).Select(int.Parse).ToArray();
        }

        public float GetParameterAsFp32(string keyName, string sectionName = null)
        {
            return float.Parse(GetParameterAsString(keyName, sectionName));
        }

        public float[] GetParameterAsFp32Array(string keyName, string sectionName = null)
        {
            return GetParameterAsStringArray(keyName, sectionName).Select(float.Parse).ToArray();
        }

        public double GetParameterAsFp64(string keyName, string sectionName = null)
        {
            return double.Parse(GetParameterAsString(keyName, sectionName));
        }

        public double[] GetParameterAsFp64Array(string keyName, string sectionName = null)
        {
            return GetParameterAsStringArray(keyName, sectionName).Select(double.Parse).ToArray();
        }
    }
}
