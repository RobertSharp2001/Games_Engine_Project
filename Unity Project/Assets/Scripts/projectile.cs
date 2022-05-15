using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType{
    Player,
    Enemy
}

public class projectile : MonoBehaviour{
    // Start is called before the first frame update
    Rigidbody rb;
    public Transform player;
    public Vector3 moveTo;
    public BulletType type;
    public float aliveFor;
    public int damage;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        moveTo = GameObject.FindGameObjectWithTag("Player").transform.position;
        Invoke(nameof(Dest),aliveFor);
    }
    public void Dest(){
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update(){
        //MoveDir();
    }


    private void OnCollisionEnter(Collision collision){
        //Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.tag == "Player" && type != BulletType.Player){
            //Debug.Log("Enemy shot Player");
            scr_PlayerController playerController = collision.gameObject.GetComponent<scr_PlayerController>();
            playerController.TakeDamage(damage);
            Dest();
        }

        if (collision.gameObject.tag == "Enemy" && type != BulletType.Enemy){
            //Debug.Log("Enemy shot Player");
            EnemyAgent enem = collision.gameObject.GetComponent<EnemyAgent>();
            enem.TakeDamage(damage);
            Dest();
        }

    }
}
