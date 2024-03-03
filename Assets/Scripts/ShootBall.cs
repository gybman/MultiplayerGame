using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ShootBall : NetworkBehaviour
{
    //Fireball prefab
    [SerializeField] private GameObject fireball;

    //Shoot Position
    [SerializeField] private Transform shootTransform;

    // List to hold all the instantiated fireballs
    [SerializeField] private List<GameObject> spawnedBalls = new List<GameObject>();
    private void Update()
    {
        if (!IsOwner) return;
        // Shoot ball when clicking mouse button0 using the position and the direction of shooting transform
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootServerRpc();
        }
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        GameObject go = Instantiate(fireball, shootTransform.position, shootTransform.rotation);
        spawnedBalls.Add(go);
        go.GetComponent<MoveProjectiles>().parent = this;
        go.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership =false)]
    public void DestroyServerRpc(ulong networkObjectId)
    {
        //GameObject toDestroy = spawnedBalls[0];
        //toDestroy.GetComponent<NetworkObject>().Despawn();
        //spawnedBalls.Remove(toDestroy);
        //Destroy(toDestroy);
        // Get the NetworkObject associated with the provided ID
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject networkObject))
        {
            // Despawn and destroy the NetworkObject
            networkObject.Despawn();
            Destroy(networkObject.gameObject);
        }
        else
        {
            Debug.LogWarning("Failed to find NetworkObject with ID: " + networkObjectId);
        }
    }
}
