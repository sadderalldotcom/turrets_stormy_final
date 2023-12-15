using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInputController : MonoBehaviour
{
    [SerializeField] Transform hand;
    [SerializeField] IItem lastTouchedItem;
    [SerializeField] IItem heldItem;

    // Start is called before the first frame update
    void Start()
    {
        //heldItem = GameObject.Find("FlashlightPivot").GetComponent<Flashlight>();
    }

    // Update is called once per frame
    void Update()
    {   
        // are we holding an item right now?
        if(heldItem != null) {
            // if so, when we press the left mouse button, call the primary action
            if(Input.GetKeyDown(KeyCode.Mouse0)) {
                heldItem.PrimaryAction();
            }
            // if so, when we press the right mouse button, call the secondary action
            if(Input.GetKeyDown(KeyCode.Mouse1)) {
                heldItem.SecondaryAction();
            }
            
            if(Input.GetKeyDown(KeyCode.Q)) {
                heldItem.Drop();
                // set helditem to null.
                heldItem = null;
            }
        }
        // if we aren't holding something, let's try picking one up!
        else {
            if(Input.GetKeyDown(KeyCode.E)) {
                if(lastTouchedItem != null) {
                    // make this the helditem
                    heldItem = lastTouchedItem;
                    lastTouchedItem = null;
                    heldItem.Pickup(hand);
                } else {
                    Debug.Log("There are no items nearby!");
                }
                
            }
        }
        
    }

    // if we run into an item, we can pick it up!
    void OnTriggerEnter(Collider other) {
        Debug.Log("I've run into an object.");
        if(other.gameObject.CompareTag("Item")) {
            Debug.Log("The object is an item!");
            lastTouchedItem = other.gameObject.GetComponent<IItem>();
        }
    }

    // if we get far enough away from that item, we can't pick it up.
    void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Item")) {
            lastTouchedItem = null;
        }
    }
}
