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

    private void Start()
    {
        ammoInWeapon = magazineSize;
        reserveAmmo = startingReserve;
        pad = (Gamepad)pm.devices[0];
        anim = GetComponent<Animator>();
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
    }

    public override void PutAway()
    {

    }

    public override void MakeActive()
    {

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

            }
            rotationforce *= new Quaternion(-.12f, 0, 0, 1);
            ammoInWeapon--;
            yield return new WaitForSeconds(.05f);
        }
        if (pad != null)
        {
            pad.SetMotorSpeeds(0, 0);

        }
        firing = false;

    }
}
