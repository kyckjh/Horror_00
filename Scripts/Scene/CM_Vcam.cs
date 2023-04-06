using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CM_Vcam : MonoBehaviour
{
    CinemachineVirtualCamera vCamera;

    private void Awake()
    {
        vCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        NPC_BlackMan npc = FindObjectOfType<NPC_BlackMan>();
        vCamera.LookAt = npc.transform;
    }

    
}
