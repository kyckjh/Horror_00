using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Wood : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.Inst.MainPlayer.IsOnWood = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Inst.MainPlayer.IsOnWood = false;
        }
    }
}
