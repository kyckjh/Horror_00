using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desktop : MonoBehaviour, ISettableObject
{
    public bool isClear = false;

    [SerializeField]
    GameObject usbPrefab;

    Transform socket;

    private void Start()
    {
        socket = transform.GetChild(0);
    }

    public bool Set(ItemData data)
    {
        bool result = false;

        if(data == null)    // �Ѿ�� �����Ͱ� ���� �� �ٷ� false����
        {
            UIManager.Inst.SetMessagePanel("�������� ���õ��� �ʾҽ��ϴ�");
            return result;
        }
        if(data.id == (uint)ItemIDCode.USB)    // fuse ������ �ֱ� �õ�
        {
            ImportUSB(data);
            isClear = true;
            PatientList list = FindObjectOfType<PatientList>();
            if(list == null)
            {
                Door_Ending door = FindObjectOfType<Door_Ending>();
                door.UnLock();
                UIManager.Inst.SetQuests(true, 3);    // �������� Ż���ϱ� ����Ʈ Ȱ��ȭ
            }
            result = true;
        }
        else    // fuse�� �ƴ� ������ �ֱ� �õ�
        {
            UIManager.Inst.SetMessagePanel("����� �� ���� �������Դϴ�");
        }
        return result;
    }

    void ImportUSB(ItemData data)
    {
        ItemData_USB usbData = data as ItemData_USB;
        Instantiate(usbPrefab, socket);
        usbData.Use();
    }
}
