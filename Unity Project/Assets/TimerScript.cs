using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour{
    public float maxTime;

    private float starttime;
    public Text text;
    public scr_PlayerController player;
    
    // Start is called before the first frame update
    void Start(){
        starttime = Time.time;
    }

    // Update is called once per frame
    void Update(){

        float timepassed = Time.time;

        float timer = maxTime - (timepassed - starttime);

        text.text = timer.ToString();


        if (timepassed - starttime >= maxTime){
            player.playersettings.health = 0;
        }
    }
}
