using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveProjectiles : MonoBehaviour
{
    public ShootBall parent;
    public GameObject playerOwner;
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
        if(other.gameObject != playerOwner)
        {
            GameObject hitImpact = Instantiate(hitParticles, transform.position, Quaternion.identity);
            hitImpact.transform.localEulerAngles = new Vector3(0f, 0f, -90f);

            parent.ballDestroyed();
            Destroy(gameObject);
        }
    }
}
