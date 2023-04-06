using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class UIManager_Opening : Singleton<UIManager_Opening>
{
    PlayerInputActions action;

    public Action OnMenuOpen;
    public Action OnMenuClose;

    [SerializeField]
    private CanvasGroup mainCanvas;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private GameObject popupPanel;
    [SerializeField]
    private GameObject questPanel;
    [SerializeField]
    private GameObject messagePanel;

    // Message Panel ----------------------------------------------------------------------------

    // 메시지 상자의 RectTransform
    RectTransform messageRectPos;
    // 메시지 상자의 초기위치
    Vector2 messageInitialPos;
    // 띄울 메시지 UI
    TextMeshProUGUI messageText;
    // 메시지 배경 이미지
    Image messageBackground;
    // 메시지 띄울 때 색깔 변경하는 코루틴
    Coroutine messageColorCoroutine;
    // 메시지 띄울 때 위치 변경하는 코루틴
    Coroutine messageRectCoroutine;
    // 메시지 색깔 변경되는 속도
    float messageColorSpeed = 4.0f;
    // 메시지 상자 올라가는 속도
    float messageRectSpeed = 4.0f;

    // --------------------------------------------------------------------------------------------------

    Transform questsParent;

    // 퀘스트 목록 게임오브젝트
    // 01 환자명부 입수
    // 02 USB 획득
    // 03 USB에 환자 데이터 옮기기
    // 04 병원에서 탈출하기
    GameObject[] quests;

    TextMeshProUGUI popupText;

    [SerializeField]
    private GameObject aimImage;


    protected override void Awake()
    {
        base.Awake();
        action = new PlayerInputActions();
        popupText = popupPanel.GetComponentInChildren<TextMeshProUGUI>();
        messageRectPos = messagePanel.GetComponent<RectTransform>();
        messageInitialPos = messageRectPos.anchoredPosition;
        messageText = messagePanel.GetComponentInChildren<TextMeshProUGUI>();
        messageBackground = messagePanel.GetComponentInChildren<Image>();
        questsParent = questPanel.transform.GetChild(0);
        quests = new GameObject[questsParent.childCount];
        for(int i = 0; i < questsParent.childCount; i++)
        {
            quests[i] = questsParent.GetChild(i).gameObject;
        }
    }

    private void Start()
    {
        OnMenuOpen += () => SetAimImage(false);
        OnMenuClose += () => SetAimImage(true);
        OnMenuOpen += () => SetQuestPanel(false);
        OnMenuClose += () => SetQuestPanel(true);
    }

    private void OnEnable()
    {
        EnableMainCanvas();
        action.UI.Enable();
        action.UI.PauseMenu.performed += onPauseMenu;   // esc키 누르면 메뉴 열림
    }

    private void OnDisable()
    {
        action.UI.PauseMenu.performed -= onPauseMenu;
        action.UI.Disable();
        if(mainCanvas != null)
        {
            DisableMainCanvas();
        }
    }

    private void onPauseMenu(InputAction.CallbackContext context)
    {
        SetMenuPanel();
    }


    /// <summary>
    /// 메뉴 패널 On or Off 함수
    /// </summary>
    /// <param name="_tf">true면 On, false면 Off</param>
    public void SetMenuPanel(bool _tf)
    {
        menuPanel.SetActive(_tf);
        if(_tf)     // 열 때
        {
            GameManager.Inst.InvenUI.Close();           // 인벤토리 열려있으면 닫기
            //SetAimImage(false);                         // 화면 중앙 에임 동그라미 지우기
            Cursor.lockState = CursorLockMode.Confined; // 마우스 보이게하기
            OnMenuOpen?.Invoke();
        }
        else        // 닫을 때
        {
            //SetAimImage(true);                          // 화면 중앙 에임 동그라미 다시 나타내기
            Cursor.lockState = CursorLockMode.Locked;   // 마우스 안보이게하기
            OnMenuClose?.Invoke();
        }
    }

    /// <summary>
    /// 메뉴 패널 On/Off 토글 함수
    /// </summary>
    /// <returns>패널 On 할 때 true, Off 할 때 false</returns>
    public bool SetMenuPanel()
    {
        bool result;
        if (menuPanel.activeSelf)                       // 메뉴 열려있으면 닫기
        {
            menuPanel.SetActive(false);
            //SetAimImage(true);                          // 화면 중앙 에임 동그라미 다시 나타내기
            Cursor.lockState = CursorLockMode.Locked;   // 마우스 안보이게하기
            OnMenuClose?.Invoke();
            result = false;
        }
        else    // 메뉴 닫혀있으면 열기
        {
            GameManager.Inst.InvenUI.Close();           // 인벤토리 열려있으면 닫기
            menuPanel.SetActive(true);
            //SetAimImage(false);                         // 화면 중앙 에임 동그라미 지우기
            Cursor.lockState = CursorLockMode.Confined; // 마우스 보이게하기
            OnMenuOpen?.Invoke();
            result = true;
        }
        return result;
    }

    public void SetPopupPanel(bool _tf, string text = "F - 줍기")
    {
        popupPanel.SetActive(_tf);
        popupText.text = text;
    }

    public void SetMessagePanel(string text = "F - 줍기")
    {
        messageText.text = text;
        if(messageColorCoroutine != null)
        {
            StopCoroutine(messageColorCoroutine);
        }
        messageColorCoroutine = StartCoroutine(MessageColorCoroutine());
        if (messageRectCoroutine != null)
        {
            StopCoroutine(messageRectCoroutine);
        }
        messageRectCoroutine = StartCoroutine(MessageRectCoroutine());
    }

    IEnumerator MessageRectCoroutine()
    {
        messageRectPos.anchoredPosition = messageInitialPos - new Vector2(0, 50);
        while (Vector2.Distance(messageRectPos.anchoredPosition, messageInitialPos) > 0.1f)
        {
            messageRectPos.anchoredPosition = Vector2.Lerp(messageRectPos.anchoredPosition, messageInitialPos, Time.deltaTime * messageRectSpeed);
            yield return null;
        }
        yield return null;
    }

    /// <summary>
    /// MessagePanel을 잠깐 띄웠다가 없애는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator MessageColorCoroutine()
    {
        while(messageText.color.a < 1)
        {
            messageBackground.color += new Color(0, 0, 0, Time.deltaTime * messageColorSpeed);
            messageText.color += new Color(0, 0, 0, Time.deltaTime * messageColorSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        while (messageText.color.a > 0)
        {
            messageBackground.color -= new Color(0, 0, 0, Time.deltaTime * messageColorSpeed);
            messageText.color -= new Color(0, 0, 0, Time.deltaTime * messageColorSpeed);
            yield return null;
        }
        yield return null;
    }

    public void SetAimImage(bool _tf)
    {
        aimImage.SetActive(_tf);
    }  

    public void SetQuestPanel(bool _tf)
    {
        questPanel.SetActive(_tf);
    }

    /// <summary>
    /// 퀘스트 활성화/비활성화 함수
    /// </summary>
    /// <param name="_tf">true면 활성화, false면 비활성화</param>
    /// <param name="number">활성화할 퀘스트 넘버</param>
    public void SetQuests(bool _tf, int number)
    {
        if(_tf)
        {
            if (number < quests.Length)
            {
                quests[number].SetActive(true);
            }
            else
            {
                Debug.Log("퀘스트넘버 범위를 벗어났습니다.");
            }
        }
        else
        {
            if(number < quests.Length)
            {
                Destroy(quests[number]);
            }
            else
            {
                Debug.Log("퀘스트넘버 범위를 벗어났습니다.");
            }
        }
    }

    public void DisableMainCanvas()
    {
        mainCanvas.interactable = false;
        mainCanvas.alpha = 0;
        mainCanvas.blocksRaycasts = false;
    }

    public void EnableMainCanvas()
    {
        mainCanvas.interactable = true;
        mainCanvas.alpha = 1;
        mainCanvas.blocksRaycasts = true;
    }
}
