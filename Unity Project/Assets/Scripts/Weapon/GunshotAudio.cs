using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunshotAudio : MonoBehaviour{
    // Start is called before the first frame update
    void Start(){
        Invoke(nameof(End),2f);
    }

    // Update is called once per frame
    void End(){
        Destroy(gameObject);
    }
}
