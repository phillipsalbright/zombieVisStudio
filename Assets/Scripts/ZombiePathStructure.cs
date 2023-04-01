using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZombiePathStructure : MonoBehaviour
{
    [SerializeField, Tooltip("the points where the paths overalap with the circles. Used for creating navmesh")]
    List<List<Vector3>> overlapPoints;
    [SerializeField, Tooltip("unit vectors for the zombie paths. used to calculate overlap points")]
    List<Vector3> zombiePathDirections;
    [SerializeField, Tooltip("Circle radiuses. Used to calculate overlap points ")]
    List<float> zombieRingRadiuses;
    [SerializeField, Tooltip("the prefab for zombie path points")]
    private GameObject zombiePoint;
    [SerializeField, Tooltip("the actual zombie point objects. Used for navmesh")]
    private List<List<GameObject>> zombiePoints;
    [SerializeField, Tooltip("Spawn points")]
    private List<GameObject> zombieSpawnPoints;
    [SerializeField]
    private List<int> zombieCounts;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRings(List<float> rings){
        zombieRingRadiuses = rings;
    }

    public void SetPaths(List<Vector3> paths){
        zombiePathDirections = paths;
    }

    public void SetPoints(List<List<Vector3>> points, bool debug){
        overlapPoints = points;
        if(zombiePoints != null){
            DeletePoints();
        }
        GeneratePoints(debug);
        
    }

    public List<GameObject> getSpawnPoints(){
        return zombieSpawnPoints;
    }

    public void GeneratePoints(bool debug){
        zombiePoints = new List<List<GameObject>>();
        zombieSpawnPoints = new List<GameObject>();
        for(int i = 0; i < overlapPoints.Count; i++){
            zombiePoints.Add(new List<GameObject>());
            for(int j = 0; j < overlapPoints[i].Count; j++){
                try{
                    GameObject newPoint = Instantiate(zombiePoint, new Vector3(overlapPoints[i][j].x, 0, overlapPoints[i][j].z), Quaternion.identity, transform);
                    zombiePoints[i].Add(newPoint);
                    if(i == 0){
                        newPoint.GetComponent<ZombiePathPoint>().SetAttackPoint(true);
                    }
                    else{
                        ZombiePathPoint zpp = newPoint.GetComponent<ZombiePathPoint>();
                        zpp.AddMovePoint(zombiePoints[i-1][j]);
                        string output = "made points from (" + i + "," + j + ") to (" + (i - 1) + "," + j + ")";
                        if( i % 2 == j % 2){
                            if(j == 0){
                                zpp.AddMovePoint(zombiePoints[i-1][zombiePoints[i-1].Count - 1]);
                                output += ", (" + (i - 1) + "," + (zombiePoints[i-1].Count - 1) + ")";
                            }
                            else{
                                zpp.AddMovePoint(zombiePoints[i-1][j - 1]);
                                output += ", (" + (i - 1) + "," + (j-1) + ")";
                            }
                            zpp.AddMovePoint(zombiePoints[i-1][(j + 1) % zombiePoints[i-1].Count]);
                            output += ", and (" + (i - 1) + "," + ((j + 1) % zombiePoints[i-1].Count) + ")";
                        }
                        if(debug){
                            zpp.DebugDisplay();
                            Debug.Log(output);
                        }
                    }
                    if(i == overlapPoints.Count - 1){
                        zombieSpawnPoints.Add(newPoint);
                    }
                }
                catch(Exception e){
                    Debug.LogError("failed at index i=" + i + ", j=" + j + ": " + e.Message);
                }
            }
        }
    }

    public void DeletePoints(){
        while(zombiePoints.Count > 0){
            while(zombiePoints[0].Count > 0){
                GameObject o = zombiePoints[0][0];
                zombiePoints[0].RemoveAt(0);
                if(o != null){
                    DestroyImmediate(o);
                }
            }
            zombiePoints[0] = null;
            zombiePoints.RemoveAt(0);
        }
        zombiePoints = null;
    }
}
