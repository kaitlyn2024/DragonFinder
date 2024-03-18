using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
public class Exit : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode Reset = KeyCode.R;

    public bool hasEgg=false; 
    public GameObject noEgg;
    public GameObject inventory;
    public GameObject noEggLeave;
    public GameObject egg;
    public GameObject eggLeave;

    private void Start(){
        noEgg.SetActive(true);
        inventory.SetActive(true);
    }
    private void Update(){
        if(Input.GetKeyDown(Reset)){
            SceneManager.LoadScene("Menu");
        }
    }
    //
    private void OnTriggerEnter(Collider other)
    {
        print(other.tag);
        if(other.tag=="Player"){
            inventory.SetActive(false);
            if(hasEgg){
                egg.SetActive(false);
                eggLeave.SetActive(true);
            }else{
                noEgg.SetActive(false);
                noEggLeave.SetActive(true);
            }
        }
    }
    public void updateEgg(){
        if(hasEgg){
            egg.SetActive(true);
            noEgg.SetActive(false);
        }else{
            egg.SetActive(false);
            noEgg.SetActive(true);
        }
    }
}
