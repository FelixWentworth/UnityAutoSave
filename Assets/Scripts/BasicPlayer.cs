using System.Collections;
using System.Collections.Generic;
using SavingSystem;

public class BasicPlayer : AutoSave
{
    [SaveData] public float Score { get; set; }
    [SaveData] public PlayerData Metadata { get; set; }

    public class PlayerData
    {
        public string Name;
        public string Country;
    }

    public void SetData(PlayerData data)
    {
        Metadata = data;
        gameObject.name = Metadata.Name;
    }

    public override void Awake()
    {
        base.Awake();
        PlayerSpawner.Players.Add(this);
    }

    public override IEnumerator ApplySaveData(Dictionary<string, string> data)
    {
        SaveFile.ApplySaveData(data, this);
        yield return ApplySaveState();
        yield return base.ApplySaveData(data);
    }

    private IEnumerator ApplySaveState()
    {
        // set players state based on their data
        gameObject.name = Metadata.Name;
        yield return null;
    }
}