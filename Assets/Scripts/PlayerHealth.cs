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
                // Call the method to play death sound on all clients with positional audio
                PlayDeathSoundClientRpc(transform.position);
                RespawnClientRpc();
            }
            else
            {
                // Call the method to play death sound on all clients with positional audio
                PlayDamageSoundClientRpc(transform.position);
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

    [ClientRpc]
    public void PlayDamageSoundClientRpc(Vector3 position)
    {
        // Play the damage sound with positional audio
        if (damageDealt != null)
        {
            AudioSource.PlayClipAtPoint(damageDealt, position);
        }
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
}
