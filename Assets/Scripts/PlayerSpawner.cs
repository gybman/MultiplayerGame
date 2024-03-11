using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gun;
    public Transform[] spawnPoints;

    public float respawnDelay = 3f;

    private void Start()
    {
        // Find all game objects with the "SpawnPoint" tag
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");

        // Initialize the spawnPoints array with the same size as the spawnPointObjects array
        spawnPoints = new Transform[spawnPointObjects.Length];

        // Populate the spawnPoints array with the transform components of the spawn point game objects
        for (int i = 0; i < spawnPointObjects.Length; i++)
        {
            spawnPoints[i] = spawnPointObjects[i].transform;
        }
    }

    public IEnumerator RespawnAfterDelay()
    {
        Debug.Log("Entered respawn");
        player.SetActive(false);
        gun.SetActive(false);
        // Wait for the respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Respawn the player at a random position
        RespawnAtRandomPosition();
        player.SetActive(true);
        gun.SetActive(true);
    }

    private void RespawnAtRandomPosition()
    {
        // Generate a random index within the range of the spawnPoints array
        int randomIndex = Random.Range(0, spawnPoints.Length);

        // Get the position of the selected spawn point
        Vector3 respawnPosition = spawnPoints[randomIndex].position;
        player.transform.position = respawnPosition;
    }
}
