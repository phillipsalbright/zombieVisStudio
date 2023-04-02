using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] private GameObject[] redScreens;
    private float health;

    // Start is called before the first frame update
    void Start()
    {
        health = startingHealth;
    }

    private void Update()
    {
        //StartCoroutine(flashScreen(3));
    }

    public void DamagePlayer(float damage, Vector3 position)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
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

    private void Die()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator flashScreen(int index)
    {
        redScreens[index].SetActive(true);
        yield return new WaitForSeconds(.5f);
        redScreens[index].SetActive(false);
    }
}
