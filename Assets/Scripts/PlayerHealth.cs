using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 10; // Initial health value
    public float respawnDelay = 3f; // Delay before respawning in seconds
    private int defaultHealth = 3;

    [SerializeField] private PlayerSpawner spawner;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a bullet and if it's not the player's own bullet
        if (other.CompareTag("Bullet") && other.GetComponent<MoveProjectiles>().playerOwner != this.gameObject)
            TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Damage dealt, health now: " + health);

        // Check if health has reached zero
        if (health <= 0)
        {
            health = defaultHealth;
            spawner.StartCoroutine(spawner.RespawnAfterDelay());
        }
    }
}
