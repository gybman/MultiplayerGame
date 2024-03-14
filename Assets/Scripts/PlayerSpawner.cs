using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject deathScreen;

    public Transform[] spawnPoints;

    public float respawnDelay = 3f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        deathScreen.SetActive(false);

        // Find all game objects with the "SpawnPoint" tag
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");

        // Initialize the spawnPoints array with the same size as the spawnPointObjects array
        spawnPoints = new Transform[spawnPointObjects.Length];

        // Populate the spawnPoints array with the transform components of the spawn point game objects
        for (int i = 0; i < spawnPointObjects.Length; i++)
        {
            spawnPoints[i] = spawnPointObjects[i].transform;
        }

        RespawnAtRandomPosition();
    }

    public IEnumerator RespawnAfterDelay()
    {
        Debug.Log("Entered respawn");
        player.GetComponent<NetworkTransform>().Interpolate = false;
        DisablePlayer();
        deathScreen.SetActive(true);
        // Wait for the respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Respawn the player at a random position
        RespawnAtRandomPosition();

        deathScreen.SetActive(false);
        EnablePlayer();
        yield return new WaitForSeconds(0.5f);
        player.GetComponent<NetworkTransform>().Interpolate = true;

    }

    private void RespawnAtRandomPosition()
    {
        // Generate a random index within the range of the spawnPoints array
        int randomIndex = Random.Range(0, spawnPoints.Length);

        // Get the position of the selected spawn point
        Vector3 respawnPosition = spawnPoints[randomIndex].position;
        player.transform.position = respawnPosition;
    }

    public void DisablePlayer()
    {
        player.SetActive(false);
        gun.GetComponent<ShootBall>().enabled = false;
    }

    public void EnablePlayer()
    {
        player.SetActive(true);
        gun.GetComponent<ShootBall>().enabled = true;
    }
}
