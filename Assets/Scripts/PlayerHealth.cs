using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>();

    public int defaultHealth = 3;
    public float respawnDelay = 3f;

    public AudioSource audioSource;
    public AudioClip damageDealt;
    public AudioClip deathYell;

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

            if (health.Value <= 0)
            {
                health.Value = defaultHealth;
                parentObject.GetComponent<PlayerStatsManager>().IncrementKillCount();
                RespawnClientRpc();
            }
            else
            {
                // Check if the damage sound is already assigned to the audio source
                if (audioSource.clip != damageDealt)
                {
                    // Assign the damage sound clip to the audio source
                    audioSource.clip = damageDealt;
                }
                // Check if the audio source is not already playing
                if (!audioSource.isPlaying)
                {
                    // Play the sliding sound
                    audioSource.Play();
                }
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
        // Check if the death sound is already assigned to the audio source
        if (audioSource.clip != deathYell)
        {
            // Assign the damage sound clip to the audio source
            audioSource.clip = deathYell;
        }
        // Check if the audio source is not already playing
        if (!audioSource.isPlaying)
        {
            // Play the sliding sound
            audioSource.Play();
        }
    }
}
