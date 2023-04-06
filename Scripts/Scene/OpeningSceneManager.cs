using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class OpeningSceneManager : MonoBehaviour
{
    [SerializeField]
    NPC_BlackMan npc;

    PlayerInputActions action;

    Canvas canvas;

    [SerializeField]
    Transform glass;

    Animator glassAnim;

    float glassDelta = 0.0f;
    float minusGlassLimit = -5.0f;
    float plusGlassLimit = 5.0f;
    float glassRecoverySpeed = 5.0f;

    Button[] buttons;

    Vector3 initialGlassRotation;
    Vector3 initialCamRotation;
    Vector2 mouseDelta = Vector2.zero;

    Transform mainCamera;

    float xCamDelta = 0.0f;
    float yCamDelta = 0.0f;
    float limitCamAngle = 20.0f;
    float cameraRecoverySpeed = 2.0f;

    bool isGameStart = false;

    private void Awake()
    {
        action = new PlayerInputActions();
        canvas = FindObjectOfType<Canvas>();
        mainCamera = Camera.main.transform;
        initialCamRotation = mainCamera.rotation.eulerAngles;
        initialGlassRotation = glass.rotation.eulerAngles;
        glassAnim = glass.GetComponent<Animator>();
        if(canvas != null)
        {
            buttons = canvas.GetComponentsInChildren<Button>();
            buttons[0].onClick.AddListener(GameStart);
            buttons[1].onClick.AddListener(() => Application.Quit()) ;
        }
    }

    private void Start()
    {
        AudioManager.Inst.PlayBGM("OP");
    }

    private void OnEnable()
    {
        action.Player.Enable();
        action.Player.Mouse.performed += onMouse;               // 마우스로 시점 이동(회전)
        action.Player.Mouse.canceled += onMouse;
    }

    private void OnDisable()
    {
        action.Player.Mouse.canceled -= onMouse;
        action.Player.Mouse.performed -= onMouse;
        action.Player.Disable();
    }

    private void onMouse(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (!isGameStart)
        {
            RotateCamera();
            RotateGlass();
        }
    }

    void RotateGlass()
    {
        glassDelta = initialGlassRotation.x - mouseDelta.x + mouseDelta.y;  // 마우스 움직임에 따라 유리잔이 기울어질 각도
        // x축을 기준으로 유리잔 회전, 회전 각도 제한
        // Vector3.Lerp로 하면 버그생겨서 Quaternion으로 변환
        Quaternion qDelta = Quaternion.Euler(Mathf.Clamp(glassDelta, minusGlassLimit, plusGlassLimit), initialGlassRotation.y, initialGlassRotation.z);    

        glass.rotation = Quaternion.Lerp(glass.rotation, qDelta, Time.deltaTime * glassRecoverySpeed);
    }

    private void RotateCamera()
    {
        xCamDelta = Mathf.Clamp(initialCamRotation.x - mouseDelta.y, initialCamRotation.x - limitCamAngle, initialCamRotation.x + limitCamAngle); // 카메라 x축 회전(상하)
        yCamDelta = Mathf.Clamp(initialCamRotation.y + mouseDelta.x, initialCamRotation.y - limitCamAngle, initialCamRotation.y + limitCamAngle); // 카메라 y축 회전(좌우)

        Quaternion qDelta = Quaternion.Euler(xCamDelta, yCamDelta, initialCamRotation.z);    // Vector3.Lerp로 하면 버그생겨서 Quaternion으로 변환
        mainCamera.rotation = Quaternion.Lerp(mainCamera.rotation, qDelta, Time.deltaTime * cameraRecoverySpeed);

        //mainCamera.eulerAngles = Vector3.Lerp(mainCamera.eulerAngles, delta, Time.deltaTime);
    }

    void GameStart()
    {
        AudioManager.Inst.StopBGM();
        AudioManager.Inst.PlaySFX("Button_New_Game");

        glassAnim.enabled = true;
        glassAnim.SetTrigger("Activate");
        StartCoroutine(GlassSFX());
        StartCoroutine(ReplaceCamera());
        StartCoroutine(ButtonSelected(0));
        StartCoroutine(ButtonFade(1));
        //StartCoroutine(ButtonFade(2)); 3번째 버튼 삭제함
        npc.FirstMoveStart();
    }

    IEnumerator ReplaceCamera()
    {
        isGameStart = true;
        while(Vector3.Angle(mainCamera.rotation.eulerAngles, initialCamRotation) > 0.1f)
        {
            mainCamera.rotation = Quaternion.Lerp(mainCamera.rotation, Quaternion.Euler(initialCamRotation.x, initialCamRotation.y, initialCamRotation.z), Time.deltaTime * cameraRecoverySpeed * 0.25f);
            glass.rotation = Quaternion.Lerp(glass.rotation, Quaternion.Euler(initialGlassRotation.x, initialGlassRotation.y, initialGlassRotation.z), Time.deltaTime * glassRecoverySpeed);
            yield return null;
        }
        yield return null;
    }

    IEnumerator ButtonFade(int number)
    {
        TextMeshProUGUI text = buttons[number].GetComponentInChildren<TextMeshProUGUI>();
        buttons[number].interactable = false;
        while(text.color.a > 0)
        {
            text.color = Color.Lerp(text.color, new Color(1, 1, 1, 0), Time.deltaTime * 3.0f);
            text.fontSize -= Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    IEnumerator ButtonSelected(int number)
    {
        TextMeshProUGUI text = buttons[number].GetComponentInChildren<TextMeshProUGUI>();
        buttons[number].interactable = false;
        while(text.fontSize < 70)
        {            
            text.fontSize = Mathf.Lerp(text.fontSize, 75, Time.deltaTime * 10.0f);
            yield return null;
        }
        while (text.fontSize > 60)
        {
            text.fontSize = Mathf.Lerp(text.fontSize, 55, Time.deltaTime * 10.0f);
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(ButtonFade(number));
        yield return new WaitForSeconds(2.0f);
        //SceneChangeManager.Inst.LoadOpeningScene_2();
        SceneChangeManager.Inst.LoadScene(1);
        yield return null;
    }

    IEnumerator GlassSFX()
    {
        yield return new WaitForSeconds(1.0f);
        AudioManager.Inst.PlaySFX("Glass_Break");
        AudioManager.Inst.PlayBGM("Horror_BGM2", MusicTransition.CrossFade, 7.0f);
    }
}
