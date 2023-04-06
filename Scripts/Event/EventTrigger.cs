using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    Animator anim;
    Collider eventCollider;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        eventCollider = GetComponent<Collider>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EventStart();
        }
    }

    protected virtual void EventStart()
    {
        anim.SetTrigger("Activate");
        eventCollider.enabled = false;
    }

    void OnEventEnd()
    {
        Destroy(this.gameObject);
    }
}


