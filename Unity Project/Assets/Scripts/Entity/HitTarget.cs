using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SettingsModel;

public class HitTarget : MonoBehaviour{
    // Start is called before the first frame update
    public entitySettings settings;
    public Material offMaterial;
    public Light light;

    public bool triggered = false;

    public void TakeDamage(){
        Trigger();
    }

    // Update is called once per frame
    void Trigger(){
        Renderer m_Material = GetComponent<Renderer>();
        m_Material.material = offMaterial;
        light.intensity = 0;
        triggered = true;
    }

    public void OnCollisionEnter(Collision collision){
        projectile target = collision.transform.GetComponent<projectile>();
        if (target != null){
            TakeDamage();
            //Debug.Log(hit.transform.name);
        }
    }
}
