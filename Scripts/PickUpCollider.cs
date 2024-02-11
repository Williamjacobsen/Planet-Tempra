using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCollider : MonoBehaviour
{
    private static GameObject plant;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Plant")
        {
            plant = other.gameObject;
            Inventory.PickUpCollider = other.gameObject.name;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (Inventory.PickUpCollider == other.gameObject.name)
        {
            plant = null;
            Inventory.PickUpCollider = null;
        }
    }

    public static IEnumerator DestoryPlant()
    {
        if (plant != null)
        {
            yield return new WaitForSeconds(0.85f); // wait a bit (animation)
            Destroy(plant);
            plant = null;
            Inventory.PickUpCollider = null;
        }
    }
}
