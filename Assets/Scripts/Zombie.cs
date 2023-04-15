using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public enum ZombieAIType{
        Normal,
        Aggressive,
        Circling
    }

    public enum DamageType
    {
        Body = 0, 
        Head = 1, 
        LeftArm = 2, 
        RightArm = 3
    }
    [SerializeField, Tooltip("This zombie's destination. Should be set automatically")]
    GameObject dest;
    [SerializeField, Tooltip("this zombie's navmeshagent")]
    NavMeshAgent navAgent;
    [Header("Navigation settings")]
    [SerializeField, Tooltip("this zombie's navigation mode. If set to true, will wait for collision to look for new destination. \nOtherwise will use the assigned distance from the target destination")]
    private bool useAgentCollision = true;
    [SerializeField, Tooltip("if not using agent collision, this is the minimum distance from the target the zombie must be before getting a new destination")]
    private float minAgentDist = 2f;
    [SerializeField, Tooltip("once this has been set to true, the zombie will stop moving and start attacking instead")]
    private bool atAttackDestination = false;
    [SerializeField, Tooltip("zombie's ai type. \nNormal will follow random paths that generall move inwards. \nAggressive will always take the shortest path to you. \nCircling will try to take roundabout paths whenever possible while still moving towards you")]
    private ZombieAIType aiType = ZombieAIType.Normal;


    //Animation stuff
    [SerializeField] Animator animator;

    // Health Stuff
    [SerializeField] private float startingHealth;
    private float health;
    private float timeBetweenAttacks = 3.3f;
    private float timeSinceLastAttack = 2f;
    private bool dead = false;
    [SerializeField] private float headHealth;
    [SerializeField] private float leftArmHealth;
    [SerializeField] private float rightArmHealth;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;

    // Start is called before the first frame update
    void Start()
    {
        if(navAgent == null){
            navAgent = gameObject.GetComponent<NavMeshAgent>();
            if(navAgent == null){
                Debug.LogWarning("had to generate navMeshAgent at runtime. may cause issues");
                navAgent = gameObject.AddComponent<NavMeshAgent>();
            }
        }
        health = startingHealth;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!useAgentCollision){
            if(!atAttackDestination && dest != null && Vector3.Distance(transform.position, dest.transform.position) < minAgentDist){
                GetNextDest();
            }
            else if(atAttackDestination && dest != null && Vector3.Distance(transform.position, dest.transform.position) > minAgentDist){
                ContinueMoving();
            }
        }
        if (atAttackDestination)
        {
            timeSinceLastAttack += Time.deltaTime;
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                StartCoroutine(Attack());
                timeSinceLastAttack = 0;
            }
        }
    }

    private IEnumerator Attack()
    {
        animator.SetTrigger("AttackingTrigger");
        yield return new WaitForSeconds(.75f);
        if (!dead)
        {
            GameObject.FindObjectOfType<PlayerHealthManager>().DamagePlayer(5, this.transform.position);
        }
    }

    void OnTriggerEnter(Collider col){
        
        if(col.gameObject == dest){
            GetNextDest();
        }
    }

    void LeftAttackArea(){
        atAttackDestination = false;
        ContinueMoving();
    }
    void ContinueMoving(){
        navAgent.isStopped = false;
        animator.SetBool("Walking", true);
    }
    void StopMoving(){
        navAgent.isStopped = true;
        animator.SetBool("Walking", false);
    }

    public void SetDestination(GameObject destination){
        dest = destination;
        navAgent.SetDestination(dest.transform.position);
        animator.SetBool("Walking", true);
       // Debug.LogWarning("new destination acquired");
    }

    public void GetNextDest(){
        if(dest.GetComponent<ZombiePathPoint>().GetAttackPoint()){
            atAttackDestination = true;
            StopMoving();
            Debug.LogWarning("attacking now. Graag");
        }
        else{
            if(aiType == ZombieAIType.Aggressive){
                SetDestination(dest.GetComponent<ZombiePathPoint>().GetAggressivePoint());
            }
            else if(aiType == ZombieAIType.Aggressive){
                SetDestination(dest.GetComponent<ZombiePathPoint>().GetCirclingPoint());
            }
            else{
                SetDestination(dest.GetComponent<ZombiePathPoint>().GetRandomPoint());
            }
            
        }
    }
    public void TakeDamage(float damage, int damageType)
    {
        animator.SetTrigger("Hit");
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        switch((DamageType)damageType) {
            case DamageType.Body:
                break;
            case DamageType.Head:
                if (head.activeInHierarchy) {
                    headHealth -= damage;
                    if (headHealth <= 0) {
                        head.SetActive(false);
                    }
                }
                break;
            case DamageType.LeftArm:
                if (leftArm.activeInHierarchy) {
                    leftArmHealth -= damage;
                    if (leftArmHealth <= 0) {
                        leftArm.SetActive(false);
                    }
                }
                break;
            case DamageType.RightArm:
                if (head.activeInHierarchy) {
                    rightArmHealth -= damage;
                    if (rightArmHealth <= 0) {
                        rightArm.SetActive(false);
                    }
                }
                break;
            default: break;
        }
    }

    private IEnumerator Die()
    {
        animator.SetTrigger("Death");
        dead = true;
        StopMoving();
        this.enabled = false;
        this.GetComponent<Collider>().enabled = false;
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
        FindObjectOfType<PlayerHealthManager>().ZombieKilled();
        yield return new WaitForSeconds(1f);
        PowerUpManager.Instance.OnDeathPowerUpSpawn(transform.position);
        yield return new WaitForSeconds(10f);
        Destroy(this.gameObject);
    }
}
