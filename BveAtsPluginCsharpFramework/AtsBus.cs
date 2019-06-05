using System.Collections.Generic;

namespace AtsPlugin
{
    public class AtsBus
    {
        public struct ValueData
        {
            public bool BoolValue;
            public int Int32Value;
            public double Fp64Value;
            public string StringValue;
        }

        public class ValueSet
        {
            public ValueData Value { get; set; } = new ValueData();
            public ValueData DefaultValue { get; set; } = new ValueData();
            public bool AsBoolean
            {
                get => Value.BoolValue;

                set
                {
                    var changedValue = Value;
                    changedValue.BoolValue = value;
                    Value = changedValue;
                }
            }
            public int AsInt32
            {
                get => Value.Int32Value;

                set
                {
                    var changedValue = Value;
                    changedValue.Int32Value = value;
                    Value = changedValue;
                }
            }
            public double AsFp64
            {
                get => Value.Fp64Value;

                set
                {
                    var changedValue = Value;
                    changedValue.Fp64Value = value;
                    Value = changedValue;
                }
            }
            public float AsFp32
            {
                get => (float)Value.Fp64Value;

                set
                {
                    var changedValue = Value;
                    changedValue.Fp64Value = (double)value;
                    Value = changedValue;
                }
            }
            public string AsString
            {
                get => Value.StringValue;

                set
                {
                    var changedValue = Value;
                    changedValue.StringValue = value;
                    Value = changedValue;
                }
            }

            public ValueSet(ValueData value, ValueData defaultValue)
            {
                Value = value;
                DefaultValue = defaultValue;
            }

            public void ResetToDefaultValue()
            {
                Value = DefaultValue;
            }
        }


        private Dictionary<string, ValueSet> ValueTable { get; set; } = new Dictionary<string, ValueSet>();

        public ValueSet this[string key]
        {
            get
            {
                if (!ValueTable.ContainsKey(key))
                {
                    throw new KeyNotFoundException(message:
                        $"{typeof(AtsBus).Name}: The key does not exist: {typeof(AtsBus).Name}");
                }

                
                return ValueTable[key];
            }
        }


        public AtsBus()
        {
        }

        public void Reset()
        {
            foreach (var pair in ValueTable)
            {
                pair.Value.ResetToDefaultValue();
            }
        }

        public bool AddBoolean(string key, bool defaultValue)
        {
            if (ValueTable.ContainsKey(key))
            {
                return false;
            }


            var valueSet = new ValueSet(
                new ValueData() { BoolValue = defaultValue },
                new ValueData() { BoolValue = defaultValue }
                );


            ValueTable.Add(key, valueSet);

            return true;
        }

        public bool AddInt32(string key, int defaultValue)
        {
            if (ValueTable.ContainsKey(key))
            {
                return false;
            }


            var valueSet = new ValueSet(
                new ValueData() { Int32Value = defaultValue },
                new ValueData() { Int32Value = defaultValue }
                );


            ValueTable.Add(key, valueSet);

            return true;
        }

        public bool AddFp64(string key, double defaultValue)
        {
            if (ValueTable.ContainsKey(key))
            {
                return false;
            }


            var valueSet = new ValueSet(
                new ValueData() { Fp64Value = defaultValue },
                new ValueData() { Fp64Value = defaultValue }
                );


            ValueTable.Add(key, valueSet);

            return true;
        }

        public bool AddString(string key, string defaultValue)
        {
            if (ValueTable.ContainsKey(key))
            {
                return false;
            }


            var valueSet = new ValueSet(
                new ValueData() { StringValue = defaultValue },
                new ValueData() { StringValue = defaultValue }
                );


            ValueTable.Add(key, valueSet);

            return true;
        }
    }
}
