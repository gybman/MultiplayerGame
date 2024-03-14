using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ShootBall : NetworkBehaviour
{
    //Fireball prefab
    [SerializeField] private GameObject fireballPrefab;

    //Shoot Position
    [SerializeField] private Transform shootTransform;

    // Parent (Player)
    [SerializeField] private GameObject player;

    //List to hold all the instantiated fireballs
    private int spawnedBalls = 0;

    private const int maxBalls = 5;   // Max number of balls allowed


    private void Update()
    {
        if (!IsOwner) return;
        // Shoot ball when clicking mouse button0 using the position and the direction of shooting transform
        if (Input.GetKeyDown(KeyCode.Mouse0) && spawnedBalls < maxBalls)
        {
            ShootServerRpc();   // Will spawn the ball on all other clients screen
            Shoot();    // spawns the ball on local screen
            spawnedBalls++;
        }
    }

    private void Shoot()
    {
        GameObject fireball = Instantiate(fireballPrefab, shootTransform.position, shootTransform.rotation);
        fireball.GetComponent<MoveProjectiles>().parent = this;
        fireball.GetComponent<MoveProjectiles>().playerOwner = player;
        Renderer renderer = fireball.GetComponent<Renderer>();
        renderer.material.color = player.GetComponent<Renderer>().material.color;
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        ShootClientRpc();
    }

    [ClientRpc]
    private void ShootClientRpc()
    {
        if (!IsOwner) Shoot();  // will not respawn ball on local screen, only on other clients screens.
                                // doing it this way makes the game feel responsive on each clients screen, regardless if they are host or not
    }

    public void ballDestroyed()
    {
        spawnedBalls--;
    }
}
