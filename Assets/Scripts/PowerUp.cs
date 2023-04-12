using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("general powerup settings")]
    [SerializeField, Tooltip("The time that this powerup lasts before despawning")]
    private float lifetime = 10f;
    //lifetime timer
    private float lifeTimer = 10f;
    // Start is called before the first frame update
    void Start()
    {
        lifeTimer = lifetime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lifeTimer -= Time.fixedDeltaTime;
        if(lifeTimer < 0){
            DespawnPowerUp();
        }
    }
    public virtual void Activate(int player){
        DespawnPowerUp();
    }

    private void DespawnPowerUp(){
        Destroy(gameObject);
    }
}
