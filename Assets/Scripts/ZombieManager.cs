using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField, Tooltip("the delay between spawning zombies")]
    float spawnDelay = 1f;
    //spawn delay timer
    [SerializeField]
    private float spawnTimer = 0f;
    [SerializeField, Tooltip("the list of spawn points")]
    List<GameObject> spawnPoints;
    [SerializeField, Tooltip("the zombie path structure")]
    ZombiePathStructure pathStructure;
    [SerializeField, Tooltip("the zombie prefab")]
    GameObject[] zombies;
    // Start is called before the first frame update
    void Start()
    {
        if(pathStructure == null){
            Debug.LogError("no path structure assigned.");
        }
        else{
            spawnPoints = pathStructure.getSpawnPoints();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        spawnTimer-=Time.deltaTime;
        if(spawnTimer < 0){
            spawnTimer = spawnDelay;
            SpawnZombie();
            
        }

    }

    void SpawnZombie(){
        Debug.Log("spawn zombie");
        GameObject spawnObject = spawnPoints[Random.Range(0,spawnPoints.Count)];
        int zombieIndex = Random.Range(0, zombies.Length);
        GameObject newZombie = Instantiate(zombies[zombieIndex], spawnObject.transform.position, Quaternion.identity);
        newZombie.GetComponent<Zombie>().SetDestination(spawnObject);
    }
}
