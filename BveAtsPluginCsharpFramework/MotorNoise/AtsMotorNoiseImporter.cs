﻿using System.IO;
using System.Collections.Generic;

using AtsPlugin.Parametrics;
using AtsPlugin.Importing;

namespace AtsPlugin.MotorNoise
{
    public static class AtsMotorNoiseImporter
    {
        public static AtsMotorNoise LoadAsset(string motorNoisePath, string soundPath, string soundTxtSectionName)
        {
            if (!File.Exists(motorNoisePath))
            {
                throw new FileNotFoundException(string.Format("{0}: The file does not exist: {0}",  typeof(AtsMotorNoiseImporter).Name, motorNoisePath));
            }

            var motorNoise = ImportMotorNoise(motorNoisePath);


            if (string.IsNullOrEmpty(soundPath) || File.Exists(soundPath))
            {
                ImportSound(motorNoise, soundPath, soundTxtSectionName);
            }


            return motorNoise;
        }

        public static AtsMotorNoise LoadAsset(string motorNoisePath)
        {
            return LoadAsset(motorNoisePath, string.Empty, string.Empty);
        }
        

        private static AtsMotorNoise ImportMotorNoise(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            var motorNoiseTxt = AtsParser.ParseIni(path, a => AtsStorage.ReadText(a));

            return new AtsMotorNoise(
                new AtsMotorNoise.ParameterTables(
                    ImportMotorNoiseTable(Path.Combine(directoryName, motorNoiseTxt["Power"].GetAsString("Frequency"))),
                    ImportMotorNoiseTable(Path.Combine(directoryName, motorNoiseTxt["Power"].GetAsString("Volume")))
                    ),
                new AtsMotorNoise.ParameterTables(
                    ImportMotorNoiseTable(Path.Combine(directoryName, motorNoiseTxt["Brake"].GetAsString("Frequency"))),
                    ImportMotorNoiseTable(Path.Combine(directoryName, motorNoiseTxt["Brake"].GetAsString("Volume")))
                    )
                );

        }

        private static AtsTable ImportMotorNoiseTable(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(AtsModule.ModuleDirectoryPath, path);
            }


            if (!File.Exists(path))
            {
                return null;
            }


            return AtsParser.ParseTable(path, a => AtsStorage.ReadText(a));
        }

        private static void ImportSound(AtsMotorNoise motorNoise, string path, string sectionName)
        {
            var directoryName = Path.GetDirectoryName(path);
            var soundTxt = AtsParser.ParseIni(path, a => AtsStorage.ReadText(a));

            var keyValuePairs = soundTxt[sectionName].GetPairArray();


            var motorTrackList = new List<AtsMotorNoise.MotorTrack>();


            foreach (var keyValuePair in keyValuePairs)
            {
                var index = int.Parse(keyValuePair.Key);
                var filePath = Path.Combine(directoryName, keyValuePair.Value);


                if (!File.Exists(filePath))
                {
                    AtsDebug.LogError(string.Format("{0}: The file does not exist: {1}, {2}", typeof(AtsMotorNoiseImporter).Name, index, filePath));
                    return;
                }

                var audio = new AtsMotorNoise.MotorAudio(new StreamReader(filePath).BaseStream);


                motorTrackList.Add(new AtsMotorNoise.MotorTrack(index, filePath, audio));
            }

            motorNoise.SetMotorTracks(motorTrackList.ToArray());
        }
    }
}
