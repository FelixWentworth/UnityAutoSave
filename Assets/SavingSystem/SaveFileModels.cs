using System;
using System.Collections.Generic;

namespace SavingSystem
{
    [Serializable]
    public class SaveValue
    {
        public List<SaveDataValue> Values;
        public List<Dictionary<string, string>> AutoSaveValues;
        public SaveValue()
        {
            Values = new List<SaveDataValue>();
            AutoSaveValues = new List<Dictionary<string, string>>();
        }
    }

    [Serializable]
    public class SaveDataValue
    {
        public string Key;
        public string Value;
    }
}
