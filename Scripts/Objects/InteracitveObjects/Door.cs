using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IUsableObject
{

    [SerializeField]
    bool isLock = true;

    bool isOpen = false;
    float angle = 0.0f;
    [SerializeField]
    [Range(0, 10)]
    float openSpeed = 2.0f;

    Coroutine coroutine;

    public bool Use(ItemData data)
    {
        if(isLock)
        {
            UIManager.Inst.SetMessagePanel("문이 잠겨있습니다");
        }
        else
        {
            if(!isOpen)
            {
                isOpen = true;
                StopMethod();
                coroutine = StartCoroutine(Open());
            }
            else
            {
                isOpen = false;
                StopMethod();
                coroutine = StartCoroutine(Close());
            }
        }
        return false;
    }

    void StopMethod()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
    IEnumerator Open()
    {
        while(transform.localRotation.y > -90.0f)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, -90.0f, 0), Time.deltaTime * openSpeed);
            yield return null;
        }
        yield return null;
    }
    IEnumerator Close()
    {
        while (transform.localRotation.y < 0.0f)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0.0f, 0), Time.deltaTime * openSpeed);
            yield return null;
        }
        yield return null;
    }

    public void UnLock()
    {
        UIManager.Inst.SetMessagePanel("문이 열렸습니다");
        isLock = false;
    }
}
