using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : MonoBehaviour, ISettableObject
{
    [SerializeField]
    GameObject fusePrefab;
    [SerializeField]
    Transform[] socket;
    [SerializeField]
    ElevatorManager elevator;

    int socketCount;
    int currentCount;

    private void Start()
    {
        socketCount = socket.Length; 
        currentCount = 0;
        if(elevator == null)
        {
            elevator = FindObjectOfType<ElevatorManager>();
        }
    }

    public bool Set(ItemData data)
    {
        bool result = false;

        if(data == null)    // 넘어온 데이터가 없을 때 바로 false리턴
        {
            UIManager.Inst.SetMessagePanel("아이템이 선택되지 않았습니다");
            return result;
        }

        if(data.id == (uint)ItemIDCode.Fuse)    // fuse 아이템 넣기 시도
        {
            ImportFuse();
            result = true;
        }
        else    // fuse가 아닌 아이템 넣기 시도
        {
            UIManager.Inst.SetMessagePanel("사용할 수 없는 아이템입니다");
        }
        return result;
    }

    void ImportFuse()
    {
        if(currentCount < socketCount)
        {
            Instantiate(fusePrefab, socket[currentCount]);
            currentCount++;
        }
        if(currentCount == socketCount)
        {
            UIManager.Inst.SetMessagePanel("엘리베이터 전원이 활성화되었습니다");
            elevator.isActivate = true;
            elevator.Room.CurrentFloor = 1; // 엘리베이터 스크린에 숫자 1 띄우기 위함
        }
    }
}
