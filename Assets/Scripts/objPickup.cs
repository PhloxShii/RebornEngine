using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objPickup : MonoBehaviour
{
    public Transform objTransform, cameraTrans;
    public bool interactable, pickedup;
    public Rigidbody objRigidbody;
    public Collider objCollider; // Reference to the object's collider
    public float throwAmount;
    float holdTime = 0f;
    float requiredHoldTime = 5f;

    void Start()
    {
        objCollider = GetComponent<Collider>(); // Assuming the collider is on the same GameObject
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (pickedup)
            {
                DropObject();
            }
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!pickedup)
                {
                    PickupObject();
                }
            }
            if (Input.GetMouseButton(1))
            {
                holdTime += Time.deltaTime;
            }
            else
            {
                holdTime = 0f;
            }

            if (pickedup && Input.GetMouseButtonUp(1))
            {
                if (holdTime >= requiredHoldTime)
                {
                    ThrowObject();
                }
                DropObject();
            }
        }
    }

    void PickupObject()
    {
        objTransform.parent = cameraTrans;
        objRigidbody.useGravity = false;
        objRigidbody.isKinematic = true; // Ensure the Rigidbody doesn't interfere with movement
        objCollider.enabled = false; // Disable collider when picked up
        pickedup = true;
    }

    void ThrowObject()
    {
        objRigidbody.velocity = cameraTrans.forward * throwAmount;
    }

    void DropObject()
    {
        objTransform.parent = null;
        objRigidbody.useGravity = true;
        objRigidbody.isKinematic = false;
        objCollider.enabled = true; // Re-enable collider when dropped
        pickedup = false;
    }
}
