using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class AssaultRifle : Weapon
{
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private AudioSource firingSound;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Transform barrelForward;
    [SerializeField] private LayerMask validLayers;
    [SerializeField] private PlayerInput pm;
    private bool triggerPulled = false;
    private float timeBetweenShots = .4f;
    private float timeSinceLastShot = 0;
    private Quaternion rotationforce = Quaternion.identity;
    private Gamepad pad;

    private void Start()
    {
        pad = (Gamepad)GetComponent<PlayerInput>().devices[0];
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
        if (triggerPulled && timeSinceLastShot > timeBetweenShots)
        {
            StartCoroutine(Fire());
            timeSinceLastShot = 0;
            muzzleFlash.SetActive(true);
        } else if (timeSinceLastShot > timeBetweenShots)
        {
            muzzleFlash.SetActive(false);
        }
        gunTransform.localRotation = Quaternion.Lerp(gunTransform.localRotation, rotationforce, Time.deltaTime * 2);
        rotationforce = new Quaternion(Mathf.Min(0, rotationforce.x + Time.deltaTime), 0, 0, 1);
    }

    private IEnumerator Fire()
    {
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
                    objecthit.collider.gameObject.GetComponent<ZombieHealth>().TakeDamage(5);
                }
            }
            rotationforce *= new Quaternion(-.1f, 0, 0, 1);
            yield return new WaitForSeconds(.05f);
        }
        pad.SetMotorSpeeds(0, 0);

    }
}
