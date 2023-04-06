using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player_Opening : MonoBehaviour
{
    PlayerInputActions action;
    Rigidbody rigid;
    Transform playerCamera;
    Animator anim;
    Transform characterPrefabTransform;
    CinemachineVirtualCamera virtualCamera;

    // 플레이어 용 ------------------------------------------------------------------------------------------------

    bool isRun = false;
    bool isMove = false;


    const uint InvalidID = uint.MaxValue;       // 내 아이템 선택 해제할 때

    uint mySlotID = InvalidID;    // InvalidID 이면 현재 플레이어의 슬롯이 비었다.


    PlayerState state = PlayerState.Stand;

    public PlayerState State { get => state; }


    Coroutine sitCoroutine;

    // 이동, 회전 용 ------------------------------------------------------------------------------------------------

    Vector3 moveDir = Vector3.zero;     // 이동 방향
    Vector2 mouseDelta = Vector2.zero;  // 마우스 움직인 방향

    public float moveSpeed = 10.0f;  // 플레이어 움직임 속도
    public float turnSpeed = 10.0f; // 시점이동 속도(마우스 감도)

    private float xRotate = 0.0f;
    private float yRotate = 0.0f;

    Vector3 initialCamPos;  // 플레이어 앉고 일어설 때 카메라 위치 변경할 때 사용하는 카메라 로컬 위치의 초기값

    // 상호작용 용 ----------------------------------------------------------------------------------------------------

    bool canInteraction = false;    // 상호작용 가능한지 여부(마우스 에임에 아이템 등이 있을 때 true)

    delegate bool InteractDelegate(ItemData data);  // 상호작용 용 델리게이트

    InteractDelegate interact;

    IUsableObject iUse;
    Ray ray;    // 상호작용 ray

    float rayRange = 7.0f;  // 상호작용 가능 거리


    // 유니티 이벤트 함수 -------------------------------------------------------------------------------------------------

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        action = new PlayerInputActions();
        anim = GetComponentInChildren<Animator>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        playerCamera = virtualCamera.transform;

        initialCamPos = playerCamera.localPosition; // 초기 카메라 로컬 위치값

        characterPrefabTransform = transform.GetChild(1);   // 캐릭터 실제 모습 보여줄 게임오브젝트 가져오기


        ray = new Ray(playerCamera.position, playerCamera.forward); // 플레이어 정면 방향으로 Ray 생성
    }

    /// <summary>
    /// 애니메이션 재생 등으로 캐릭터 프리팹의 회전이 틀어지는 것을 보완하기 위해 1초마다 초기 로컬 회전값으로 조정
    /// </summary>
    /// <returns></returns>
    IEnumerator InitializePrefabTransform()
    {
        while (true)
        {
            characterPrefabTransform.localEulerAngles = Vector3.zero;
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        UIManager_Opening.Inst.OnMenuOpen += action.Player.Disable;            // 메뉴 열 때 이동 및 시점 이동 막기
        UIManager_Opening.Inst.OnMenuClose += action.Player.Enable;
        StartCoroutine(InitializePrefabTransform());
    }

    private void OnEnable()
    {
        action.Player.Enable();

        action.Player.Move.performed += onMove;                 // WASD 이동
        action.Player.Move.canceled += onMove;              
        action.Player.Mouse.performed += onMouse;               // 마우스로 시점 이동(회전)
        action.Player.Mouse.canceled += onMouse;
        action.Player.Interaction.performed += onInteraction;   // F키 상호작용
        action.Player.Run.performed += onRun;                   // Shift 달리기
        action.Player.Run.canceled += onRun;
        action.Player.Sit.performed += onSit;                   // Ctrl 앉기
        action.Player.Sit.canceled += onSit;
    }

    private void OnDisable()
    {
        action.Player.Sit.canceled -= onSit;
        action.Player.Sit.performed -= onSit;
        action.Player.Run.canceled -= onRun;
        action.Player.Run.performed -= onRun;
        action.Player.Interaction.performed -= onInteraction;
        action.Player.Mouse.canceled -= onMouse;
        action.Player.Mouse.performed -= onMouse;
        action.Player.Move.canceled -= onMove;
        action.Player.Move.performed -= onMove;

        action.Player.Disable();
    }

    void SitUpdate()
    {
        Move(moveSpeed * 0.5f); 
        if (isMove && isRun)
        {
            ChangeState(PlayerState.Run);
        }
    }
    void StandUpdate()
    {
        Move(moveSpeed);
        if(isMove && isRun)
        {
            ChangeState(PlayerState.Run);
        }
    }
    void RunUpdate()
    {
        Move(moveSpeed * 1.5f);
        if(!isRun)
        {
            ChangeState(PlayerState.Stand);
        }
    }

    void Move(float speed)
    {
        rigid.MovePosition(transform.position + transform.rotation * moveDir * Time.fixedDeltaTime * speed);    // 이동
    }

    private void Update()
    {
        Rotate();   // 회전
        RayTarget();    // 앞에 있는 아이템 인식 
    }

    private void FixedUpdate()
    {
        //Rotate();   // 회전


        switch (state)
        {
            case PlayerState.Sit:
                SitUpdate();
                break;
            case PlayerState.Stand:
                StandUpdate();
                break;
            case PlayerState.Run:
                RunUpdate();
                break;
            default:
                break;
        }
    }

    // 인풋 관련 함수 ----------------------------------------------------------------------------------------------------

    private void onInteraction(InputAction.CallbackContext context) // F키 눌렀을 때 상호작용
    {
        if (canInteraction)
        {
            bool result = false;
            if (interact != null)
            {
                result = interact.Invoke(null);

            }
        }
    }
    private void onMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
        moveDir = new Vector3(moveDir.x, 0, moveDir.y);
        anim.SetFloat("DirX", moveDir.x);
        anim.SetFloat("DirY", moveDir.z);
        if (moveDir.magnitude > 0)
        {
            anim.SetBool("isMove", true);
            isMove = true;
        }
        else
        {
            anim.SetBool("isMove", false);
            isMove = false;
        }
    }
    private void onMouse(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }
    private void onSit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ChangeState(PlayerState.Sit);
        }
        else
        {
            ChangeState(PlayerState.Stand);
        }
    }
    private void onRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }
    }


    /// <summary>
    /// 플레이어 및 메인카메라(화면) 회전
    /// </summary>
    private void Rotate()
    {
        playerCamera.eulerAngles = new Vector3(xRotate, playerCamera.eulerAngles.y, playerCamera.eulerAngles.z);    // 카메라 x축만 회전
        xRotate = Mathf.Clamp(xRotate - mouseDelta.y * turnSpeed * Time.fixedDeltaTime, -80, 80);    // 카메라의 x축 회전(상하 시점 이동), 상하 80도 제한
        
        yRotate = transform.eulerAngles.y + mouseDelta.x * turnSpeed * Time.fixedDeltaTime;  // 플레이어의 y축 회전(좌우 시점 이동)
        transform.eulerAngles = new Vector3(0, yRotate, 0); // 플레이어 y축 회전
    }

    /// <summary>
    /// PlayerState 변경 함수
    /// </summary>
    /// <param name="newState"></param>
    void ChangeState(PlayerState newState)
    {
        // 이전 상태를 나가면서 해야 할 일들
        switch (state)
        {
            case PlayerState.Sit:
                anim.SetTrigger("Stand");
                if (sitCoroutine != null)
                {
                    StopCoroutine(sitCoroutine);
                }
                sitCoroutine = StartCoroutine(SitStandChange(false));
                break;
            case PlayerState.Stand:
                break;
            case PlayerState.Run:
                anim.SetTrigger("Stand");
                break;
            default:
                break;
        }

        // 새 상태로 들어가면서 해야 할 일들
        switch (newState)
        {
            case PlayerState.Sit:
                anim.SetTrigger("Sit");
                if (sitCoroutine != null)
                {
                    StopCoroutine(sitCoroutine);
                }
                sitCoroutine = StartCoroutine(SitStandChange(true));
                break;
            case PlayerState.Stand:
                anim.SetTrigger("Stand");
                break;
            case PlayerState.Run:
                anim.SetTrigger("Run");
                break;
            default:
                break;
        }

        state = newState;
        //anim.SetInteger("EnemyState", (int)state);
    }

    /// <summary>
    /// 앉고 설 때 카메라 위치 변경해주는 코루틴
    /// </summary>
    /// <param name="isSitting">앉을 때 true</param>
    /// <returns></returns>
    IEnumerator SitStandChange(bool isSitting)
    {
        Vector3 pos;
        if (isSitting)
        {
            while (Mathf.Abs(playerCamera.localPosition.y - (initialCamPos.y - 0.7f)) > 0.01f)
            {
                pos = new Vector3(playerCamera.localPosition.x, initialCamPos.y - 0.7f, playerCamera.localPosition.z);
                playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, pos, Time.deltaTime * 10.0f);
                yield return null;
            }
        }
        else
        {
            while (Mathf.Abs(playerCamera.localPosition.y - initialCamPos.y) > 0.01f)
            {
                pos = new Vector3(playerCamera.localPosition.x, initialCamPos.y, playerCamera.localPosition.z);
                playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, pos, Time.deltaTime * 10.0f);
                yield return null;
            }
        }
    }

    /// <summary>
    /// Ray를 이용해 카메라 방향의 아이템 인식
    /// </summary>
    private void RayTarget()
    {
        ray.origin = playerCamera.position;
        ray.direction = playerCamera.transform.forward;
        // rayRange 거리 안에서 Item 레이어를 가지고 있는 타겟에 ray가 hit 했을 때
        if (Physics.Raycast(ray, out RaycastHit hit, rayRange, LayerMask.GetMask("Interactive")))
        {
            if (iUse == null)    // 상호작용 가능한 오브젝트가 아무것도 없을 때
            {
                iUse = hit.collider.GetComponent<IUsableObject>();
            } 

            if(iUse != null)   // hit된 collider가 IUsableObject를 가지고 있으면
            {
                if(hit.collider.CompareTag("Equipment"))
                {
                    UIManager_Opening.Inst.SetPopupPanel(true, "F - 선택");
                    interact = iUse.Use;
                    canInteraction = true;
                }
                else
                {
                    UIManager_Opening.Inst.SetPopupPanel(true, "F - 타기");
                    interact = iUse.Use;
                    canInteraction = true;
                }
            }
            else
            {
                NoInteraction();
            }
        }
        else
        {
            NoInteraction();
        }
    }

    /// <summary>
    /// 물체와 상호작용이 없을 때 UI 끄는 등의 작업 함수
    /// </summary>
    void NoInteraction()
    {
        UIManager_Opening.Inst.SetPopupPanel(false);
        iUse = null;
        interact = null;
        canInteraction = false;
    }
}
