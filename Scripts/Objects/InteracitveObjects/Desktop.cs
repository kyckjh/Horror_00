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

        if(data == null)    // 넘어온 데이터가 없을 때 바로 false리턴
        {
            UIManager.Inst.SetMessagePanel("아이템이 선택되지 않았습니다");
            return result;
        }
        if(data.id == (uint)ItemIDCode.USB)    // fuse 아이템 넣기 시도
        {
            ImportUSB(data);
            isClear = true;
            PatientList list = FindObjectOfType<PatientList>();
            if(list == null)
            {
                Door_Ending door = FindObjectOfType<Door_Ending>();
                door.UnLock();
                UIManager.Inst.SetQuests(true, 3);    // 병원에서 탈출하기 퀘스트 활성화
            }
            result = true;
        }
        else    // fuse가 아닌 아이템 넣기 시도
        {
            UIManager.Inst.SetMessagePanel("사용할 수 없는 아이템입니다");
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
