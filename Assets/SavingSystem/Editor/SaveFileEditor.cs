using UnityEngine;
using UnityEditor;

namespace SavingSystem
{
    public class SaveFileEditor
    {
        private const string FileName = "SaveData";
        private static string EditorPath => $"Assets/Resources/{FileName}.json";

        [MenuItem("Saves/Overwrite Path To Resources")]
        public static void SetPathToResources()
        {
            SaveFile.SetPath(EditorPath);
        }

        [MenuItem("Saves/Create Save File Copy")]
        public static void CreateSaveFile()
        {
            SetPathToResources();
            var so = ScriptableObject.CreateInstance<SO_SaveFile>();
            var saveData = SaveFile.Load(true);
            so.SaveData = saveData;

            AssetDatabase.CreateAsset(so, "Assets/SaveFileCopy.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = so;
        }

        [MenuItem("Saves/Clear Save File")]
        public static void ClearSaveFile()
        {
            SetPathToResources();
            SaveFile.ClearSaveFile();
        }
    }
}
