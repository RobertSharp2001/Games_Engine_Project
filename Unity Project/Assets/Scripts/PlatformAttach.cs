using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAttach : MonoBehaviour{

    public GameObject player;
    public float speed;
    public float distance;
    private float xStartPosition;
    private float zStartPosition;
    private float yStartPosition;

    public int dir = 1;

    public bool x;
    public bool negx;
    public bool z;
    public bool negz;
    public bool y;
    public bool negy;

    public void Awake(){
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update(){

        if (x) {
            if (transform.position.x < xStartPosition || transform.position.x > xStartPosition + distance){
                dir *= -1;
            }
            transform.position = new Vector3(transform.position.x + (speed * dir) * Time.deltaTime, transform.position.y, transform.position.z);
        }

        if (negx){
            if (transform.position.x > xStartPosition || transform.position.x < xStartPosition - distance) {
                dir *= -1;
            }
            transform.position = new Vector3(transform.position.x - (speed * dir) * Time.deltaTime, transform.position.y, transform.position.z);
        }

        if (z){
            if (transform.position.z < zStartPosition || transform.position.z > zStartPosition + distance) {
                dir *= -1;
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (speed * dir) * Time.deltaTime);
        }

        if (negz){
            if (transform.position.z > zStartPosition || transform.position.z < zStartPosition - distance)
            {
                dir *= -1;
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (speed * dir) * Time.deltaTime);
        }

        if (y) {
            if (transform.position.y < yStartPosition || transform.position.y > yStartPosition + distance){
                dir *= -1;
            }
            transform.position = new Vector3(transform.position.x, transform.position.y + (speed * dir) * Time.deltaTime, transform.position.z );
        }


        if (negy){
            if (transform.position.y > yStartPosition || transform.position.y < yStartPosition - distance){
                dir *= -1;
            }
            transform.position = new Vector3(transform.position.x, transform.position.y - (speed * dir) * Time.deltaTime, transform.position.z);
        }

    }

    void Start() {
        xStartPosition = transform.position.x;
        zStartPosition = transform.position.z;
        yStartPosition = transform.position.y;
    }

    public void OnTriggerEnter(Collider other){
        //Debug.Log("GO");
        if (other.gameObject == player){
            player.transform.parent = transform;
        }
    }

    public void OnTriggerExit(Collider other){
        if (other.gameObject == player){
            player.transform.parent = null;
        }
    }

}
