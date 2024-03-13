using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections;

public class ScoreBoard : NetworkBehaviour
{
    public TMP_Text scoreboardText;
    public bool test;

    public override void OnNetworkSpawn()
    {
        UpdateScoreboardServerRpc();
    }

    private void OnEnable()
    {
        Debug.Log("Updating scoreboard text: ");
        // Subscribe to the event
        PlayerStatsManager.OnKillCountChanged += UpdateScoreboard;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        PlayerStatsManager.OnKillCountChanged -= UpdateScoreboard;
    }

    private void Update()
    {
        if (test)
        {
            test = false;
            UpdateScoreboard();
        }
    }

    private void UpdateScoreboard()
    {
        // This method needs to collect all player stats and update the scoreboardText
        if (IsServer)
        {
            UpdateScoreBoardClientRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateScoreboardServerRpc()
    {
        UpdateScoreBoardClientRpc();
    }

    [ClientRpc]
    private void UpdateScoreBoardClientRpc()
    {
        StartCoroutine(UpdateScoreboardCoroutine());
    }

    private IEnumerator UpdateScoreboardCoroutine()
    {
        // Assuming you have a more efficient way to access player stats
        yield return new WaitForSeconds(1f); // Wait to ensure all data is up-to-date

        // Assuming you have a way to get all player stats managers
        var playerStatsManagers = FindObjectsOfType<PlayerStatsManager>();
        string scoreboardTextValue = "Player Kills:\n";
        int player = 1;
        foreach (var playerStats in playerStatsManagers)
        {
            // Append each player's kill count to the string
            scoreboardTextValue += $"Player {player}: {playerStats.killCount.Value} kills\n";
            player++;
        }
        Debug.Log("Updating scoreboard text: " + scoreboardTextValue);

        scoreboardText.text = scoreboardTextValue;
    }
}
