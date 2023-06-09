using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using TMPro;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.EventSystems;

public class Pistol : Weapon
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
    [SerializeField] private ParticleSystem particlesMuzzleFlash;
    [SerializeField] private LineRenderer laserSight;
    [SerializeField] private Transform laserOrigin;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Sprite[] crosshairSprites;

    [SerializeField] private VisualEffect blood;
    private int magazineSize = 12;
    private int ammoInWeapon;

    private bool triggerPulled = false;
    private float timeBetweenShots = .2f;
    private float timeSinceLastShot = 0;
    private Quaternion rotationforce = Quaternion.identity;
    private Gamepad pad;
    private Animator anim;
    private bool firing = false;
    private bool reloading = false;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image healthImage;
    private PlayerHealthManager phm;
    private int playerNum;

    private void Start()
    {
        ammoInWeapon = magazineSize;
        pad = (Gamepad)pm.devices[0];
        anim = GetComponent<Animator>();
        phm = FindObjectOfType<PlayerHealthManager>();
        UpdateAmmoDisplay();
        playerNum = pm.playerIndex;
        laserSight.SetWidth(laserSight.startWidth * this.transform.root.lossyScale.x, laserSight.startWidth * this.transform.root.lossyScale.x);
        Debug.Log(playerNum);
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
        if (!firing && ammoInWeapon < magazineSize)
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
        ammoInWeapon = magazineSize;
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
            triggerPulled = false;
            if (ammoInWeapon > 0)
            {
                StartCoroutine(Fire());
                timeSinceLastShot = 0;
                muzzleFlash.SetActive(true);
                particlesMuzzleFlash.Play();
            }
            else
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
        rotationforce = new Quaternion(Mathf.Min(0, rotationforce.x + Time.deltaTime * 1.5f), 0, 0, 1.5f);
        healthImage.fillAmount = (phm.GetHealth() / phm.GetMaxHealth());
        //healthText.text = "Health: " + phm.GetHealth();
        RaycastHit objecthit;
        if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out objecthit, Mathf.Infinity, validLayers))
        {
            laserSight.SetPosition(1, new Vector3(0, 0, Mathf.Min((objecthit.point - laserOrigin.position).magnitude/ this.transform.lossyScale.x, 7)));
            crosshair.transform.position = objecthit.point + (laserOrigin.position - objecthit.point).normalized * .05f;
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
                crosshair.GetComponentInChildren<Image>().sprite = crosshairSprites[(playerNum*2 + 1) % crosshairSprites.Length];
            }
            else
            {
                crosshair.GetComponentInChildren<Image>().sprite = crosshairSprites[playerNum*2 % crosshairSprites.Length];
            }
            if (objecthit.collider.gameObject.layer == 5)
            {
                EventSystem.current.SetSelectedGameObject(objecthit.collider.gameObject);
            } else if (playerNum == 0)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

        }
        else
        {
            laserSight.SetPosition(1, new Vector3(0, 0, 7));
            crosshair.SetActive(false);
            if (playerNum == 0)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
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
        RaycastHit objecthit;
        if (Physics.Raycast(barrelForward.position, barrelForward.forward, out objecthit, Mathf.Infinity, validLayers))
        {
            if (objecthit.collider.gameObject.layer == 6)
            {
                if (objecthit.collider.gameObject.GetComponent<Hitbox>() != null) {
                    objecthit.collider.gameObject.GetComponent<Hitbox>().DamageBodyPart(3);
                } else {
                    objecthit.collider.gameObject.GetComponent<Zombie>().TakeDamage(3, 0);
                }
                VisualEffect bloodEffect = Instantiate(blood, objecthit.point, Quaternion.LookRotation((objecthit.point - barrelForward.position)), objecthit.transform);
                bloodEffect.Play();
            } else if (objecthit.collider.gameObject.layer == 7)
            {
                objecthit.collider.gameObject.GetComponent<PowerUp>().Activate(playerNum);
            }

        }
        rotationforce *= new Quaternion(-.25f, 0, 0, 1);
        ammoInWeapon--;
        UpdateAmmoDisplay();
        yield return new WaitForSeconds(.05f);
        if (pad != null)
        {
            pad.SetMotorSpeeds(0, 0);

        }
        firing = false;

    }

    private void UpdateAmmoDisplay()
    {
        ammoText.text = ": " + ammoInWeapon + "/inf";
    }
}
