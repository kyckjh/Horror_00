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

    // �÷��̾� �� ------------------------------------------------------------------------------------------------

    bool isRun = false;
    bool isMove = false;


    const uint InvalidID = uint.MaxValue;       // �� ������ ���� ������ ��

    uint mySlotID = InvalidID;    // InvalidID �̸� ���� �÷��̾��� ������ �����.


    PlayerState state = PlayerState.Stand;

    public PlayerState State { get => state; }


    Coroutine sitCoroutine;

    // �̵�, ȸ�� �� ------------------------------------------------------------------------------------------------

    Vector3 moveDir = Vector3.zero;     // �̵� ����
    Vector2 mouseDelta = Vector2.zero;  // ���콺 ������ ����

    public float moveSpeed = 10.0f;  // �÷��̾� ������ �ӵ�
    public float turnSpeed = 10.0f; // �����̵� �ӵ�(���콺 ����)

    private float xRotate = 0.0f;
    private float yRotate = 0.0f;

    Vector3 initialCamPos;  // �÷��̾� �ɰ� �Ͼ �� ī�޶� ��ġ ������ �� ����ϴ� ī�޶� ���� ��ġ�� �ʱⰪ

    // ��ȣ�ۿ� �� ----------------------------------------------------------------------------------------------------

    bool canInteraction = false;    // ��ȣ�ۿ� �������� ����(���콺 ���ӿ� ������ ���� ���� �� true)

    delegate bool InteractDelegate(ItemData data);  // ��ȣ�ۿ� �� ��������Ʈ

    InteractDelegate interact;

    IUsableObject iUse;
    Ray ray;    // ��ȣ�ۿ� ray

    float rayRange = 7.0f;  // ��ȣ�ۿ� ���� �Ÿ�


    // ����Ƽ �̺�Ʈ �Լ� -------------------------------------------------------------------------------------------------

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        action = new PlayerInputActions();
        anim = GetComponentInChildren<Animator>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        playerCamera = virtualCamera.transform;

        initialCamPos = playerCamera.localPosition; // �ʱ� ī�޶� ���� ��ġ��

        characterPrefabTransform = transform.GetChild(1);   // ĳ���� ���� ��� ������ ���ӿ�����Ʈ ��������


        ray = new Ray(playerCamera.position, playerCamera.forward); // �÷��̾� ���� �������� Ray ����
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
        UIManager_Opening.Inst.OnMenuOpen += action.Player.Disable;            // �޴� �� �� �̵� �� ���� �̵� ����
        UIManager_Opening.Inst.OnMenuClose += action.Player.Enable;
        StartCoroutine(InitializePrefabTransform());
    }

    private void OnEnable()
    {
        action.Player.Enable();

        action.Player.Move.performed += onMove;                 // WASD �̵�
        action.Player.Move.canceled += onMove;              
        action.Player.Mouse.performed += onMouse;               // ���콺�� ���� �̵�(ȸ��)
        action.Player.Mouse.canceled += onMouse;
        action.Player.Interaction.performed += onInteraction;   // FŰ ��ȣ�ۿ�
        action.Player.Run.performed += onRun;                   // Shift �޸���
        action.Player.Run.canceled += onRun;
        action.Player.Sit.performed += onSit;                   // Ctrl �ɱ�
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
        rigid.MovePosition(transform.position + transform.rotation * moveDir * Time.fixedDeltaTime * speed);    // �̵�
    }

    private void Update()
    {
        Rotate();   // ȸ��
        RayTarget();    // �տ� �ִ� ������ �ν� 
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
        //anim.SetInteger("EnemyState", (int)state);
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
        if (Physics.Raycast(ray, out RaycastHit hit, rayRange, LayerMask.GetMask("Interactive")))
        {
            if (iUse == null)    // ��ȣ�ۿ� ������ ������Ʈ�� �ƹ��͵� ���� ��
            {
                iUse = hit.collider.GetComponent<IUsableObject>();
            } 

            if(iUse != null)   // hit�� collider�� IUsableObject�� ������ ������
            {
                if(hit.collider.CompareTag("Equipment"))
                {
                    UIManager_Opening.Inst.SetPopupPanel(true, "F - ����");
                    interact = iUse.Use;
                    canInteraction = true;
                }
                else
                {
                    UIManager_Opening.Inst.SetPopupPanel(true, "F - Ÿ��");
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
    /// ��ü�� ��ȣ�ۿ��� ���� �� UI ���� ���� �۾� �Լ�
    /// </summary>
    void NoInteraction()
    {
        UIManager_Opening.Inst.SetPopupPanel(false);
        iUse = null;
        interact = null;
        canInteraction = false;
    }
}
