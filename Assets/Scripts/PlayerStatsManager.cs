using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerStatsManager : NetworkBehaviour
{
    // Define network variables for player stats
    public NetworkVariable<int> killCount = new NetworkVariable<int>(0);

    public static event Action OnKillCountChanged;

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
        OnKillCountChanged?.Invoke();
    }
}
