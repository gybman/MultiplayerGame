using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections;
using System.Linq;

public class ScoreBoard : NetworkBehaviour
{
    public TMP_Text scoreboardText;
    private Dictionary<ulong, PlayerStatsManager> playerStatsManagers = new Dictionary<ulong, PlayerStatsManager>();
    private Dictionary<ulong, int> playerNumbers = new Dictionary<ulong, int>();
    private int nextPlayerNumber = 1;
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

    // updates the player list when a player joins or leaves the game
    private void UpdatePlayerList()
    {
        foreach (var player in FindObjectsOfType<PlayerStatsManager>())
        {
            if (!playerStatsManagers.ContainsKey(player.PlayerId))
            {
                playerStatsManagers[player.PlayerId] = player;
                playerNumbers[player.PlayerId] = nextPlayerNumber++;
                UpdateScoreboard();
            }
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        PlayerStatsManager.OnKillCountChanged -= UpdateScoreboard;
        // Clear local player data when leaving the game
        if (!IsServer && !IsClient)
        {
            ResetLocalPlayerData();
        }
    }

    private void UpdateScoreboard()
    {
        // This method needs to collect all player stats and update the scoreboardText
        if (IsServer)
        {
            UpdatePlayerList(); // Refresh the player list to account for any joins/leaves
            UpdateScoreBoardClientRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateScoreboardServerRpc()
    {
        UpdateScoreBoardClientRpc();    // updates the scoreboard for all clients
    }

    [ClientRpc]
    private void UpdateScoreBoardClientRpc()
    {
        StartCoroutine(UpdateScoreboardCoroutine());
    }

    private IEnumerator UpdateScoreboardCoroutine()
    {
        UpdatePlayerList();
        // Assuming you have a more efficient way to access player stats
        yield return new WaitForSeconds(1f); // Wait to ensure all data is up-to-date

        var sortedPlayers = playerNumbers.OrderBy(p => p.Value).ToList();
        string scoreboardTextValue = "Player Kills:\n";
        
        // For loop goes through all players and correctly displays their scoreboard with color so each player knows who is who
        foreach (var playerStats in sortedPlayers)
        {
            PlayerStatsManager stat = playerStatsManagers[playerStats.Key];
            Color color = stat.playerColor.material.color;
            string colorHex = ColorUtility.ToHtmlStringRGB(color);
            scoreboardTextValue += $"<color=#{colorHex}>Player {playerStats.Value}: {stat.killCount.Value} kills</color>\n";
        }
        scoreboardText.text = scoreboardTextValue;
    }

    // resets all player stats and scoreboard
    public void ResetLocalPlayerData()
    {
        playerStatsManagers.Clear();
        playerNumbers.Clear();
        nextPlayerNumber = 1;
    }

}