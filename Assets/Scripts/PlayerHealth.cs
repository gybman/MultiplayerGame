using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>();

    // health stats
    public int defaultHealth = 3;
    public float respawnDelay = 3f;

    // Audio
    public AudioSource audioSource;
    public AudioClip damageDealt;
    public AudioClip deathYell;

    // references
    [SerializeField] private PlayerSpawner spawner;
    private GameObject parentObject;

    private void Start()
    {
        if (IsServer)
        {
            health.Value = defaultHealth;   // set all players health the default health when first joining the game
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only the server can adjust the health in order to keep all health synced amongst players
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

            // if the host loses all its health, play the death sound for all players and respawn the dead player
            if (health.Value <= 0)
            {
                health.Value = defaultHealth;
                parentObject.GetComponent<PlayerStatsManager>().IncrementKillCount();
                // Call the method to play death sound on all clients with positional audio
                PlayDeathSoundServerRpc(transform.position);
                RespawnClientRpc();
            }
            else
            {
                // Call the method to play death sound on all clients with positional audio
                PlayDamageSoundServerRpc(transform.position);
            }
        }
        else
        {
            // if client takes damage, request the server to update and sync health
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

    [ClientRpc]
    public void PlayDamageSoundClientRpc(Vector3 position)
    {
        // Play the damage sound with positional audio
        if (damageDealt != null)
        {
            AudioSource.PlayClipAtPoint(damageDealt, position);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayDamageSoundServerRpc(Vector3 position)
    {
        // Calls client rpc so all clients hear the sound in the correct location
        PlayDamageSoundClientRpc(position);
    }

    [ClientRpc]
    public void PlayDeathSoundClientRpc(Vector3 position)
    {
        // Play the death sound with positional audio
        if (deathYell != null)
        {
            AudioSource.PlayClipAtPoint(deathYell, position);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayDeathSoundServerRpc(Vector3 position)
    {
        // Calls client rpc so all clients hear the sound in the correct location
        PlayDeathSoundClientRpc(position);
    }
}
