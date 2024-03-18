using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Net.Configuration;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    [Header("References")]
    public Transform playerCameraPosition;
    public Transform playerThrowPosition;
    

    [Header("Settings")]

    public int activeIndex=0;
    private GameObject activeObject;
    public GameObject[] items;
    public int[] quantities;
    [SerializeField]
    public UnityEngine.UI.Image [] slots;
    public UnityEngine.UI.Image slot;
    
    [Header("Keys")]
    public KeyCode keyActivate0 = KeyCode.Keypad0;
    public KeyCode keyActivate1 = KeyCode.Keypad1;
    public KeyCode keyActivate2 = KeyCode.Keypad2;
    public KeyCode throwKey = KeyCode.Mouse1;
    public KeyCode pickUpKey = KeyCode.Mouse0;



    [Header("Throw")]
    public float throwForwardForce;
    public float throwUpwardForce;
    public float throwCooldown;
    public float throwCorrectRange;
    bool throwReady=true;
    [Header("Pickup")]
    public float pickUpRange;

    [Header("Exit")]
    public Exit exit;

    // Start is called before the first frame update
    void Start(){
        changeActiveItem();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActiveInput();
        
        Hold();
        if(Input.GetKeyDown(throwKey) && throwReady && quantities[activeIndex]>0){
            Throw();
        }
        if(Input.GetKeyDown(pickUpKey)){
            PickUp();
        }
               
    }

    private void UpdateActiveInput(){
        if(Input.GetKeyDown(keyActivate0)){
            activeIndex=0;
            changeActiveItem();
        }
        if(Input.GetKeyDown(keyActivate1)){
            activeIndex=1;
            changeActiveItem();
        }
        if(Input.GetKeyDown(keyActivate2)){
            activeIndex=2;
            changeActiveItem();
        }

        
    }
    private void changeActiveItem(){
            if(activeObject){
                Destroy(activeObject);
            }
            if(quantities[activeIndex]>0){
                activeObject=Instantiate(items[activeIndex], playerThrowPosition.position, playerCameraPosition.rotation);
            }
            for(int i=0; i<items.Length; i++){
                if(i!=activeIndex){
                    slots[i].color = new Color32(0, 0, 0, 100);
                }else{
                    slots[i].color = new Color32(255, 255, 255, 100);
                }
            }
            if(quantities[2]!=0){
                exit.hasEgg=true;
            }else{
                exit.hasEgg=false;
            }
            exit.updateEgg();
    }
    private void Throw(){
        throwReady=false;

        GameObject projectile = Instantiate(activeObject, playerThrowPosition.position, playerCameraPosition.rotation);
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
        Vector3 forceDirection = playerCameraPosition.transform.forward;
        RaycastHit hit;
        if(Physics.Raycast(playerCameraPosition.position, playerCameraPosition.forward, out hit, throwCorrectRange)){
            forceDirection = (hit.point - playerThrowPosition.position).normalized;
        }

        Vector3 throwForce = forceDirection*throwForwardForce + transform.up*throwUpwardForce; 
        projectileRB.AddForce(throwForce, ForceMode.Impulse);
        quantities[activeIndex]=quantities[activeIndex]-1;
        Invoke(nameof(ResetThrow), throwCooldown);

        changeActiveItem();
    }
    private void ResetThrow(){
        throwReady=true;
    }

    private void Hold(){
        if(activeObject!=null){
            activeObject.transform.position=playerThrowPosition.position;
        }
    }
    private void PickUp(){
        RaycastHit hit;
        if(Physics.Raycast(playerCameraPosition.position, playerCameraPosition.forward, out hit, pickUpRange)){
            print("hello");
            if(hit.collider.tag=="Flashlight"){
                quantities[0]++;
                Destroy(hit.transform.gameObject); 
            }else if(hit.collider.tag=="Flair"){
                quantities[1]++;
                Destroy(hit.transform.gameObject); 
            }else if(hit.collider.tag=="Egg"){
                quantities[2]++;
                Destroy(hit.transform.gameObject); 
            }
            changeActiveItem();
            print(hit.collider.name);
            
        }
    }
}
