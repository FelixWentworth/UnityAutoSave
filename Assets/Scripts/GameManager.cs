using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        PlayerSpawner.OnPlayersSet += LogCurrentState;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // assign random score
            var players = PlayerSpawner.Players;
            var rand = Random.Range(0, players.Count);
            players[rand].Score++;

            LogCurrentState();
            SavingSystem.SaveFile.SetBool("Added Score", true);
        }
    }

    private void LogCurrentState()
    {
        var players = PlayerSpawner.Players;

        var str = "";
        foreach(var p in players)
        {
            str += $"{p.Metadata.Name} - {p.Score}\t";
        }
        Debug.Log(string.Format("<color=cyan>{0}</color>", str));
    }
}
