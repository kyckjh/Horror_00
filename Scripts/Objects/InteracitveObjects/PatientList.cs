using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientList : MonoBehaviour, IUsableObject
{
    public bool Use(ItemData data)
    {
        Desktop desktop = FindObjectOfType<Desktop>();
        if (desktop.isClear == true)
        {
            Door_Ending door = FindObjectOfType<Door_Ending>();
            door.UnLock();
            UIManager.Inst.SetQuests(true, 3);    // 병원에서 탈출하기 퀘스트 활성화
        }
        UIManager.Inst.SetQuests(false, 0);
        Destroy(this.gameObject);
        return false;
    }
}
