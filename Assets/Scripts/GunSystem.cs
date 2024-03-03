using UnityEngine;
using TMPro;
using Unity.Netcode;

public class GunSystem : NetworkBehaviour
{
    // Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    // bools
    bool shooting, readyToShoot, reloading;

    // Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    // Graphics (Use brackeys cam shake video)
    //public CamShake camShake;
    //public float camShakeMagnitude, camShakeDuration;
    public GameObject muzzleFlash, bulletHoleGraphic;
    // public TextMeshProUGUI text;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            this.enabled = false;
        

        // initialize ammo count on spawn
        readyToShoot = true;
        bulletsLeft = magazineSize;
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        MyInput();

        //SetText
        // text.SetText(bulletsLeft + " / " + magazineSize);
    }


    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            ShootServerRpc();
            Debug.Log(bulletsLeft);
        }
    }

    // This method is called by clients (including the host) to request shooting.
    [ServerRpc]
    public void ShootServerRpc()
    {
        readyToShoot = false;

        //Spread
        // can increase spread while running with, if(rigidbody.velocity.magnitude > 0) spread = spread * 1.5f; else spread = "normal spread";
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Calculate direction with spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Player"))
                rayHit.collider.GetComponent<PlayerHealth>().TakeDamage(damage);  // tag players with "Player" tag and create script which as TakeDamage function
        }

        // ShakeCamera
        // camShake.Shake(camShakeDuration, camShakeMagnitude);


        CreateBulletHoleClientRpc(rayHit.point, Quaternion.LookRotation(rayHit.normal));
        CreateMuzzleFlashClientRpc(attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot--;
        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);

        // Update the client's ammo count
        UpdateAmmoCountClientRpc(bulletsLeft);
    }

    [ClientRpc]
    private void UpdateAmmoCountClientRpc(int count)
    {
        bulletsLeft = count;
    }


    [ClientRpc]
    public void CreateMuzzleFlashClientRpc(Vector3 position, Quaternion rotation)
    {
        // Instantiate the muzzle flash effect locally for each client here
        GameObject flashInstance = Instantiate(muzzleFlash, position, rotation);
        Destroy(flashInstance, 0.3f); // Destroy the muzzle flash object after 0.3 seconds
    }

    // Call this method within your Shoot method after a successful hit detection
    [ClientRpc]
    public void CreateBulletHoleClientRpc(Vector3 position, Quaternion rotation)
    {
        Instantiate(bulletHoleGraphic, position, rotation);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
