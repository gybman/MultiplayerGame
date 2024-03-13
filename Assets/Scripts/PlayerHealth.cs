using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    //public int health = 10; // Initial health value
    //public float respawnDelay = 3f; // Delay before respawning in seconds
    //private int defaultHealth = 3;

    //[SerializeField] private PlayerSpawner spawner;

    //private void OnTriggerEnter(Collider other)
    //{
    //    // Check if the collider belongs to a bullet and if it's not the player's own bullet
    //    if (other.CompareTag("Bullet") && other.GetComponent<MoveProjectiles>().playerOwner != this.gameObject)
    //        TakeDamage(1);
    //}

    //public void TakeDamage(int damage)
    //{
    //    health -= damage;
    //    Debug.Log("Damage dealt, health now: " + health);

    //    // Check if health has reached zero
    //    if (health <= 0)
    //    {
    //        health = defaultHealth;
    //        spawner.StartCoroutine(spawner.RespawnAfterDelay());
    //    }
    //}
    public NetworkVariable<int> health = new NetworkVariable<int>();

    public int defaultHealth = 3;
    public float respawnDelay = 3f;

    [SerializeField] private PlayerSpawner spawner;
    private GameObject parentObject;

    private void Start()
    {
        if (IsServer)
        {
            health.Value = defaultHealth;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.CompareTag("Bullet") && other.GetComponent<MoveProjectiles>().playerOwner != this.gameObject)
            {
                TakeDamage(1);
                parentObject = other.gameObject.GetComponent<MoveProjectiles>().parent.gameObject;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsServer)
        {
            health.Value -= damage;
            Debug.Log("Damage dealt, health now: " + health.Value);

            if (health.Value <= 0)
            {
                health.Value = defaultHealth;
                parentObject.GetComponent<PlayerStatsManager>().IncrementKillCount();
                RespawnClientRpc();
            }
        }
        else
        {
            TakeDamageServerRpc();
        }
    }

    [ServerRpc]
    public void TakeDamageServerRpc()
    {
        TakeDamage(1);
    }

    [ClientRpc]
    public void RespawnClientRpc()
    {
        spawner.StartCoroutine(spawner.RespawnAfterDelay());
    }
}
