using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorRoom : MonoBehaviour
{
    ElevatorManager manager;

    /// <summary>
    /// 현재 층수
    /// </summary>
    int currentFloor = 1;

    /// <summary>
    /// 엘리베이터 움직이는 속도
    /// </summary>
    public float moveSpeed = 3.0f;

    /// <summary>
    /// 문이 저절로 닫힐 때 까지의 시간
    /// </summary>
    const float DoorCoolTime = 3.0f;

    /// <summary>
    /// 문 닫히는 시간 카운터
    /// </summary>
    float doorCoolTimeCounter = DoorCoolTime;

    /// <summary>
    /// 엘리베이터가 움직이고 있으면 true
    /// </summary>
    bool isMoving = false;

    /// <summary>
    /// 엘리베이터 문이 열려있으면 true
    /// </summary>
    bool isOpen = false;

    ElevatorState state = ElevatorState.Stop;

    Animator anim;

    ElevatorBase[] bases;

    // 프로퍼티 -------------------------------------------------------------------------------------------

    public int CurrentFloor
    {
        get => currentFloor;
        set
        {
            currentFloor = value;
            foreach (var elevatorBase in bases)
            {
                elevatorBase.ScreenNumberChange(currentFloor);
                ScreenNumberChange(currentFloor);
            }
        }
    }

    bool IsMoving
    {
        get => isMoving;
        set
        {
            isMoving = value;
            if(!isMoving)
            {
                if(callQueue.Count > 0)
                {
                    if (callQueue[0] > currentFloor)
                    {
                        State = ElevatorState.Up;
                    }
                    else
                    {
                        State = ElevatorState.Down;
                    }
                }
                else
                {
                    State = ElevatorState.Stop;
                }
            }
            else
            {
                if (callQueue.Count > 0)
                {
                    if (callQueue[0] > currentFloor)
                    {
                        State = ElevatorState.Up;
                    }
                    else
                    {
                        State = ElevatorState.Down;
                    }
                }
                else
                {
                    State = ElevatorState.Stop;
                }
            }
        }
    }

    ElevatorState State
    {
        get => state;
        set
        {
            state = value;
            ScreenDirectionChange(IsMoving, state);
            foreach(var elevatorBase in bases)
            {
                elevatorBase.ScreenDirectionChange(IsMoving, state);
            }
        }
    }

    // Panel 관련 -------------------------------------------------------------------------------------------

    /// <summary>
    /// 패널 부모 트랜스폼 (버튼, 스크린 찾기 위함)
    /// </summary>
    Transform panelParent1, panelParent2;

    /// <summary>
    /// 패널 스크린의 렌더러
    /// </summary>
    Renderer screen_Direction_1, screen_Number_1, screen_Direction_2, screen_Number_2;

    /// <summary>
    /// 패널의 버튼(위아래 딱히 의미 없음)
    /// </summary>
    ElevatorButton button1_1, button2_1, button3_1, buttonOpen_1, buttonClose_1;
    ElevatorButton button1_2, button2_2, button3_2, buttonOpen_2, buttonClose_2;

    /// <summary>
    /// 엘리베이터 호출 순서 큐
    /// </summary>
    List<int> callQueue;

    /// <summary>
    /// 문 열고 닫는 버튼 저절로 꺼지게 하는 코루틴
    /// </summary>
    Coroutine doorButtonCoroutine;

    /// <summary>
    /// 문이 완전히 닫혔을 때 isOpen = false 로 만들어주는 코루틴
    /// </summary>
    Coroutine doorCloseCoroutine;

    /// <summary>
    /// 엘리베이터 이동 구현 코루틴
    /// </summary>
    Coroutine moveCoroutine;

    Material[] screenNum;


    // 유니티 이벤트 함수 -------------------------------------------------------------------------------------------

    private void Awake()
    {
        anim = GetComponent<Animator>();

        manager = GetComponentInParent<ElevatorManager>();

        panelParent1 = transform.Find("ElevatorPanel1");

        button1_1 = panelParent1.Find("ElevatorButton1").GetComponent<ElevatorButton>();
        button2_1 = panelParent1.Find("ElevatorButton2").GetComponent<ElevatorButton>();
        button3_1 = panelParent1.Find("ElevatorButton3").GetComponent<ElevatorButton>();
        buttonOpen_1 = panelParent1.Find("ElevatorButtonOpen").GetComponent<ElevatorButton>();
        buttonClose_1 = panelParent1.Find("ElevatorButtonClose").GetComponent<ElevatorButton>();
        screen_Direction_1 = panelParent1.Find("ElevatorScreenDirection").GetComponent<Renderer>();
        screen_Number_1 = panelParent1.Find("ElevatorScreenNumber").GetComponent<Renderer>();

        panelParent2 = transform.Find("ElevatorPanel2");

        button1_2 = panelParent2.Find("ElevatorButton1").GetComponent<ElevatorButton>();
        button2_2 = panelParent2.Find("ElevatorButton2").GetComponent<ElevatorButton>();
        button3_2 = panelParent2.Find("ElevatorButton3").GetComponent<ElevatorButton>();
        buttonOpen_2 = panelParent2.Find("ElevatorButtonOpen").GetComponent<ElevatorButton>();
        buttonClose_2 = panelParent2.Find("ElevatorButtonClose").GetComponent<ElevatorButton>();
        screen_Direction_2 = panelParent2.Find("ElevatorScreenDirection").GetComponent<Renderer>();
        screen_Number_2 = panelParent2.Find("ElevatorScreenNumber").GetComponent<Renderer>();

        // 층수 버튼을 누르면 호출 큐에 해당 층 추가
        button1_1.onButtonClick += () => AddCallQueue(1);
        button1_2.onButtonClick += () => AddCallQueue(1);
        button2_1.onButtonClick += () => AddCallQueue(2);
        button2_2.onButtonClick += () => AddCallQueue(2);
        button3_1.onButtonClick += () => AddCallQueue(3);
        button3_2.onButtonClick += () => AddCallQueue(3);

        // 층수 버튼을 누르면 다른 쪽 층수 버튼도 켜지도록 하기
        button1_1.onButtonClick += button1_2.ButtonOn;
        button1_2.onButtonClick += button1_1.ButtonOn;
        button2_1.onButtonClick += button2_2.ButtonOn;
        button2_2.onButtonClick += button2_1.ButtonOn;
        button3_1.onButtonClick += button3_2.ButtonOn;
        button3_2.onButtonClick += button3_1.ButtonOn;

        // 문 열고 닫는 버튼에 문 열고 닫는 함수와 일정 시간이 지나면 저절로 꺼지는 함수 추가
        buttonOpen_1.onButtonClick += DoorOpen;
        buttonOpen_1.onButtonClick += () => ButtonDelay(true);
        buttonOpen_2.onButtonClick += DoorOpen;
        buttonOpen_2.onButtonClick += () => ButtonDelay(true);
        buttonClose_1.onButtonClick += DoorClose;
        buttonClose_1.onButtonClick += () => ButtonDelay(false);
        buttonClose_2.onButtonClick += DoorClose;
        buttonClose_2.onButtonClick += () => ButtonDelay(false);
    }

    private void Start()
    {
        bases = manager.ElevatorBases;

        callQueue = new List<int>(3);

        screenNum = new Material[3];
        screenNum[0] = manager.screenNum1;
        screenNum[1] = manager.screenNum2;
        screenNum[2] = manager.screenNum3;

        screen_Direction_1.material = manager.screenBlack;    // ScreenDirection 초기 머테리얼은 아무것도 안보이도록
        screen_Direction_2.material = manager.screenBlack;    
        screen_Number_1.material = screenNum[0];      // ScreenNumber 초기 머테리얼은 1층으로
        screen_Number_2.material = screenNum[0];      
    }

    private void Update()
    {
        if (isOpen)
        {
            doorCoolTimeCounter -= Time.deltaTime;
            if (doorCoolTimeCounter < 0)
            {
                DoorClose();
            }
        }
        if (callQueue.Count > 0 && !IsMoving && !isOpen)
        {
            if (callQueue.Contains(CurrentFloor))
            {
                callQueue.Remove(CurrentFloor);
                OnElevatorArrive();
            }
            else
            {
                ChangeFloor(callQueue[0]);
                callQueue.RemoveAt(0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ElevatorBase elevator;

        elevator = other.GetComponentInParent<ElevatorBase>();

        if (elevator != null)
        {
            CurrentFloor = elevator.floor;
        }
    }

    // 이동 관련 함수 -------------------------------------------------------------------------------------------

    /// <summary>
    /// ElevatorRoom의 실제 물리적 위치 변경용 함수
    /// </summary>
    /// <param name="destination"></param>
    void ChangeFloor(Transform destination)
    {
        if (!IsMoving)
        {
            if(moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(MoveElevator(destination));
        }
    }

    void ChangeFloor(int floor)
    {
        ChangeFloor(bases[floor - 1].destinationPos);
    }

    public void AddCallQueue(int floor)
    {
        if (!callQueue.Contains(floor))
        {
            callQueue.Add(floor);
        }
    }

    void OnElevatorArrive()
    {
        DoorOpen();
        NumberButtonOff(CurrentFloor);
        foreach(var elevatorBase in bases)
        {
            elevatorBase.ButtonOff();
        }
        IsMoving = false;
    }

    // 패널 관련 함수 -------------------------------------------------------------------------------------------
    private void ScreenDirectionChange(bool isMoving, ElevatorState state)
    {
        if (isMoving)
        {
            switch (state)
            {
                case ElevatorState.Up:
                    screen_Direction_1.material = manager.screenUpMoving;
                    screen_Direction_2.material = manager.screenUpMoving;
                    break;
                case ElevatorState.Down:
                    screen_Direction_1.material = manager.screenDownMoving;
                    screen_Direction_2.material = manager.screenDownMoving;
                    break;
                case ElevatorState.Stop:
                default:
                    screen_Direction_1.material = manager.screenBlack;
                    screen_Direction_2.material = manager.screenBlack;
                    break;
            }
        }
        else
        {
            switch (state)
            {
                case ElevatorState.Up:
                    screen_Direction_1.material = manager.screenUp;
                    screen_Direction_2.material = manager.screenUp;
                    break;
                case ElevatorState.Down:
                    screen_Direction_1.material = manager.screenDown;
                    screen_Direction_2.material = manager.screenDown;
                    break;
                case ElevatorState.Stop:
                default:
                    screen_Direction_1.material = manager.screenBlack;
                    screen_Direction_2.material = manager.screenBlack;
                    break;
            }
        }
    }

    private void ScreenNumberChange(int floor)
    {
        screen_Number_1.material = screenNum[floor - 1];
        screen_Number_2.material = screenNum[floor - 1];
    }

    void NumberButtonOff(int number)
    {
        switch (number)
        {
            case 1:
                button1_1.ButtonOff();
                button1_2.ButtonOff();
                break;
            case 2:
                button2_1.ButtonOff();
                button2_2.ButtonOff();
                break;
            case 3:
                button3_1.ButtonOff();
                button3_2.ButtonOff();
                break;
            default:
                button1_1.ButtonOff();
                button1_2.ButtonOff();
                button2_1.ButtonOff();
                button2_2.ButtonOff();
                button3_1.ButtonOff();
                button3_2.ButtonOff();
                break;
        }
    }

    /// <summary>
    /// 일정 시간 후에 버튼을 꺼주는 함수(문 열고 닫는 버튼)
    /// </summary>
    /// <param name="isOpenButton">true면 여는 버튼 조작, false면 끄는 버튼 조작</param>
    public void ButtonDelay(bool isOpenButton)
    {
        doorButtonCoroutine = StartCoroutine(ButtonOffDelay(isOpenButton));
    }

    // 일반 함수 -------------------------------------------------------------------------------------------


    public void DoorOpen()
    {
        if(doorCloseCoroutine != null)
        {
            StopCoroutine(doorCloseCoroutine);
        }
        isOpen = true;
        bases[currentFloor - 1].DoorOpen();
        anim.ResetTrigger("Close");
        anim.SetTrigger("Open");
        doorCoolTimeCounter = DoorCoolTime;
    }

    public void DoorClose()
    {
        doorCoolTimeCounter = float.MaxValue;
        bases[currentFloor - 1].DoorClose();
        anim.ResetTrigger("Open");
        anim.SetTrigger("Close");
        if (doorCloseCoroutine != null)
        {
            StopCoroutine(doorCloseCoroutine);
        }
        doorCloseCoroutine = StartCoroutine(CloseDelay());
    }

    // 코루틴 -------------------------------------------------------------------------------------------

    IEnumerator MoveElevator(Transform destination)
    {
        IsMoving = true;
        while (Vector3.Distance(transform.position, destination.position) > 0.01f)
        {
            Vector3 dir = destination.position - transform.position;
            transform.position += Time.deltaTime * moveSpeed * dir.normalized;
            yield return null;
        }
        OnElevatorArrive();
        yield return null;
    }

    /// <summary>
    /// 일정 시간 후에 버튼을 꺼주는 코루틴(문 열고 닫는 버튼)
    /// </summary>
    /// <param name="isOpenButton">true면 여는 버튼 조작, false면 끄는 버튼 조작</param>
    IEnumerator ButtonOffDelay(bool isOpenButton)
    {
        yield return new WaitForSeconds(0.5f);

        if (isOpenButton)
        {
            buttonOpen_1.ButtonOff();
            buttonOpen_2.ButtonOff();
        }
        else
        {
            buttonClose_1.ButtonOff();
            buttonClose_2.ButtonOff();
        }
    }

    IEnumerator CloseDelay()
    {
        yield return new WaitForSeconds(3.0f);

        isOpen = false;
    }

}
