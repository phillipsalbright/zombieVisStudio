using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePathPoint : MonoBehaviour
{
    [SerializeField, Tooltip("Whether this is a point where a zombie should stop and begin to attack. Should only be true around innermost ring")]
    private bool attackPoint = false;
    [SerializeField, Tooltip("All of the possible ZombiePathPoints that the zombies can go to after this one. Should stay empty for innermost ring")]
    private List<GameObject> movePoints = new List<GameObject>();
    [SerializeField, Tooltip("the object mesh to be disabled on startup")]
    private GameObject capsuleMesh;

    // Start is called before the first frame update
    void Start()
    {
        if(capsuleMesh != null){
                capsuleMesh.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(List<GameObject> zombiePoints = null, bool attack = false){
        if(zombiePoints != null){
            movePoints = zombiePoints;
        }
        attackPoint = attack;
    }

    public void AddMovePoint(GameObject newPoint){
        if(!movePoints.Contains(newPoint)){
            movePoints.Add(newPoint);
        }
    }

    public void SetAttackPoint(bool attack){
        attackPoint = attack;
    }
    public bool GetAttackPoint(){
        return attackPoint;
    }

    public List<GameObject> GetPoints(){
        return movePoints;
    }

    public void DebugDisplay(){
        foreach(GameObject go in movePoints){
            Debug.DrawLine(transform.position, go.transform.position + new Vector3(0,1,0), Color.yellow, 10f);
        }
    }

    public GameObject GetRandomPoint(){
        if(movePoints == null || movePoints.Count == 0){
            return null;
        }
        if(movePoints.Count == 1){
            return movePoints[0];
        }
        int rand = Random.Range(0,movePoints.Count);
        return movePoints[rand];
    }

    public GameObject GetAggressivePoint(){
        if(movePoints == null || movePoints.Count == 0){
            return null;
        }
        return movePoints[0];
    }
    public GameObject GetCirclingPoint(){
        if(movePoints == null || movePoints.Count == 0){
            return null;
        }
        if(movePoints.Count == 1){
            return movePoints[0];
        }
        int rand = Random.Range(1,movePoints.Count);
        return movePoints[rand];
    }

    public void DestroyThis(){
        DestroyImmediate(gameObject);
    }

}
