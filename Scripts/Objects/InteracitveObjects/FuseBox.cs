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

        if(data == null)    // �Ѿ�� �����Ͱ� ���� �� �ٷ� false����
        {
            UIManager.Inst.SetMessagePanel("�������� ���õ��� �ʾҽ��ϴ�");
            return result;
        }

        if(data.id == (uint)ItemIDCode.Fuse)    // fuse ������ �ֱ� �õ�
        {
            ImportFuse();
            result = true;
        }
        else    // fuse�� �ƴ� ������ �ֱ� �õ�
        {
            UIManager.Inst.SetMessagePanel("����� �� ���� �������Դϴ�");
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
            UIManager.Inst.SetMessagePanel("���������� ������ Ȱ��ȭ�Ǿ����ϴ�");
            elevator.isActivate = true;
            elevator.Room.CurrentFloor = 1; // ���������� ��ũ���� ���� 1 ���� ����
        }
    }
}
