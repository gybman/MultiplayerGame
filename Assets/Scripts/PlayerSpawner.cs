using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gun;

    public float respawnDelay = 3f;
        
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
        // Example: Respawn at a random position within a certain range
        Vector3 respawnPosition = new Vector3(Random.Range(-20f, 20f), 2f, Random.Range(-20f, 20f));
        player.transform.position = respawnPosition;
    }
}
