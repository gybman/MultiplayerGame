using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform player;

    private void Update()
    {
        transform.position = player.transform.position; // moves the camera with the player
    }
}
