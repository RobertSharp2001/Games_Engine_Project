using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour{
    // Start is called before the first frame update
    public List<HitTarget> buttons;
    public GameObject doorObject;

    // Update is called once per frame
    void Update(){
        if (TestButtons()){
            Destroy(doorObject);
        }
    }

    public bool TestButtons(){
        for (int i = 0; i <= buttons.Count-1; i++){
            if (buttons[i].triggered == false) {
                return false;
            }
        }
        return true;
    }
}
