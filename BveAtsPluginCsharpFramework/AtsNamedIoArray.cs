using System;
using System.Collections.Generic;
using System.Linq;

namespace AtsPlugin
{
    public class AtsNamedIoArray : AtsIoArray
    {
        private Dictionary<string, int> ArrayTable { get; set; } = new Dictionary<string, int>();


        public int this[string key]
        {
            get
            {
                if (!ArrayTable.ContainsKey(key))
                {
                    return 0;
                }


                var index = ArrayTable[key];
                return base[index];
            }
            set
            {
                if (!ArrayTable.ContainsKey(key))
                {
                    return;
                }


                var index = ArrayTable[key];
                base[index] = value;
            }
        }


        public AtsNamedIoArray() : base()
        {
        }

        public AtsNamedIoArray(IntPtr source, int length = 256) : base(source, length)
        {
        }
        
        public void Add(string key)
        {
            if (!FindFreeElementIndex(out var desireIndex))
            {
                return;
            }


            ArrayTable.Add(key, desireIndex);
        }

        public void Add(string key, int desireIndex)
        {
            if (ArrayTable.ContainsValue(desireIndex))
            {
                return;
            }


            ArrayTable.Add(key, desireIndex);
        }

        private bool FindFreeElementIndex(out int emptyIndex)
        {
            var emptyIndices = Enumerable.Range(0, Length).Where(e => !ArrayTable.ContainsValue(e)).ToArray();


            emptyIndex = emptyIndices.Length > 0 ? emptyIndices[0] : -1;
            return (emptyIndex != -1);
        }
    }
}
