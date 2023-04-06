using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, IUsableObject
{
    Animator anim;
    [SerializeField]
    CardKeyLock cardKeyLock;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public bool Use(ItemData data)
    {
        anim.SetBool("SwitchOn", !anim.GetBool("SwitchOn"));
        cardKeyLock.Activate();
        return false;
    }


}
