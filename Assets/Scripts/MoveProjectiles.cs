using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MoveProjectiles : NetworkBehaviour
{
    public ShootBall parent;
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private float shootForce;
    private Rigidbody rb;

    private void Start()
    {
        // reference for the rigidbody
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Move the fireball forward based on the player facing direction
        rb.velocity = rb.transform.forward * shootForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        InstantiateHitParticlesServerRpc();
        // Pass the NetworkObject ID of the ball to the server RPC
        NetworkObject ballNetworkObject = gameObject.GetComponent<NetworkObject>();
        parent.DestroyServerRpc(ballNetworkObject.NetworkObjectId);
    }

    [ServerRpc]
    private void InstantiateHitParticlesServerRpc()
    {
       // instantiate the hit particles when we collide with something then destroy the ball
        GameObject hitImpact = Instantiate(hitParticles, transform.position, Quaternion.identity);
        hitImpact.GetComponent<NetworkObject>().Spawn();
        hitImpact.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
    }
}
