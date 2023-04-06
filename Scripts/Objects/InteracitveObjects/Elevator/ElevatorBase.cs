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
    /// 패널의 버튼(위아래 딱히 의미 없음)
    /// </summary>
    ElevatorButton buttonUp, buttonDown;

    /// <summary>
    /// 패널 스크린의 렌더러
    /// </summary>
    Renderer screen_Direction, screen_Number;

    /// <summary>
    /// 패널 부모 트랜스폼 (버튼, 스크린 찾기 위함)
    /// </summary>
    Transform panelParent;

    /// <summary>
    /// 해당 층의 목적지 (ElevatorRoom 이동 목적지)
    /// </summary>
    public Transform destinationPos;

    // 스크립트에서 머테리얼 미리 넣어주기 위해 배열 따로 만듬(스크립트에서는 배열에 직접 머테리얼 못 넣음)
    Material[] screenNum;

    Coroutine buttonCoroutine;

    // 유니티 이벤트 함수 ------------------------------------------------------------------------------------------

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

        screen_Direction.material = manager.screenBlack;    // ScreenDirection 초기 머테리얼은 아무것도 안보이도록
        screen_Number.material = manager.screenBlack;      // ScreenNumber 초기 머테리얼도 아무것도 안보이도록

        buttonUp.onButtonClick += CallElevator;
        buttonDown.onButtonClick += CallElevator;

        buttonUp.onButtonClick += ButtonDelay;
        buttonDown.onButtonClick += ButtonDelay;

        // ElevatorBase 스크립트가 포함된 게임오브젝트의 이름에서 층수 확인
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

    // 일반 함수 ------------------------------------------------------------------------------------------
        
    /// <summary>
    /// 각 층 문 열기
    /// </summary>
    public void DoorOpen()
    {
        anim.ResetTrigger("Close");
        anim.SetTrigger("Open");
    }

    /// <summary>
    /// 각 층 문 닫기
    /// </summary>
    public void DoorClose()
    {
        anim.ResetTrigger("Open");
        anim.SetTrigger("Close");
    }

    /// <summary>
    /// 엘리베이터를 해당 층으로 부르기
    /// </summary>
    void CallElevator()
    {
        room.AddCallQueue(floor);
    }

    // Panel 관련 함수 ------------------------------------------------------------------------------------------

    /// <summary>
    /// 일정 시간 후에 버튼 꺼주는 함수(엘리베이터가 해당 층에 도착했을 때만 작동)
    /// </summary>
    void ButtonDelay()
    {
        if(floor == room.CurrentFloor)
        {
            buttonCoroutine = StartCoroutine(ButtonOffDelay());
        }
    }

    /// <summary>
    /// 일정 시간 후에 버튼 꺼주는 코루틴
    /// </summary>
    IEnumerator ButtonOffDelay()
    {
        yield return new WaitForSeconds(0.5f);

        ButtonOff();
    }

    /// <summary>
    /// 모든 버튼 끄기
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
    /// 스크린에 표시되는 층수 바꾸기
    /// </summary>
    /// <param name="floor"></param>
    public void ScreenNumberChange(int floor)
    {
        screen_Number.material = screenNum[floor - 1];
    }
}
