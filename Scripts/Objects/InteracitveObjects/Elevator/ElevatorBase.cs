using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorBase : MonoBehaviour
{
    ElevatorManager manager;

    Animator anim;

    ElevatorRoom room;

    public int floor = 1;

    // Panel ------------------------------------------------------------------------------------------

    /// <summary>
    /// �г��� ��ư(���Ʒ� ���� �ǹ� ����)
    /// </summary>
    ElevatorButton buttonUp, buttonDown;

    /// <summary>
    /// �г� ��ũ���� ������
    /// </summary>
    Renderer screen_Direction, screen_Number;

    /// <summary>
    /// �г� �θ� Ʈ������ (��ư, ��ũ�� ã�� ����)
    /// </summary>
    Transform panelParent;

    /// <summary>
    /// �ش� ���� ������ (ElevatorRoom �̵� ������)
    /// </summary>
    public Transform destinationPos;

    // ��ũ��Ʈ���� ���׸��� �̸� �־��ֱ� ���� �迭 ���� ����(��ũ��Ʈ������ �迭�� ���� ���׸��� �� ����)
    Material[] screenNum;

    Coroutine buttonCoroutine;

    // ����Ƽ �̺�Ʈ �Լ� ------------------------------------------------------------------------------------------

    private void Awake()
    {
        anim = GetComponent<Animator>();

        manager = GetComponentInParent<ElevatorManager>();

        destinationPos = transform.Find("DestinationPos");
        panelParent = transform.Find("ElevatorPanel");

        buttonUp = panelParent.Find("ElevatorButtonUp").GetComponent<ElevatorButton>();
        buttonDown = panelParent.Find("ElevatorButtonDown").GetComponent<ElevatorButton>();
        screen_Direction = panelParent.Find("ElevatorScreenDirection").GetComponent<Renderer>();
        screen_Number = panelParent.Find("ElevatorScreenNumber").GetComponent<Renderer>();
    }

    private void Start()
    {
        room = manager.Room;

        screenNum = new Material[3];
        screenNum[0] = manager.screenNum1;
        screenNum[1] = manager.screenNum2;
        screenNum[2] = manager.screenNum3;

        screen_Direction.material = manager.screenBlack;    // ScreenDirection �ʱ� ���׸����� �ƹ��͵� �Ⱥ��̵���
        screen_Number.material = manager.screenBlack;      // ScreenNumber �ʱ� ���׸��� �ƹ��͵� �Ⱥ��̵���

        buttonUp.onButtonClick += CallElevator;
        buttonDown.onButtonClick += CallElevator;

        buttonUp.onButtonClick += ButtonDelay;
        buttonDown.onButtonClick += ButtonDelay;

        // ElevatorBase ��ũ��Ʈ�� ���Ե� ���ӿ�����Ʈ�� �̸����� ���� Ȯ��
        string name = transform.gameObject.name;
        if (name.Contains("1"))
        {
            floor = 1;
        }
        else if(name.Contains("2"))
        {
            floor = 2;
        }
        else
        {
            floor = 3;
        }
    }

    // �Ϲ� �Լ� ------------------------------------------------------------------------------------------
        
    /// <summary>
    /// �� �� �� ����
    /// </summary>
    public void DoorOpen()
    {
        anim.ResetTrigger("Close");
        anim.SetTrigger("Open");
    }

    /// <summary>
    /// �� �� �� �ݱ�
    /// </summary>
    public void DoorClose()
    {
        anim.ResetTrigger("Open");
        anim.SetTrigger("Close");
    }

    /// <summary>
    /// ���������͸� �ش� ������ �θ���
    /// </summary>
    void CallElevator()
    {
        room.AddCallQueue(floor);
    }

    // Panel ���� �Լ� ------------------------------------------------------------------------------------------

    /// <summary>
    /// ���� �ð� �Ŀ� ��ư ���ִ� �Լ�(���������Ͱ� �ش� ���� �������� ���� �۵�)
    /// </summary>
    void ButtonDelay()
    {
        if(floor == room.CurrentFloor)
        {
            buttonCoroutine = StartCoroutine(ButtonOffDelay());
        }
    }

    /// <summary>
    /// ���� �ð� �Ŀ� ��ư ���ִ� �ڷ�ƾ
    /// </summary>
    IEnumerator ButtonOffDelay()
    {
        yield return new WaitForSeconds(0.5f);

        ButtonOff();
    }

    /// <summary>
    /// ��� ��ư ����
    /// </summary>
    public void ButtonOff()
    {
        buttonUp.ButtonOff();
        buttonDown.ButtonOff();
    }

    public void ScreenDirectionChange(bool isMoving, ElevatorState state)
    {
        if (isMoving)
        {
            switch (state)
            {
                case ElevatorState.Up:
                    screen_Direction.material = manager.screenUpMoving;
                    break;
                case ElevatorState.Down:
                    screen_Direction.material = manager.screenDownMoving;
                    break;
                case ElevatorState.Stop:
                default:
                    screen_Direction.material = manager.screenBlack;
                    break;
            }
        }
        else
        {
            switch (state)
            {
                case ElevatorState.Up:
                    screen_Direction.material = manager.screenUp;
                    break;
                case ElevatorState.Down:
                    screen_Direction.material = manager.screenDown;
                    break;
                case ElevatorState.Stop:
                default:
                    screen_Direction.material = manager.screenBlack;
                    break;
            }
        }
    }

    /// <summary>
    /// ��ũ���� ǥ�õǴ� ���� �ٲٱ�
    /// </summary>
    /// <param name="floor"></param>
    public void ScreenNumberChange(int floor)
    {
        screen_Number.material = screenNum[floor - 1];
    }
}
