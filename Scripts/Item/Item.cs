using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ 1���� ��Ÿ�� Ŭ����
/// </summary>
public class Item : MonoBehaviour
{
    public ItemData data;   // �� ������ �������� ������ ������

    private void Start()
    {
        // ������ ����. Awake�� ���� data�� ��� Start���� ����
        GameObject obj = Instantiate(data.prefab, transform.position, transform.rotation * data.prefab.transform.rotation, transform);
        obj.layer = LayerMask.NameToLayer("Item");
    }

    /// <summary>
    /// ������ ȹ��
    /// </summary>
    /// <param name="inven">�������� �߰��� �κ��丮</param>
    /// <returns>������ ȹ�� ���� ����</returns>
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
