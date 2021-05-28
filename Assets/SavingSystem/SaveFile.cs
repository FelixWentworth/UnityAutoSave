using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SavingSystem
{
    public static class SaveFile
    {
        public static Action OnInitComplete;

        public static bool Loaded;
        public static List<Dictionary<string, string>> AutoSaveValues => _saveData.AutoSaveValues;

        private static SaveValue _saveData;
        private static string _path;
        private const string FileName = "SaveData";
        private static string _devicePath => System.IO.Path.Combine(Application.persistentDataPath, $"{FileName}.json");
        private static string _resourcesPath => $"Assets/Resources/{FileName}.json";
        private static string Path => string.IsNullOrEmpty(_path) ? _devicePath : _path;

        private static List<Behaviour> _savedBehaviours = new List<Behaviour>();
        private static bool _clearedSaveFile;

        public static void AddBehaviourToSaveList(Behaviour b)
        {
            _savedBehaviours.Add(b);
        }

        public static void RemoveBehaviourFromSaveList(Behaviour b)
        {
            if (_savedBehaviours.Contains(b))
            {
                _savedBehaviours.Remove(b);
            }
        }

        public static void SetPath(string path)
        {
            _path = path;
        }

        public static void Init()
        {
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.WindowsEditor)
            {
                SetPath(_resourcesPath);
            }
            Load();
            OnInitComplete?.Invoke();
        }

        public static void OverrideSave(SaveValue saveData)
        {
            _saveData = saveData;
        }

        public static void ClearSaveFile()
        {
            _saveData = new SaveValue();
            Save();
            _clearedSaveFile = true;
        }

        public static void Save()
        {
            // check if we cleared save file. if we did - dont auto save anything // debug only
            if (_clearedSaveFile)
            {
                return;
            }
            string json = JsonConvert.SerializeObject(_saveData);
            using (FileStream fs = new FileStream(Path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(json);
                }
            }
        }

        public static void AutoSave()
        {
            // Get all classes that have auto save defined
            var data = _savedBehaviours.Select(b => GetSaveData(b)).ToList();
            Debug.Log("Data : " + JsonConvert.SerializeObject(data));

            // append extra save data to save file
            _saveData.AutoSaveValues = data;

            // Set this here to make sure we have the most up to date time 
            SetLong("OfflineStartTime", DateTime.UtcNow.Ticks);
            // Save
            Save();
        }

        public static Dictionary<string, string> GetSaveData(Behaviour b)
        {
            var t = b.GetType();
            var savedata = new Dictionary<string, string>();
            savedata.Add("type", t.ToString());
            foreach (var prop in t.GetProperties())
            {
                var attrs = (SaveDataAttribute[])prop.GetCustomAttributes
                    (typeof(SaveDataAttribute), false);
                foreach (var attr in attrs)
                {
                    //var saveData = (SaveDataAttribute)attr;
                    var data = JsonConvert.SerializeObject(prop.GetValue(b));
                    savedata.Add(prop.Name, data);
                }
            }
            return savedata;
        }

        public static void ApplySaveData(Dictionary<string, string> data, Behaviour b)
        {
            var t = b.GetType();
            foreach (var prop in t.GetProperties())
            {
                // Find all attributes with the SaveDataAttribute
                var attrs = (SaveDataAttribute[])prop.GetCustomAttributes
                    (typeof(SaveDataAttribute), false);
                foreach (var attr in attrs)
                {
                    if (data.ContainsKey(prop.Name))
                    {
                        var d = data[prop.Name];
                        var ty = prop.PropertyType;
                        var parsedData = JsonConvert.DeserializeObject(d, ty);
                        prop.SetValue(b, parsedData);
                    }
                }
            }
        }

        // SETTERS
        public static void SetInt(string key, int value)
        {
            Set(key, value.ToString());
        }

        public static void SetLong(string key, long value)
        {
            Set(key, value.ToString());
        }

        public static void SetFloat(string key, float value)
        {
            Set(key, value.ToString());
        }

        public static void SetString(string key, string value)
        {
            Set(key, value);
        }

        public static void SetBool(string key, bool value)
        {
            Set(key, value ? "true" : "false");
        }

        // GETTERS
        public static int GetInt(string key, int defaultValue = 0)
        {
            var data = Get(key);
            if (data == null)
            {
                Set(key, defaultValue.ToString());
                return defaultValue;
            }
            else
            {
                return int.Parse(data.Value);
            }
        }

        public static long GetLong(string key, long defaultValue = 0)
        {
            var data = Get(key);
            if (data == null)
            {
                Set(key, defaultValue.ToString());
                return defaultValue;
            }
            else
            {
                return long.Parse(data.Value);
            }
        }

        public static float GetFloat(string key, float defaultValue = 0f)
        {
            var data = Get(key);
            if (data == null)
            {
                Set(key, defaultValue.ToString());
                return defaultValue;
            }
            else
            {
                return float.Parse(data.Value);
            }
        }

        public static string GetString(string key, string defaultValue = "")
        {
            var data = Get(key);
            if (data == null)
            {
                Set(key, defaultValue);
                return defaultValue;
            }
            else
            {
                return data.Value;
            }
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            var data = Get(key);
            if (data == null)
            {
                Set(key, defaultValue ? "true" : "false");
                return defaultValue;
            }
            else
            {
                return data.Value == "true" ? true : false;
            }
        }

        // PRIVATE 
        private static SaveDataValue Get(string key)
        {
            return _saveData.Values.FirstOrDefault(s => s.Key == key);
        }

        private static void Set(string key, string value)
        {
            var saveValue = Get(key);
            if (saveValue == null)
            {
                _saveData.Values.Add(new SaveDataValue { Key = key, Value = value });
            }
            else
            {
                saveValue.Value = value;
            }
        }

        public static SaveValue Load(bool loadFromResources = false)
        {
            if (loadFromResources)
            {
                var textAsset = Resources.Load<TextAsset>(FileName);
                if (textAsset != null)
                {
                    var data = textAsset.text;
                    _saveData = JsonConvert.DeserializeObject<SaveValue>(data);
                }
                else
                {
                    _saveData = new SaveValue();
                }
                Loaded = true;
            }
            else
            {
                try
                {
                    using (FileStream fs = new FileStream(Path, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(fs))
                        {
                            var data = reader.ReadToEnd();
                            _saveData = JsonConvert.DeserializeObject<SaveValue>(data);
                        }
                    }
                }
                catch
                {
                    Debug.LogError("Unable to load save data from file: setting to default");
                    _saveData = new SaveValue();
                }
            }
            Loaded = true;
            return _saveData;
        }
    }
}