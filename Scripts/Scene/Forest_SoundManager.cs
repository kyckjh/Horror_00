using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest_SoundManager : MonoBehaviour
{
    float maxDistance = 100.0f;
    [SerializeField]
    GameObject cart;

    void Start()
    {
        AudioManager.Inst.RepeatSFX("Car_Drive", -1);
        maxDistance = Vector3.Distance(Camera.main.transform.position, cart.transform.position)/2;
    }

    private void Update()
    {
        float distance = Vector3.Distance(Camera.main.transform.position, cart.transform.position);
        AudioManager.Inst.SoundVolume = Mathf.Lerp(0.8f, 0.1f, distance / maxDistance);
    }

    public void WindowSFX()
    {
        AudioManager.Inst.PlaySFX("Car_Window");
    }
}
