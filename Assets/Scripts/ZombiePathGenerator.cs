using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePathGenerator : MonoBehaviour
{
    [Header("Generation settings")]
    [SerializeField, Tooltip("number of rings to be generated (including innermost ring directly around player)"), Range(2, 50)]
    private int numRings = 3;
    [SerializeField, Tooltip("number of paths that should exist for zombies to walk along. must be at least 1"), Range(1, 64)]
    private int numPaths = 8;
    [SerializeField, Tooltip("distance around player for near field. Distance zombies should stop at."), Min(0)]
    private float smallestCircleRadius = 14;
    [SerializeField, Tooltip("Distance to outermost circle. Distance at which zombies will spawn. \nNeeds to be greater than smallest"), Min(0)]
    private float largestCircleRadius = 100;
    [SerializeField, Tooltip("Center of the world. player position")]
    private Vector3 center = Vector3.zero;
    [SerializeField, Tooltip("should it display debug lines and rays")]
    private bool debugOn = true;
    [SerializeField, Tooltip("vector to be added to debug rays so they are visible. should be only in y directoin")]
    private Vector3 debugOffset = new Vector3(0, 1, 0);
    [Header("Structure")]
    [SerializeField, Tooltip("actual zombie path structure to be generated")]
    private ZombiePathStructure zPath;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate(){
        if(zPath == null){
            zPath = gameObject.AddComponent<ZombiePathStructure>();
        }
        List<float> rings = GenerateRings();
        List<Vector3> paths = GeneratePaths();
        GeneratePoints(debugOn, rings, paths);
    }

    public List<float> GenerateRings(){
        List<float> rings = new List<float>();
        float ringDist = (largestCircleRadius - smallestCircleRadius) / (numRings - 1);
        for(float i = smallestCircleRadius; i <= largestCircleRadius; i += ringDist){
            rings.Add(i);
        }
        zPath.SetRings(rings);
        return rings;
    }

    public List<Vector3> GeneratePaths(){
        float angleChange = 360f / numPaths;
        List<Vector3> paths = new List<Vector3>();
        for(float i = 0; i < 360; i += angleChange){
            Vector3 direction = Quaternion.Euler(0, i, 0) * Vector3.forward;
            paths.Add(direction.normalized);
        }
        zPath.SetPaths(paths);
        return paths;
    }

    public void GeneratePoints(bool debugRaysOn, List<float> rings, List<Vector3> paths){
        List<List<Vector3>> points = new List<List<Vector3>>();
        foreach(float ring in rings){
            List<Vector3> ringPoints = new List<Vector3>();
            foreach(Vector3 path in paths){
                ringPoints.Add(path * ring + center);
                if(debugRaysOn){
                    Debug.DrawRay(path * ring + center + debugOffset, Vector3.up * 5, Color.red, 10f );
                }
            }
            if(debugRaysOn){
                for(int i = 1; i < ringPoints.Count; i++){
                    Debug.DrawLine(ringPoints[i]+ debugOffset, ringPoints[i-1] + debugOffset, Color.green, 10f);
                }
                Debug.DrawLine(ringPoints[0]+ debugOffset, ringPoints[ringPoints.Count - 1] + debugOffset, Color.green, 10f);
            }
            points.Add(ringPoints);

        }
        if(debugRaysOn){
            foreach(Vector3 path in paths){
                Debug.DrawLine(center + debugOffset, center + (path * smallestCircleRadius) + debugOffset, Color.blue, 10f);
                Debug.DrawLine(center + (path * smallestCircleRadius) + debugOffset, center + (path * largestCircleRadius) + debugOffset, Color.blue, 10f);
            }
        }
        zPath.SetPoints(points, debugRaysOn);
    }

    public void DebugLines(){
        zPath.DebugLines();
    }
}
