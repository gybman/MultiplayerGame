using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerStatsManager : NetworkBehaviour
{
    // Define network variables for player stats
    public NetworkVariable<int> killCount = new NetworkVariable<int>(0);

    public Renderer playerColor;
    public ulong PlayerId => NetworkObjectId; // Using NetworkObjectId as a unique identifier

    public static event Action OnKillCountChanged;  // Creates an event that can be subscribed to

    // Method to increment kill count
    public void IncrementKillCount()
    {
        if (IsServer)
        {
            // Increment kill count only on the server
            killCount.Value++;
            OnKillCountChanged?.Invoke();
        }
        else
        {
            // If called on a client, send an RPC to the server to increment kill count
            IncrementKillCountServerRpc();
        }
    }

    // RPC to increment kill count on the server
    [ServerRpc]
    private void IncrementKillCountServerRpc()
    {
        killCount.Value++;
        OnKillCountChanged?.Invoke();   // activates event
    }

    public void ResetKillCount()
    {
        if (IsServer)
        {
            killCount.Value = 0;    // resets the kill count when the game restarts
            OnKillCountChanged?.Invoke();   // activates event
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        FindObjectOfType<ScoreBoard>().ResetLocalPlayerData();  // resets stats for all players
    }

}
