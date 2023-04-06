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

    // �޽��� ������ RectTransform
    RectTransform messageRectPos;
    // �޽��� ������ �ʱ���ġ
    Vector2 messageInitialPos;
    // ��� �޽��� UI
    TextMeshProUGUI messageText;
    // �޽��� ��� �̹���
    Image messageBackground;
    // �޽��� ��� �� ���� �����ϴ� �ڷ�ƾ
    Coroutine messageColorCoroutine;
    // �޽��� ��� �� ��ġ �����ϴ� �ڷ�ƾ
    Coroutine messageRectCoroutine;
    // �޽��� ���� ����Ǵ� �ӵ�
    float messageColorSpeed = 4.0f;
    // �޽��� ���� �ö󰡴� �ӵ�
    float messageRectSpeed = 4.0f;

    // --------------------------------------------------------------------------------------------------

    Transform questsParent;

    // ����Ʈ ��� ���ӿ�����Ʈ
    // 01 ȯ�ڸ�� �Լ�
    // 02 USB ȹ��
    // 03 USB�� ȯ�� ������ �ű��
    // 04 �������� Ż���ϱ�
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
        action.UI.PauseMenu.performed += onPauseMenu;   // escŰ ������ �޴� ����
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
    /// �޴� �г� On or Off �Լ�
    /// </summary>
    /// <param name="_tf">true�� On, false�� Off</param>
    public void SetMenuPanel(bool _tf)
    {
        menuPanel.SetActive(_tf);
        if(_tf)     // �� ��
        {
            GameManager.Inst.InvenUI.Close();           // �κ��丮 ���������� �ݱ�
            //SetAimImage(false);                         // ȭ�� �߾� ���� ���׶�� �����
            Cursor.lockState = CursorLockMode.Confined; // ���콺 ���̰��ϱ�
            OnMenuOpen?.Invoke();
        }
        else        // ���� ��
        {
            //SetAimImage(true);                          // ȭ�� �߾� ���� ���׶�� �ٽ� ��Ÿ����
            Cursor.lockState = CursorLockMode.Locked;   // ���콺 �Ⱥ��̰��ϱ�
            OnMenuClose?.Invoke();
        }
    }

    /// <summary>
    /// �޴� �г� On/Off ��� �Լ�
    /// </summary>
    /// <returns>�г� On �� �� true, Off �� �� false</returns>
    public bool SetMenuPanel()
    {
        bool result;
        if (menuPanel.activeSelf)                       // �޴� ���������� �ݱ�
        {
            menuPanel.SetActive(false);
            //SetAimImage(true);                          // ȭ�� �߾� ���� ���׶�� �ٽ� ��Ÿ����
            Cursor.lockState = CursorLockMode.Locked;   // ���콺 �Ⱥ��̰��ϱ�
            OnMenuClose?.Invoke();
            result = false;
        }
        else    // �޴� ���������� ����
        {
            GameManager.Inst.InvenUI.Close();           // �κ��丮 ���������� �ݱ�
            menuPanel.SetActive(true);
            //SetAimImage(false);                         // ȭ�� �߾� ���� ���׶�� �����
            Cursor.lockState = CursorLockMode.Confined; // ���콺 ���̰��ϱ�
            OnMenuOpen?.Invoke();
            result = true;
        }
        return result;
    }

    public void SetPopupPanel(bool _tf, string text = "F - �ݱ�")
    {
        popupPanel.SetActive(_tf);
        popupText.text = text;
    }

    public void SetMessagePanel(string text = "F - �ݱ�")
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
    /// MessagePanel�� ��� ����ٰ� ���ִ� �ڷ�ƾ
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
    /// ����Ʈ Ȱ��ȭ/��Ȱ��ȭ �Լ�
    /// </summary>
    /// <param name="_tf">true�� Ȱ��ȭ, false�� ��Ȱ��ȭ</param>
    /// <param name="number">Ȱ��ȭ�� ����Ʈ �ѹ�</param>
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
                Debug.Log("����Ʈ�ѹ� ������ ������ϴ�.");
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
                Debug.Log("����Ʈ�ѹ� ������ ������ϴ�.");
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
