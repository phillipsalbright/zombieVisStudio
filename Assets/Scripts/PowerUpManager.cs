using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerUpManager : Singleton<PowerUpManager>
{
    [Serializable]
    struct powerupRNG{
        [Tooltip("the powerup object that will be spawned")]
        public GameObject powerUp;
        [Tooltip("the weighted chance of this item spawning when a powerup spawns")]
        public float spawnChanceWeight;
    }
    [Header("PowerUp settings")]
    [SerializeField, Tooltip("the powerups to spawn")]
    private List<powerupRNG> powerups;
    //total item weights
    private float totalRandWeights = 0;
    [Header("On Kill PowerUps settings")]
    [SerializeField, Tooltip("whether on kill PowerUps are active")]
    private bool onKill = true;
    [SerializeField, Tooltip("chance of PowerUp spawn on zombie death"), Range(0f,1f)]
    private float onKillChance = .3f;
    
    [Header("Area spawn PowerUps settings")]
    [SerializeField, Tooltip("whether area spawn PowerUps are active")]
    private bool areaSpawns = true;
    [SerializeField, Tooltip("the inner radius for the spawn area"), Min(0f)]
    private float innerRadius = 14f;
    [SerializeField, Tooltip("the outer radius for the spawn area"), Min(0f)]
    private float outerRadius = 100f;
    [SerializeField, Tooltip("the lower y bound for the powerup spawns"), Min(0f)]
    private float bottomYVal = 1f;
    [SerializeField, Tooltip("the upper y bound for the powerup spawns")]
    private float topYVal = 10f;
    [SerializeField, Tooltip("the center of this field")]
    private Vector3 center = Vector3.zero;
    [SerializeField, Tooltip("the delay between powerup spawns"), Min(.5f)]
    private float powerUpSpawnDelay = 5f;
    //powerup spawn timer
    [SerializeField]
    private float powerUpSpawnTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        powerUpSpawnTimer = powerUpSpawnDelay;
        foreach(powerupRNG powerUp in powerups){
            totalRandWeights += powerUp.spawnChanceWeight;
        }
        if(powerups == null){
            powerups = new List<powerupRNG>();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(areaSpawns){
            powerUpSpawnTimer -= Time.deltaTime;
            if(powerUpSpawnTimer < 0){
                SpawnAreaPowerUp();
                powerUpSpawnTimer = powerUpSpawnDelay;

            }
        }
    }

    private GameObject getPowerUpToSpawn(){
        float randVal = UnityEngine.Random.Range(0f,totalRandWeights);
        float countVal = 0f;
        foreach(powerupRNG powerUp in powerups){
            countVal += powerUp.spawnChanceWeight;
            if(randVal <= countVal){
                return powerUp.powerUp;
            }
        }
        return powerups[powerups.Count - 1].powerUp;
    }

    private void SpawnAreaPowerUp(){
        Vector3 direction = Quaternion.Euler(0, UnityEngine.Random.Range(0,360), 0) * Vector3.forward;
        direction = direction.normalized;
        Vector3 point = direction * UnityEngine.Random.Range(innerRadius, outerRadius);
        point = new Vector3(point.x, UnityEngine.Random.Range(bottomYVal, topYVal), point.z);
        SpawnPowerUp(point);
    }

    private void SpawnPowerUp(Vector3 location){
        Instantiate(getPowerUpToSpawn(), location, Quaternion.identity, transform);
        Debug.Log("spawned power up!");
    }

    public void OnDeathPowerUpSpawn(Vector3 location){
        SpawnPowerUp(location);
    }

    public void OnDeathPowerUpSpawn(GameObject obj){
        if(UnityEngine.Random.Range(0f,1f) < onKillChance){
            OnDeathPowerUpSpawn(obj.transform.position);
        }
        
    }

    public void Heal(int health){

    }

    public void RestoreAmmo(int ammo){

    }
}
