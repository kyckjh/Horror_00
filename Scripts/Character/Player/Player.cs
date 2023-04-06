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

    // �÷��̾� �� ------------------------------------------------------------------------------------------------

    bool isRun = false;
    bool isMove = false;

    bool isOnWood = false;

    const float MaxHP = 100;
    float hp = 100;

    ItemData myItemData = null;

    public ItemData MyItemData => myItemData;

    const uint InvalidID = uint.MaxValue;       // �� ������ ���� ������ ��

    uint mySlotID = InvalidID;    // InvalidID �̸� ���� �÷��̾��� ������ �����.

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
                        UIManager.Inst.SetShortCutUI(-1);   // 0, 1, 2 �̿��� ���� ������ ShortCut ���� ���û���
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

    // �̵�, ȸ�� �� ------------------------------------------------------------------------------------------------

    Vector3 moveDir = Vector3.zero;     // �̵� ����
    Vector2 mouseDelta = Vector2.zero;  // ���콺 ������ ����

    public float moveSpeed = 10.0f;  // �÷��̾� ������ �ӵ�
    public float turnSpeed = 10.0f; // �����̵� �ӵ�(���콺 ����) �׽�Ʈ�� ���� public ����, ��ũ��Ʈ���� ������Ƽ�� ����ϱ�

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

    Vector3 initialCamPos;  // �÷��̾� �ɰ� �Ͼ �� ī�޶� ��ġ ������ �� ����ϴ� ī�޶� ���� ��ġ�� �ʱⰪ

    // ��ȣ�ۿ� �� ----------------------------------------------------------------------------------------------------

    bool canInteraction = false;    // ��ȣ�ۿ� �������� ����(���콺 ���ӿ� ������ ���� ���� �� true)

    delegate bool InteractDelegate(ItemData data);  // ��ȣ�ۿ� �� ��������Ʈ

    InteractDelegate interact;
    Item rayItem;      // ray �� item

    
    ISettableObject iSet;
    IUsableObject iUse;
    Ray ray;    // ��ȣ�ۿ� ray

    float rayRange = 2.5f;  // ��ȣ�ۿ� ���� �Ÿ�

    Action<Inventory> getItem;

    // ����Ʈ ���μ��� ------------------------------------------------------------------------------------------------

    Volume volume;
    LiftGammaGain lgg;
    Vignette vig;
    LensDistortion lens;

    // ����Ƽ �̺�Ʈ �Լ� -------------------------------------------------------------------------------------------------

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        action = new PlayerInputActions();
        anim = GetComponentInChildren<Animator>();

        flashLight = flashLightTransform.GetComponentInChildren<Light>();
        lightInitialRotation = flashLightTransform.localRotation.eulerAngles;   // �÷��� �ʱ� localRotation ��        

        playerCamera = Camera.main.transform;

        initialCamPos = playerCamera.localPosition; // �ʱ� ī�޶� ���� ��ġ��

        characterPrefabTransform = transform.GetChild(1);   // ĳ���� ���� ��� ������ ���ӿ�����Ʈ ��������


        ray = new Ray(playerCamera.position, playerCamera.forward); // �÷��̾� ���� �������� Ray ����

        inven = new Inventory();    // �÷��̾� �κ��丮 ����

        //enemies = FindObjectsOfType<Enemy>();
    }

    /// <summary>
    /// �ִϸ��̼� ��� ������ ĳ���� �������� ȸ���� Ʋ������ ���� �����ϱ� ���� 1�ʸ��� �ʱ� ���� ȸ�������� ����
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
        GameManager.Inst.InvenUI.OnInventoryOpen += action.Player.Disable;  // �κ��丮 �� �� �̵� �� ���� �̵� ����
        GameManager.Inst.InvenUI.OnInventoryClose += action.Player.Enable;
        UIManager.Inst.OnMenuOpen += action.Player.Disable;            // �޴� �� �� �̵� �� ���� �̵� ����
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

        action.Player.Move.performed += onMove;                 // WASD �̵�
        action.Player.Move.canceled += onMove;              
        action.Player.Mouse.performed += onMouse;               // ���콺�� ���� �̵�(ȸ��)
        action.Player.Mouse.canceled += onMouse;
        action.Player.Interaction.performed += onInteraction;   // FŰ ��ȣ�ۿ�
        action.Player.Run.performed += onRun;                   // Shift �޸���
        action.Player.Run.canceled += onRun;
        action.Player.Sit.performed += onSit;                   // Ctrl �ɱ�
        action.Player.Sit.canceled += onSit;
                
        action.ShortCut.ShortCut1.performed += OnShortCut1;
        action.ShortCut.ShortCut2.performed += OnShortCut2;
        action.ShortCut.ShortCut3.performed += OnShortCut3;
        action.ShortCut.FlashLight.performed += OnFlashLight;   // R Ű�� �÷��� ��� ����
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
        rigid.MovePosition(transform.position + transform.rotation * moveDir * Time.fixedDeltaTime * speed);    // �̵�
    }

    private void Update()
    {
        Rotate();   // ȸ��
        RayTarget();    // �տ� �ִ� ������ �ν� 
        FlashRotate();
        if(HP < MaxHP)
        {
            HP += Time.deltaTime * 3.0f;
        }
    }

    private void FixedUpdate()
    {
        //Rotate();   // ȸ��


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

    // ��ǲ ���� �Լ� ----------------------------------------------------------------------------------------------------

    private void onInteraction(InputAction.CallbackContext context) // FŰ ������ �� ��ȣ�ۿ�
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
        // RŰ ���������� �÷��� ��� ��ȭ ( intensity�� 0, 2, 4, 8�� 4�ܰ�� ����)
        // �÷��� ��⿡ ���� �� �νİŸ� ����
        if (flashLight.intensity <= 0) // �÷��� 0 -> 2
        {
            flashLight.intensity = 2;
            //foreach(var enemy in enemies)
            {
                Enemy.sightRange_Chase = 2.5f;
                Enemy.sightRange_Suspect = 5.0f;
            }
        }
        else if (flashLight.intensity >= 8) // �÷��� 8 -> 0
        {
            flashLight.intensity = 0;
            //foreach (var enemy in enemies)
            {
                Enemy.sightRange_Chase = 1.0f;
                Enemy.sightRange_Suspect = 2.5f;
            }
        }
        else                                // �÷��� 2-> 4 -> 8
        {
            flashLight.intensity *= 2;
            //foreach (var enemy in enemies)
            {
                Enemy.sightRange_Chase = flashLight.intensity;
                Enemy.sightRange_Suspect = flashLight.intensity * 2.0f;
            }
        }
        // ����Ʈ ���μ������ε� ��� ����
        lgg.gain.value = new Vector4(1, 1, 1, flashLight.intensity * 0.1f - 0.5f);
    }


    /// <summary>
    /// �÷��̾� �� ����ī�޶�(ȭ��) ȸ��
    /// </summary>
    private void Rotate()
    {
        playerCamera.eulerAngles = new Vector3(xRotate, playerCamera.eulerAngles.y, playerCamera.eulerAngles.z);    // ī�޶� x�ุ ȸ��
        xRotate = Mathf.Clamp(xRotate - mouseDelta.y * turnSpeed * Time.fixedDeltaTime, -80, 80);    // ī�޶��� x�� ȸ��(���� ���� �̵�), ���� 80�� ����
        
        yRotate = transform.eulerAngles.y + mouseDelta.x * turnSpeed * Time.fixedDeltaTime;  // �÷��̾��� y�� ȸ��(�¿� ���� �̵�)
        transform.eulerAngles = new Vector3(0, yRotate, 0); // �÷��̾� y�� ȸ��
    }
    /// <summary>
    /// �÷��� ȸ�� �Լ�, ���� ���� �̵����� ���� ���� ���� ������ �÷����� ���Ϸ� �����̵��� ��
    /// </summary>
    void FlashRotate()
    {
        // ���� ���� �̵��� xRotate�� ������ -80 ~ 80 �̹Ƿ� 80�� ���� �� 160���� ���� 0 ~ 1 ������ ������ ����
        // Mathf.Lerp �Լ��� �̿��� �ʱ� �÷��� ȸ���������� -40 ~ +40 �� ȸ���� ������ ������ ��ȯ
        float flashRotation = Mathf.Lerp(lightInitialRotation.x - 40.0f, lightInitialRotation.x + 40.0f, (xRotate + 80.0f) / 160.0f);
        flashLightTransform.localRotation = Quaternion.Euler(flashRotation, lightInitialRotation.y, lightInitialRotation.z);
    }


    /// <summary>
    /// PlayerState ���� �Լ�
    /// </summary>
    /// <param name="newState"></param>
    void ChangeState(PlayerState newState)
    {
        // ���� ���¸� �����鼭 �ؾ� �� �ϵ�
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

        // �� ���·� ���鼭 �ؾ� �� �ϵ�
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
    /// �ɰ� �� �� ī�޶� ��ġ �������ִ� �ڷ�ƾ
    /// </summary>
    /// <param name="isSitting">���� �� true</param>
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
    /// Ray�� �̿��� ī�޶� ������ ������ �ν�
    /// </summary>
    private void RayTarget()
    {
        ray.origin = playerCamera.position;
        ray.direction = playerCamera.transform.forward;
        // rayRange �Ÿ� �ȿ��� Item ���̾ ������ �ִ� Ÿ�ٿ� ray�� hit ���� ��
        if (Physics.Raycast(ray, out RaycastHit hit, rayRange, LayerMask.GetMask("Item", "Interactive")))
        {
            if (rayItem == null && iSet == null && iUse == null)    // ��ȣ�ۿ� ������ ������Ʈ�� �ƹ��͵� ���� ��
            {
                rayItem = hit.collider.GetComponentInParent<Item>();
                iSet = hit.collider.GetComponent<ISettableObject>();
                iUse = hit.collider.GetComponent<IUsableObject>();
            } 

            if (rayItem != null)    // hit �� collider�� Item�� ������ ������
            {
                UIManager.Inst.SetPopupPanel(true);
                getItem = rayItem.GetItem;
                canInteraction = true;
            }
            else if(iSet != null && myItemData != null) // hit �� collider�� ISettableObject�� ������ ������
            {
                UIManager.Inst.SetPopupPanel(true, "F - �ֱ�");
                interact = iSet.Set;
                canInteraction = true;
                
            }
            else if(iUse != null)   // hit�� collider�� IUsableObject�� ������ ������
            {
                UIManager.Inst.SetPopupPanel(true, "F - ���");
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
    /// ��ü�� ��ȣ�ۿ��� ���� �� UI ���� ���� �۾� �Լ�
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
    /// �����Լ� ������ �޴� �Լ�
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>������ true ��ȯ</returns>
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
