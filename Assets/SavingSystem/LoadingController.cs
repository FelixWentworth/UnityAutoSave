using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SavingSystem
{
    public class LoadingController : MonoBehaviour
    {
        [Header("Loading")]
        [HideInInspector]
        public bool LoadingCompleted;
        public static Action OnLoadingComplete;

        [SerializeField]
        private List<LoadingComponent> _loadingSteps;

        private IEnumerator Start()
        {
            yield return LoadAll();
        }

        private IEnumerator LoadAll()
        {
            foreach (var step in _loadingSteps)
            {
                yield return step.Load();
                var completed = step.Loaded;

                Debug.Log("<color=cyan>" + step.gameObject.name + ": Loading " + (completed ? "completed" : "failed") + "</color>");
                if (!completed)
                {
                    throw new Exception("Loading failed");
                }

            }
            Debug.Log("All Loaded");
            LoadingCompleted = true;
            OnLoadingComplete?.Invoke();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            var isPaused = pauseStatus;
            if (isPaused)
            {
                SaveFile.AutoSave();
            }
        }

        private void OnApplicationQuit()
        {
            SaveFile.AutoSave();
        }
    }
}