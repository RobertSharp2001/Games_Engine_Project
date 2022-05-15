using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
//to access mixer
public class SliderScript : MonoBehaviour{
    public AudioMixer audioMixer;
    public Slider Slider;
    public float Amount;

    void Start(){
        PlayerPrefs.GetFloat("Sensitivity");
        PlayerPrefs.SetFloat("Sensitivity", 0.5f);
    }

    public void SetSensitivity(float amount){//slider gets called to SetVolume{
        Debug.Log(amount);
        //to testand see debug log
        Amount = 0 + amount;
    }

    public void Update(){
        if (Amount < 0.3f){
            Amount = 0.3f;
        }

        PlayerPrefs.SetFloat("Sensitivity", Amount);

        Debug.Log(PlayerPrefs.GetFloat("Sensitivity"));
    }
}