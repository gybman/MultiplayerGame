using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Timer : NetworkBehaviour
{
    public TMP_Text timerText;
    private NetworkVariable<int> remainingTime = new NetworkVariable<int>();
    public GameObject endScreen;
    public GameObject restartButton;
    public GameObject waitingOnHost;

    private SimpleRelayMenu relayScript;

    private const int totalTime = 60; // 10 minutes in seconds

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            remainingTime.Value = totalTime; // Initialize remainingTime on the server
            StartCoroutine(StartTimer()); // Start the timer coroutine on the server
        }

        if (IsClient)
        {
            UpdateTimerUI(remainingTime.Value); // Initial UI update for clients
        }
    }

    private void Start()
    {
        endScreen.SetActive(false);
        relayScript = GameObject.Find("Relay").GetComponent<SimpleRelayMenu>();
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(3); // Wait three seconds before starting timer

        while (remainingTime.Value > 0)
        {
            yield return new WaitForSeconds(1);
            remainingTime.Value--;
            UpdateTimerOnClientsClientRpc(remainingTime.Value); // Update clients with the new time
        }

        if (IsServer)
        {
            // Disable players
            var players = FindObjectsOfType<PlayerSpawner>();
            foreach (var player in players)
            {
                player.DisablePlayer();
            }
            GameEndClientRpc(); // Notify all clients that the game has ended
        }
    }

    [ClientRpc]
    private void UpdateTimerOnClientsClientRpc(int time)
    {
        UpdateTimerUI(time); // Update the UI on all clients
    }

    [ClientRpc]
    private void GameEndClientRpc()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        endScreen.SetActive(true); // Show the end screen
        if (IsServer)
        {
            restartButton.SetActive(true); // Show restart button only to the host
        }
        else
        {
            waitingOnHost.SetActive(true); // Tell clients they are waiting on the host
        }
        
    }

    private void UpdateTimerUI(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    public void ExitGame()
    {
        relayScript.ActivateMenu();
        endScreen.SetActive(false);
        // If using Unity Netcode, disconnect the client or stop the host
        if (NetworkManager.Singleton.IsHost)
        {
            RemovePlayersClientRpc();
            NetworkManager.Singleton.Shutdown();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    [ClientRpc]
    public void RemovePlayersClientRpc()
    {
        relayScript.ActivateMenu();
        endScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        NetworkManager.Singleton.Shutdown();
    }

    public void RestartGame()
    {
        remainingTime.Value = totalTime;

        // Enable players
        var players = FindObjectsOfType<PlayerSpawner>();
        foreach (var player in players)
        {
            player.EnablePlayer();
        }

        // Reset kill counts for all players
        var stats = FindObjectsOfType<PlayerStatsManager>();
        foreach (var stat in stats)
        {
            stat.ResetKillCount();
        }

        // Respawn all players
        var healths = FindObjectsOfType<PlayerHealth>();
        foreach (var health in healths)
        {
            health.RespawnClientRpc();
        }

        RestartGameClientRpc();
        StartCoroutine(StartTimer()); // Start the timer coroutine on the server
    }

    [ClientRpc]
    public void RestartGameClientRpc()
    {
        endScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
