using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using TMPro;
using UnityEngine.UI;

public class AssaultRifle : Weapon
{
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform recoilTransform;
    [SerializeField] private AudioSource firingSound;
    [SerializeField] private AudioSource emptySound;
    [SerializeField] private AudioSource reloadSound;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Transform barrelForward;
    [SerializeField] private LayerMask validLayers;
    [SerializeField] private PlayerInput pm;
    private int magazineSize = 36;
    private int startingReserve = 72;
    private int reserveAmmo;
    private int ammoInWeapon;

    private bool triggerPulled = false;
    private float timeBetweenShots = .4f;
    private float timeSinceLastShot = 0;
    private Quaternion rotationforce = Quaternion.identity;
    private Gamepad pad;
    private Animator anim;
    private bool firing = false;
    private bool reloading = false;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text healthText;
    private PlayerHealthManager phm;
    [SerializeField] private LineRenderer laserSight;
    [SerializeField] private Transform laserOrigin;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Sprite[] crosshairSprites;
    private int playerNum;

    private void Start()
    {
        ammoInWeapon = magazineSize;
        reserveAmmo = startingReserve;
        pad = (Gamepad)pm.devices[0];
        anim = GetComponent<Animator>();
        phm = FindObjectOfType<PlayerHealthManager>();
        UpdateAmmoDisplay();
        playerNum = pm.playerIndex;
        laserSight.SetWidth(laserSight.startWidth * this.transform.root.lossyScale.x, laserSight.startWidth *this.transform.root.lossyScale.x);
    }

    public override void AttackDown()
    {
        triggerPulled = true;
    }

    public override void AttackRelease()
    {
        triggerPulled = false;
    }

    public override void Reload()
    {
        if (!firing && reserveAmmo > 0 && ammoInWeapon < magazineSize)
        {
            StartCoroutine(ReloadEnumator());

        }
    }

    private IEnumerator ReloadEnumator()
    {
        anim.SetTrigger("Reload");
        reloading = true;
        yield return new WaitForSeconds(1f);
        reloadSound.Play();
        yield return new WaitForSeconds(1f);
        reserveAmmo += ammoInWeapon;
        if (reserveAmmo / magazineSize > 0)
        {
            ammoInWeapon = magazineSize;
            reserveAmmo -= magazineSize;
        } else
        {
            ammoInWeapon = reserveAmmo;
            reserveAmmo = 0;
        }
        reloading = false;
        UpdateAmmoDisplay();
    }

    public override void PutAway()
    {
        this.gameObject.SetActive(false);
        reloading = false;
        firing = false;
    }

    public override void MakeActive()
    {
        this.gameObject.SetActive(true);
    }

    public override Transform GetModelTransform()
    {
        return modelTransform;
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        if (triggerPulled && timeSinceLastShot > timeBetweenShots && !reloading)
        {
            if (ammoInWeapon >= 3)
            {
                StartCoroutine(Fire());
                timeSinceLastShot = 0;
                muzzleFlash.SetActive(true);

            } else
            {
                timeSinceLastShot = 0;
                emptySound.Play();
            }
            // For UI
            Ray ray = new Ray(barrelForward.position, barrelForward.forward);
            LayerMask l = LayerMask.GetMask("UI");
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity, l);
            if (hit.collider != null)
            {
                hit.collider.gameObject.GetComponent<Button>().onClick.Invoke();
            }
        } else if (timeSinceLastShot > timeBetweenShots)
        {
            muzzleFlash.SetActive(false);
        }
        recoilTransform.localRotation = Quaternion.Lerp(recoilTransform.localRotation, rotationforce, Time.deltaTime * 2);
        rotationforce = new Quaternion(Mathf.Min(0, rotationforce.x + Time.deltaTime), 0, 0, 1);
        healthText.text = "Health: " + phm.GetHealth();
        RaycastHit objecthit;
        if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out objecthit, Mathf.Infinity, validLayers))
        {
            laserSight.SetPosition(1, new Vector3(0, 0, Mathf.Min((objecthit.point - laserOrigin.position).magnitude / this.transform.lossyScale.x, 7)));
            crosshair.transform.position = objecthit.point;
            if ((objecthit.point - laserOrigin.position).magnitude <= 15)
            {
                crosshair.SetActive(true);

            }
            else
            {
                crosshair.SetActive(false);
            }
            if (firing)
            {
                crosshair.GetComponentInChildren<Image>().sprite = crosshairSprites[(playerNum * 2 + 1) % crosshairSprites.Length];
            }
            else
            {
                crosshair.GetComponentInChildren<Image>().sprite = crosshairSprites[playerNum * 2 % crosshairSprites.Length];
            }
        }
        else
        {
            laserSight.SetPosition(1, new Vector3(0, 0, 7));
            crosshair.SetActive(false);
        }
    }

    private IEnumerator Fire()
    {
        firing = true;
        firingSound.Play();
        if (pad != null)
        {
            pad.SetMotorSpeeds(.9f, .9f);
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 3; i++)
        {
            RaycastHit objecthit;
            if (Physics.Raycast(barrelForward.position, barrelForward.forward, out objecthit, Mathf.Infinity, validLayers))
            {
                if (objecthit.collider.gameObject.layer == 6)
                {
                    objecthit.collider.gameObject.GetComponent<Zombie>().TakeDamage(5);
                }
                else if (objecthit.collider.gameObject.layer == 7)
                {
                    objecthit.collider.gameObject.GetComponent<PowerUp>().Activate(playerNum);
                }

            }
            rotationforce *= new Quaternion(-.12f, 0, 0, 1);
            ammoInWeapon--;
            UpdateAmmoDisplay();
            yield return new WaitForSeconds(.05f);
        }
        if (pad != null)
        {
            pad.SetMotorSpeeds(0, 0);

        }
        firing = false;

    }

    private void UpdateAmmoDisplay()
    {
        ammoText.text = "Bullets: " + ammoInWeapon + "/" + reserveAmmo;
    }

    public void GainAmmo(int ammo)
    {
        reserveAmmo += ammo;
    }
}
