using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_bullet : MonoBehaviour{
    // Start is called before the first frame update
    [Header("Settings")]
    public float lifeTime = 0.3f;
    void Awake(){
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update(){
        
    }
}
