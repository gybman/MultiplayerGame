using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DisableCameraForNonLocalPlayers : MonoBehaviour
{
    public GameObject cameraObject; // Drag camera object here in the inspector

    void Start()
    {
        if (GetComponent<NetworkObject>().IsLocalPlayer == false)
        {
            cameraObject.SetActive(false);
        }
    }
}
