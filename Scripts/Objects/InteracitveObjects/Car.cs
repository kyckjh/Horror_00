using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Car : MonoBehaviour, IUsableObject
{
    PlayableDirector timeline;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public bool Use(ItemData data)
    {
        timeline = FindObjectOfType<PlayableDirector>();
        Player_Opening player = FindObjectOfType<Player_Opening>();
        Destroy(player.gameObject, 2.0f);
        anim.SetTrigger("Open");
        timeline.Play();
        UIManager_Opening.Inst.DisableMainCanvas();
        return false;
    }
}
