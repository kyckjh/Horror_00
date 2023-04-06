using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Lamp : EventTrigger
{
    Light light;
    float originalIntensity = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        light = GetComponentInChildren<Light>();
        originalIntensity = light.intensity;
    }

    protected override void EventStart()
    {
        base.EventStart();
        StartCoroutine(LightBlink());
    }

    IEnumerator LightBlink()
    {
        yield return new WaitForSeconds(0.1f);
        light.intensity = 0;
        yield return new WaitForSeconds(0.1f);
        light.intensity = originalIntensity;
        yield return new WaitForSeconds(0.1f);
        light.intensity = 0;
        yield return new WaitForSeconds(0.5f);
        light.intensity = originalIntensity;
        yield return new WaitForSeconds(0.1f);
        light.intensity = 0;
        yield return new WaitForSeconds(0.1f);
        light.intensity = originalIntensity;
        yield return new WaitForSeconds(0.1f);
        light.intensity = 0;
        yield return new WaitForSeconds(0.1f);
        light.intensity = originalIntensity;
    }
}
