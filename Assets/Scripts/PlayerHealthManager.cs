using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] private float maxHealth = 50;
    [SerializeField] private GameObject[] redScreens;
    private float health;
    private Animator anim;
    private bool dead = false;
    private int zombiesKilled;

    // Start is called before the first frame update
    void Start()
    {
        health = Mathf.Min(startingHealth, maxHealth);
        anim = GetComponentInParent<Animator>();
        zombiesKilled = 0;
    }

    private void Update()
    {
        //StartCoroutine(flashScreen(3));
    }

    public void DamagePlayer(float damage, Vector3 position)
    {
        if (dead)
        {
            return;
        }
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        Vector3 directionOfDamage = position - this.transform.position;
        directionOfDamage.y = 0;
        directionOfDamage = directionOfDamage.normalized;
        float angle = Vector3.Angle(this.transform.forward, directionOfDamage);
        int index = (int)Mathf.Floor(angle / 45);
        if (Vector3.Cross(directionOfDamage, this.transform.forward).y > 0)
        {
            index = 7 - index;
        }
        StartCoroutine(flashScreen(index));
    }

    private IEnumerator Die()
    {
        anim.SetTrigger("Death");
        FindObjectOfType<PlayerInputManager>().DisableJoining();
        PlayerInput[] players = FindObjectsOfType<PlayerInput>();
        dead = true;
        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }

        TMP_Text[] texts = anim.gameObject.GetComponentsInChildren<TMP_Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = "Zombies killed: " + zombiesKilled;
        }
        yield return new WaitForSeconds(10f);

        SceneManager.LoadScene(0);
    }

    private IEnumerator flashScreen(int index)
    {
        redScreens[index].SetActive(true);
        yield return new WaitForSeconds(.5f);
        redScreens[index].SetActive(false);
    }

    public float GetHealth()
    {
        return health;
    }

    public void Heal(float healAmount)
    {
        health = Mathf.Min(health + healAmount, maxHealth);
    }

    public void ZombieKilled()
    {
        zombiesKilled++;
    }

    public float GetMaxHealth()
    {
        return this.maxHealth;
    }
}
