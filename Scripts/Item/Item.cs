using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 1개를 나타낼 클래스
/// </summary>
public class Item : MonoBehaviour
{
    public ItemData data;   // 이 아이템 종류별로 동일한 데이터

    private void Start()
    {
        // 프리팹 생성. Awake일 때는 data가 없어서 Start에서 실행
        GameObject obj = Instantiate(data.prefab, transform.position, transform.rotation * data.prefab.transform.rotation, transform);
        obj.layer = LayerMask.NameToLayer("Item");
    }

    /// <summary>
    /// 아이템 획득
    /// </summary>
    /// <param name="inven">아이템을 추가할 인벤토리</param>
    /// <returns>아이템 획득 성공 여부</returns>
    public void GetItem(Inventory inven)
    {
        bool result;
        result = inven.AddItem(data);
        if(result)
        {
            if(data is ItemData_USB)
            {
                UIManager.Inst.SetQuests(false, 1);
                UIManager.Inst.SetQuests(true, 2);
            }
            Destroy(this.gameObject);
        }
        //return result;
    }
}
