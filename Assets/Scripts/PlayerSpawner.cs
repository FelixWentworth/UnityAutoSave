using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SavingSystem;

public class PlayerSpawner : MonoBehaviour
{
    public static System.Action OnPlayersSet;
    public static List<BasicPlayer> Players = new List<BasicPlayer>();

    [System.Serializable]
    public class LoadMapping
    {
        public GameObject Prefab;
        public AutoSave Behaviour;
    }
    [SerializeField] private LoadMapping[] _objectsForLoading;

    public GameObject PlayerPrefab;
    public int PlayersToSpawn;

    private void Awake()
    {
        LoadingController.OnLoadingComplete += OnLoadingComplete;
    }

    private void OnLoadingComplete()
    {
        var data = SaveFile.AutoSaveValues;
        if (data.Count > 0)
        {
            StartCoroutine(RecreatePlayers(data));
        }
        else
        {
            for(var i=0; i<PlayersToSpawn; i++)
            {
                var p = Instantiate(PlayerPrefab).GetComponent<BasicPlayer>();
                p.SetData(new BasicPlayer.PlayerData { Country = "GB", Name = "Player " + (i + 1) });
            }
            OnPlayersSet?.Invoke();
        }
    }

    private IEnumerator RecreatePlayers(List<Dictionary<string, string>> saveData)
    {
        foreach (var data in saveData)
        {
            var type = data["type"];
            var obj = _objectsForLoading.FirstOrDefault(o => o.Behaviour.GetType().ToString() == type);
            if (obj != null)
            {
                var go = Instantiate(obj.Prefab);
                var autoSave = go.GetComponent<AutoSave>();
                if (autoSave != null)
                {
                    yield return autoSave.ApplySaveData(data);
                }
            }
            else
            {
                Debug.LogError("No behaviour found of type " + type);
            }
        }
        OnPlayersSet?.Invoke();
    }

}

