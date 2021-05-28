using UnityEngine;

namespace SavingSystem
{
    public class SO_SaveFile : ScriptableObject
    {
        public bool OverrideSaveFile;
        public SaveValue SaveData;

        void OnEnable()
        {
            if (OverrideSaveFile)
            {
                SaveFile.OverrideSave(SaveData);
            }
        }
    }
}