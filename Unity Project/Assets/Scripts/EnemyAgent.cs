using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static SettingsModel;

public class EnemyAgent : MonoBehaviour{
    // Start is called before the first frame update
    public NavMeshAgent agent;

    public Transform player;

    public enemySettings settings;

    public LayerMask isGround, isPlayer;

    [Header("Patrol")]
    public Vector3 walkpoint;
    bool walkPointSet;
    public float walkpointRange;

    [Header("Attacking")]
    public GameObject projectile;
    public GameObject audio;
    public float upForce;
    public float onForce;



    private void Awake(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update(){
        settings.playerinSightRange = Physics.CheckSphere(transform.position, settings.sightRange, isPlayer);
        settings.playerinAttackRange = Physics.CheckSphere(transform.position, settings.attackRange, isPlayer);

        if(!settings.playerinSightRange && !settings.playerinAttackRange){
            Patrolling();
        }


        if ((settings.playerinSightRange && !settings.playerinAttackRange) || settings.alert) {
            ChasePlayer();
        }

        if (settings.playerinSightRange && settings.playerinAttackRange) {
            AttackPlayer();
        }
    }

    public void Patrolling(){
        if (!walkPointSet) {
            SearchWalkPoint();
        }

        if (walkPointSet){
            agent.SetDestination(walkpoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkpoint;
        transform.LookAt(walkpoint);
        if (distanceToWalkPoint.magnitude < 1f) {
            walkPointSet = false;
        }
    }

    public void SearchWalkPoint(){
        float randomZ = Random.Range(-walkpointRange, walkpointRange);
        float randomX = Random.Range(-walkpointRange, walkpointRange);

        walkpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkpoint, -transform.up, 2f, isGround)){
            walkPointSet = true;
        }
    }

    public void ChasePlayer() {
        agent.SetDestination(player.position);
        transform.LookAt(player);
    }

    public void AttackPlayer() {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!settings.alreadyAttacked) {
            //ATTACK CODE HERE.

            Vector3 posA = transform.position;
            Vector3 posB = player.transform.position;
            Vector3 dir = (posB - posA).normalized;

            Quaternion rotation = Quaternion.Euler(dir);

            var bulletClone = Instantiate(projectile, transform.position, rotation);
            var audioClone = Instantiate(audio, transform.position, rotation);
            Rigidbody x = bulletClone.gameObject.GetComponent<Rigidbody>();
            bulletClone.gameObject.GetComponent<projectile>().type = BulletType.Enemy;
            bulletClone.gameObject.GetComponent<projectile>().damage = 5;
            x.velocity = transform.forward * 20f;


            settings.alreadyAttacked = true;
            Invoke(nameof(ResetAttack), settings.timeBetweenAttacks);
        }
    }

    private void ResetAttack() {
        settings.alreadyAttacked = false;
    }

    public void TakeDamage(int damage){
        settings.health -= damage;
        //agent.SetDestination(player.position);
        settings.alert = true;
        if (settings.health <= 0){
            Invoke(nameof(DestroySelf), .5f);
        }
    }

    public void DestroySelf(){
        Destroy(gameObject);
    }

}
