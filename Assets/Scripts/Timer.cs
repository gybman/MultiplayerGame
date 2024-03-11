using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Timer : NetworkBehaviour
{
    public TMP_Text timerText;
    private NetworkVariable<int> remainingTime = new NetworkVariable<int>();

    private const int totalTime = 600; // 10 minutes in seconds

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

    private IEnumerator StartTimer()
    {
        while (remainingTime.Value > 0)
        {
            yield return new WaitForSeconds(1);
            remainingTime.Value--;
            UpdateTimerOnClientsClientRpc(remainingTime.Value); // Update clients with the new time
        }
    }

    [ClientRpc]
    private void UpdateTimerOnClientsClientRpc(int time)
    {
        UpdateTimerUI(time); // Update the UI on all clients
    }

    private void UpdateTimerUI(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
