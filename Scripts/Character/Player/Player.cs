using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    PlayerInputActions action;
    Rigidbody rigid;
    Transform playerCamera;
    Inventory inven;
    Animator anim;

    //Enemy[] enemies;

    Transform characterPrefabTransform;

    [SerializeField]
    Transform flashLightTransform;

    Light flashLight;

    Vector3 lightInitialRotation;

    // 플레이어 용 ------------------------------------------------------------------------------------------------

    bool isRun = false;
    bool isMove = false;

    bool isOnWood = false;

    const float MaxHP = 100;
    float hp = 100;

    ItemData myItemData = null;

    public ItemData MyItemData => myItemData;

    const uint InvalidID = uint.MaxValue;       // 내 아이템 선택 해제할 때

    uint mySlotID = InvalidID;    // InvalidID 이면 현재 플레이어의 슬롯이 비었다.

    public uint MySlotID
    {
        get => mySlotID;
        set
        {
            uint oldSlotNum = mySlotID;
            mySlotID = value;
            if (mySlotID < inven.SlotCount)
            {
                ItemData temp = inven[(int)mySlotID].SlotItemData;
                if (temp != null)
                {
                    if (myItemData == null || mySlotID != oldSlotNum)
                    {
                        UIManager.Inst.SetShortCutUI((int)mySlotID);
                        myItemData = temp;
                    }
                    else
                    {
                        UIManager.Inst.SetShortCutUI(-1);   // 0, 1, 2 이외의 값을 보내면 ShortCut 전부 비선택상태
                        mySlotID = InvalidID;
                        myItemData = null;
                    }
                }
                else
                {
                    UIManager.Inst.SetShortCutUI(-1);
                    mySlotID = InvalidID;
                    myItemData = null;
                }
            }
            else
            {
                UIManager.Inst.SetShortCutUI(-1);
                mySlotID = InvalidID;
                myItemData = null;
            }
        }
    }

    public float HP
    {
        get => hp;
        set
        {
            if(value < 0)
            {
                Die();
            }
            hp = Mathf.Clamp(value, 0, MaxHP);
            vig.intensity.value = 1 - (hp / MaxHP);
        }
    }

    public bool IsOnWood
    {
        get => isOnWood;
        set
        {
            isOnWood = value;
            PlayFootstepSFX();
        }
    }

    PlayerState state = PlayerState.Stand;

    public PlayerState State { get => state; }


    Coroutine sitCoroutine;

    // 이동, 회전 용 ------------------------------------------------------------------------------------------------

    Vector3 moveDir = Vector3.zero;     // 이동 방향
    Vector2 mouseDelta = Vector2.zero;  // 마우스 움직인 방향

    public float moveSpeed = 10.0f;  // 플레이어 움직임 속도
    public float turnSpeed = 10.0f; // 시점이동 속도(마우스 감도) 테스트를 위해 public 선언, 스크립트에선 프로퍼티로 사용하기

    public float TurnSpeed
    {
        get
        {
            return turnSpeed;
        }
        set
        {
            turnSpeed = Mathf.Clamp(value, 0.01f, 100.0f);
        }
    }

    private float xRotate = 0.0f;
    private float yRotate = 0.0f;

    Vector3 initialCamPos;  // 플레이어 앉고 일어설 때 카메라 위치 변경할 때 사용하는 카메라 로컬 위치의 초기값

    // 상호작용 용 ----------------------------------------------------------------------------------------------------

    bool canInteraction = false;    // 상호작용 가능한지 여부(마우스 에임에 아이템 등이 있을 때 true)

    delegate bool InteractDelegate(ItemData data);  // 상호작용 용 델리게이트

    InteractDelegate interact;
    Item rayItem;      // ray 용 item

    
    ISettableObject iSet;
    IUsableObject iUse;
    Ray ray;    // 상호작용 ray

    float rayRange = 2.5f;  // 상호작용 가능 거리

    Action<Inventory> getItem;

    // 포스트 프로세싱 ------------------------------------------------------------------------------------------------

    Volume volume;
    LiftGammaGain lgg;
    Vignette vig;
    LensDistortion lens;

    // 유니티 이벤트 함수 -------------------------------------------------------------------------------------------------

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        action = new PlayerInputActions();
        anim = GetComponentInChildren<Animator>();

        flashLight = flashLightTransform.GetComponentInChildren<Light>();
        lightInitialRotation = flashLightTransform.localRotation.eulerAngles;   // 플래쉬 초기 localRotation 값        

        playerCamera = Camera.main.transform;

        initialCamPos = playerCamera.localPosition; // 초기 카메라 로컬 위치값

        characterPrefabTransform = transform.GetChild(1);   // 캐릭터 실제 모습 보여줄 게임오브젝트 가져오기


        ray = new Ray(playerCamera.position, playerCamera.forward); // 플레이어 정면 방향으로 Ray 생성

        inven = new Inventory();    // 플레이어 인벤토리 생성

        //enemies = FindObjectsOfType<Enemy>();
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
        GameManager.Inst.InvenUI.InitializeInventory(inven);
        GameManager.Inst.InvenUI.OnInventoryOpen += action.Player.Disable;  // 인벤토리 열 때 이동 및 시점 이동 막기
        GameManager.Inst.InvenUI.OnInventoryClose += action.Player.Enable;
        UIManager.Inst.OnMenuOpen += action.Player.Disable;            // 메뉴 열 때 이동 및 시점 이동 막기
        UIManager.Inst.OnMenuClose += action.Player.Enable;

        volume = FindObjectOfType<Volume>();
        volume.profile.TryGet(out lgg);
        volume.profile.TryGet(out vig);
        volume.profile.TryGet(out lens);

        StartCoroutine(InitializePrefabTransform());
    }

    private void OnEnable()
    {
        action.Player.Enable();
        action.ShortCut.Enable();

        action.Player.Move.performed += onMove;                 // WASD 이동
        action.Player.Move.canceled += onMove;              
        action.Player.Mouse.performed += onMouse;               // 마우스로 시점 이동(회전)
        action.Player.Mouse.canceled += onMouse;
        action.Player.Interaction.performed += onInteraction;   // F키 상호작용
        action.Player.Run.performed += onRun;                   // Shift 달리기
        action.Player.Run.canceled += onRun;
        action.Player.Sit.performed += onSit;                   // Ctrl 앉기
        action.Player.Sit.canceled += onSit;
                
        action.ShortCut.ShortCut1.performed += OnShortCut1;
        action.ShortCut.ShortCut2.performed += OnShortCut2;
        action.ShortCut.ShortCut3.performed += OnShortCut3;
        action.ShortCut.FlashLight.performed += OnFlashLight;   // R 키로 플래쉬 밝기 조절
    }

    private void OnDisable()
    {
        action.ShortCut.FlashLight.performed -= OnFlashLight;
        action.ShortCut.ShortCut3.performed -= OnShortCut3;
        action.ShortCut.ShortCut2.performed -= OnShortCut2;
        action.ShortCut.ShortCut1.performed -= OnShortCut1;

        action.Player.Sit.canceled -= onSit;
        action.Player.Sit.performed -= onSit;
        action.Player.Run.canceled -= onRun;
        action.Player.Run.performed -= onRun;
        action.Player.Interaction.performed -= onInteraction;
        action.Player.Mouse.canceled -= onMouse;
        action.Player.Mouse.performed -= onMouse;
        action.Player.Move.canceled -= onMove;
        action.Player.Move.performed -= onMove;

        action.ShortCut.Disable();
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
        FlashRotate();
        if(HP < MaxHP)
        {
            HP += Time.deltaTime * 3.0f;
        }
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
            if(rayItem != null)
            {
                getItem?.Invoke(inven);
            }
            else
            {
                bool result = false;
                if (interact != null)
                {
                    result = interact.Invoke(MyItemData);

                }
                if(result)
                {
                    inven[(int)MySlotID].ClearSlotItem();
                }
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
            //anim.SetBool("isMove", true);
            isMove = true;
        }
        else
        {
            AudioManager.Inst.StopAllSFX();
            //anim.SetBool("isMove", false);
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
    private void OnShortCut1(InputAction.CallbackContext _)
    {
        MySlotID = 0;
    }
    private void OnShortCut2(InputAction.CallbackContext _)
    {
        MySlotID = 1;
    }
    private void OnShortCut3(InputAction.CallbackContext _)
    {
        MySlotID = 2;
    }
    private void OnFlashLight(InputAction.CallbackContext _)    
    {
        // R키 누를때마다 플래시 밝기 변화 ( intensity를 0, 2, 4, 8의 4단계로 조정)
        // 플래시 밝기에 따라 적 인식거리 변경
        if (flashLight.intensity <= 0) // 플래시 0 -> 2
        {
            flashLight.intensity = 2;
            //foreach(var enemy in enemies)
            {
                Enemy.sightRange_Chase = 2.5f;
                Enemy.sightRange_Suspect = 5.0f;
            }
        }
        else if (flashLight.intensity >= 8) // 플래시 8 -> 0
        {
            flashLight.intensity = 0;
            //foreach (var enemy in enemies)
            {
                Enemy.sightRange_Chase = 1.0f;
                Enemy.sightRange_Suspect = 2.5f;
            }
        }
        else                                // 플래시 2-> 4 -> 8
        {
            flashLight.intensity *= 2;
            //foreach (var enemy in enemies)
            {
                Enemy.sightRange_Chase = flashLight.intensity;
                Enemy.sightRange_Suspect = flashLight.intensity * 2.0f;
            }
        }
        // 포스트 프로세싱으로도 밝기 조절
        lgg.gain.value = new Vector4(1, 1, 1, flashLight.intensity * 0.1f - 0.5f);
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
    /// 플래쉬 회전 함수, 상하 시점 이동값에 따라 일정 범위 내에서 플래쉬도 상하로 움직이도록 함
    /// </summary>
    void FlashRotate()
    {
        // 상하 시점 이동값 xRotate의 범위가 -80 ~ 80 이므로 80을 더한 후 160으로 나눠 0 ~ 1 사이의 값으로 만들어서
        // Mathf.Lerp 함수를 이용해 초기 플래쉬 회전각도에서 -40 ~ +40 의 회전값 사이의 값으로 전환
        float flashRotation = Mathf.Lerp(lightInitialRotation.x - 40.0f, lightInitialRotation.x + 40.0f, (xRotate + 80.0f) / 160.0f);
        flashLightTransform.localRotation = Quaternion.Euler(flashRotation, lightInitialRotation.y, lightInitialRotation.z);
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
        if(isMove)
        {
            PlayFootstepSFX();
        }
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
        if (Physics.Raycast(ray, out RaycastHit hit, rayRange, LayerMask.GetMask("Item", "Interactive")))
        {
            if (rayItem == null && iSet == null && iUse == null)    // 상호작용 가능한 오브젝트가 아무것도 없을 때
            {
                rayItem = hit.collider.GetComponentInParent<Item>();
                iSet = hit.collider.GetComponent<ISettableObject>();
                iUse = hit.collider.GetComponent<IUsableObject>();
            } 

            if (rayItem != null)    // hit 된 collider가 Item을 가지고 있으면
            {
                UIManager.Inst.SetPopupPanel(true);
                getItem = rayItem.GetItem;
                canInteraction = true;
            }
            else if(iSet != null && myItemData != null) // hit 된 collider가 ISettableObject를 가지고 있으면
            {
                UIManager.Inst.SetPopupPanel(true, "F - 넣기");
                interact = iSet.Set;
                canInteraction = true;
                
            }
            else if(iUse != null)   // hit된 collider가 IUsableObject를 가지고 있으면
            {
                UIManager.Inst.SetPopupPanel(true, "F - 사용");
                interact = iUse.Use;
                canInteraction = true;
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
        UIManager.Inst.SetPopupPanel(false);
        iSet = null;
        iUse = null;
        rayItem = null;
        getItem = null;
        interact = null;
        canInteraction = false;
    }

    /// <summary>
    /// 적에게서 데미지 받는 함수
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>죽으면 true 반환</returns>
    public bool TakeDamage(float damage)
    {
        bool result = false;
        Debug.Log($"Player HP : {HP}");
        if (damage > HP)
            result = true;
        HP -= damage;
        return (result);
    }

    public void DisableMovement()
    {
        action.Player.Disable();
        action.ShortCut.Disable();
    }

    public void EnableMovement()
    {
        action.Player.Enable();
        action.ShortCut.Enable();
    }

    void PlayFootstepSFX()
    {
        //AudioManager.Inst.StopAllSFX();
        if (isOnWood)
        {
            switch (state)
            {
                case PlayerState.Sit:
                    AudioManager.Inst.RepeatSFX("Wood_Sneak", 1);
                    break;
                case PlayerState.Stand:
                    AudioManager.Inst.RepeatSFX("Wood_Walking", 1);
                    break;
                case PlayerState.Run:
                    AudioManager.Inst.RepeatSFX("Wood_Running", 1);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (state)
            {
                case PlayerState.Sit:
                    AudioManager.Inst.RepeatSFX("Tile_Sneak", 1);
                    break;
                case PlayerState.Stand:
                    AudioManager.Inst.RepeatSFX("Tile_Walking", 1);
                    break;
                case PlayerState.Run:
                    AudioManager.Inst.RepeatSFX("Tile_Running", 1);
                    break;
                default:
                    break;
            }
        }
    }

    void Die()
    {
        Cursor.lockState = CursorLockMode.Confined;
        DisableMovement();
        UIManager.Inst.SetGameOverPanel(true);
        Debug.Log("Die");
    }

    public void OnGameClear()
    {
        DisableMovement();
        UIManager.Inst.DisableMainCanvas();
        StartCoroutine(GameClearFadeOut());
    }

    IEnumerator GameClearFadeOut()
    {
        yield return new WaitForSeconds(1f);
        float gain = lgg.gain.value.w;
        while (gain > -1.0f)
        {
            lgg.gain.value = new Vector4(1, 1, 1, gain);
            gain -= Time.deltaTime;
            lens.intensity.value -= Time.deltaTime * 0.5f;
            yield return null;
        }
        UIManager.Inst.SetGameClearPanel(true);
        Cursor.lockState = CursorLockMode.Confined;
        yield return null;
    }
}
