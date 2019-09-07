using System;
using System.Collections.Generic;
using AtsPlugin.Parametrics;

namespace AtsPlugin
{
    public static class AtsNamedIoArrayExtensions
    {
        public static void SetNameAndIndexFromPluginParameters(this AtsNamedIoArray self, Type indexObjectType)
        {
            var type = indexObjectType;
            var fields = type.GetFields();

            foreach (var field in fields)
            {
                var indexString = string.Empty;
                var index = (int)field.GetRawConstantValue();
                var keyName = field.Name;


                try
                {
                    indexString = AtsSimulationEnvironment.Instance.PluginParameters.GetParameterAsString(keyName);
                }
                catch (KeyNotFoundException)
                {
                    AtsDebug.LogInfo($"{nameof(AtsNamedIoArray)}: Using default index: {type.Name}.{keyName}: {index}");
                    indexString = string.Empty;
                }


                if (!string.IsNullOrEmpty(indexString))
                {
                    index = int.Parse(indexString);
                }

                self.Add(keyName, index);
            }
        }
    }
}
