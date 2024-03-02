using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
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


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
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
            Shoot();
        }
    }

    private void Shoot()
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

        // Graphics
        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
        // Instantiate muzzle flash at gun's firing point
        GameObject flashInstance = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        // Destroy the muzzle flash object after 1 second
        Destroy(flashInstance, 0.3f);


        bulletsLeft--;
        bulletsShot--;
        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
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
