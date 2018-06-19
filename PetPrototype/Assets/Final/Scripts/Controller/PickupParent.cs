using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PickupParent : MonoBehaviour {
    public Transform heldItem;
    public Transform AIs;
    private Collider itemCollider;
    public Transform lagTest;

    private void Update() {
        /*if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad)) {
            lagTest.GetComponent<SpeechRecognizerDemo>().startListening();
            return;
        }  else*/

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            //case 1: empty hand
            if (heldItem == null)
            {
                //case 1.1: item in collider
                if (itemCollider)
                {
                    PickUp(itemCollider);
                }
                //case 1.2: item pointed
                else
                {
                    itemCollider = transform.GetChild(0).GetComponent<LaserScript>().hitCollider;

                    if (itemCollider)
                        PickUp(itemCollider);
                } 
            }
            //case 2: holding item
            else
            {
                Toss(itemCollider);
            }
        }
    }

    private void PickUp(Collider col)
    {
        transform.GetChild(0).gameObject.SetActive(false); //hide controller

        heldItem = col.gameObject.transform;
        col.attachedRigidbody.isKinematic = true;
        col.transform.position = transform.position;
        col.gameObject.transform.SetParent(gameObject.transform);

        if (col.gameObject.GetComponent<Collider>().isTrigger)
            col.gameObject.GetComponent<Collider>().isTrigger = false;

        foreach (Transform child in AIs)
        {
            if (child.GetComponent<AIStateMachine>().attention)
            {
                child.GetComponent<AIStateMachine>().objectTransform = heldItem;
                child.GetComponent<AIStateMachine>().setRandomLookTarget();
            }
        }
    }

    private void Toss(Collider col)
    {
        transform.GetChild(0).gameObject.SetActive(true); //show controller

        heldItem = null;
        col.attachedRigidbody.isKinematic = false;
        col.attachedRigidbody.velocity = new Vector3(transform.forward.x, transform.forward.y + 0.5f, transform.forward.z) * 2f;
        col.gameObject.transform.SetParent(GameObject.Find("Items").transform);

        foreach (Transform child in AIs)
        {
            if (child.GetComponent<AIStateMachine>().attention)
            {
                //should have different turn/move speed
                child.GetComponent<AIStateMachine>().setAttention(true, 15f);
                child.GetComponent<AIStateMachine>().setMoveTarget(itemCollider.transform, 15f);
                child.GetComponent<AIStateMachine>().objectTransform = itemCollider.transform;
                child.GetComponent<AIStateMachine>().setStoppingDistance(0, 0.5f);
                //child.GetComponent<AICharacterControl>().agent.stoppingDistance = 0;
            }
        }
    }

    void OnTriggerEnter(Collider col) {
        if (heldItem != null)
            return;

        if (col.tag.Equals("Item")) 
            itemCollider = col;
    }

    void OnTriggerExit(Collider col) {
        if (itemCollider == col) 
            itemCollider = null;
        
        if (heldItem && heldItem.gameObject == col.gameObject) 
            heldItem = null; 
    }
}
