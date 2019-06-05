using System.Collections.Generic;

namespace AtsPlugin.Parametrics
{
    public class AtsIni : AtsParametricBase
    {
        public class Section : AtsParametricBase
        {
            public string Name { get; private set; } = string.Empty;
            public Dictionary<string, string> PairDictionary { get; private set; } = null;


            public string this[string key]
            {
                get
                {
                    key = key.ToLower();


                    if (!PairDictionary.ContainsKey(key))
                    {
                        throw new KeyNotFoundException(message: string.Format("{0}: The key '{2}' does not exist in section '[{1}]'.", FileName, Name, key));
                    }

                    
                    return PairDictionary[key];
                }
            }

            public int GetAsInt(string key)
            {
                return int.Parse(this[key]);
            }

            public float GetAsFloat(string key)
            {
                return float.Parse(this[key]);
            }

            public string GetAsString(string key)
            {
                return this[key];
            }

            public KeyValuePair<string, string>[] GetPairArray()
            {
                return new List<KeyValuePair<string, string>>(PairDictionary).ToArray();
            }


            public Section(string name, Dictionary<string, string> pairs, string path) : base(path)
            {
                Name = name.ToLower();
                PairDictionary = pairs;
            }
        }


        public Dictionary<string, Section> SectionDictionary { get; private set; } = null;


        public Section this[string sectionName]
        {
            get
            {
                sectionName = sectionName.ToLower();

                if (!SectionDictionary.ContainsKey(sectionName))
                {
                    throw new KeyNotFoundException($"{FileName}: The section '[{sectionName}]' does not exist.");
                }

                return SectionDictionary[sectionName];
            }
        }


        public AtsIni(Dictionary<string, Section> sections, string path) : base(path)
        {
            SectionDictionary = sections;
        }
    }
}
