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
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.25f;
    [SerializeField] private float rotationSpeed = 5f;
    private float yPos;
    // Start is called before the first frame update
    void Start()
    {
        lifeTimer = lifetime;
        yPos = transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lifeTimer -= Time.fixedDeltaTime;
        if(lifeTimer < 0){
            DespawnPowerUp();
        }
        transform.position = new Vector3(transform.position.x, Mathf.Cos(Time.time * bobSpeed) * bobHeight + yPos, transform.position.z);
        transform.Rotate(Vector3.up * rotationSpeed * Time.fixedDeltaTime, Space.Self);
    }
    public virtual void Activate(int player){
        DespawnPowerUp();
    }

    private void DespawnPowerUp(){
        Destroy(gameObject);
    }
}
