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
            UIManager.Inst.SetQuests(true, 3);    // �������� Ż���ϱ� ����Ʈ Ȱ��ȭ
        }
        UIManager.Inst.SetQuests(false, 0);
        Destroy(this.gameObject);
        return false;
    }
}
