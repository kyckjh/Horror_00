using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utill
{
    public static IEnumerator Delay(float second)
    {
        yield return new WaitForSeconds(second);
    }
}
