using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public UIManager uiManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            uiManager.SetScore(50);
            other.gameObject.SetActive(false);
        }
    }
}
