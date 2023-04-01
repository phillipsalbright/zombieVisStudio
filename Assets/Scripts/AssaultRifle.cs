using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AssaultRifle : Weapon
{
    [SerializeField] private Transform modelTransform;
    [SerializeField] private AudioSource firingSound;
    [SerializeField] private GameObject muzzleFlash;
    private bool triggerPulled = false;
    private float timeBetweenShots = .4f;
    private float timeSinceLastShot = 0;

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
            Fire();
            timeSinceLastShot = 0;
            muzzleFlash.SetActive(true);
        } else if (timeSinceLastShot > timeBetweenShots)
        {
            muzzleFlash.SetActive(false);
        }
    }

    private void Fire()
    {
        firingSound.Play();
    }
}
