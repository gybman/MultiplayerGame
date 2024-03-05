using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float delayBeforeDestroy = 5f;

    private void Start()
    {
        // Start a coroutine to destroy the particle effect after the specified delay
        StartCoroutine(DestroyAfterDelay(delayBeforeDestroy));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Call the destruction method on the server
        Destroy(gameObject, delayBeforeDestroy);
    }
}
