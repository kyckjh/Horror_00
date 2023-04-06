using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Ending : MonoBehaviour, IUsableObject
{
    [SerializeField]
    bool isLock = true;
    [SerializeField]
    [Range(0, 10)]
    float openSpeed = 2.0f;

    Coroutine coroutine;

    public bool Use(ItemData data)
    {
        if(isLock)
        {
            UIManager.Inst.SetMessagePanel("아직 할 일이 남아있다...");
        }
        else
        {
            StartCoroutine(Open());
            isLock = true;
            GameManager.Inst.MainPlayer.OnGameClear();
            AudioManager.Inst.StopAllSFX();
            AudioManager.Inst.PlayBGM("OP");
        }
        return false;
    }

    IEnumerator Open()
    {
        while(transform.localRotation.y < 1200.0f)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 125.0f, 0), Time.deltaTime * openSpeed);
            yield return null;
        }
        yield return null;
    }

    public void UnLock()
    {
        UIManager.Inst.SetMessagePanel("모든 할 일을 마쳤다...");
        isLock = false;
    }
}
