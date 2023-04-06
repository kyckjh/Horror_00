using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fuse Item Data", menuName = "Scriptable Object/Item Data - USB", order = 3)]
public class ItemData_USB : ItemData, IUsableItem
{
    public void Use()
    {
        UIManager.Inst.SetQuests(false, 2);    // USB에 환자 데이터 옮기기 퀘스트 완료
    }
}
