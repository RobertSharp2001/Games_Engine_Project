using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour{
    public void playGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void toMenu(){
        SceneManager.LoadScene(0);
    }

    public void toRespawn(scr_PlayerController player){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void quitGame(){
        Debug.Log("Quitting");
        Application.Quit();
    }
}
