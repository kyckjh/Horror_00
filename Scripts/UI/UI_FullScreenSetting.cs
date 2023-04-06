using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FullScreenSetting : MonoBehaviour
{
    [SerializeField]
    GameObject menuPrefab;
    Transform content;

    private void Awake()
    {
        content = transform.GetChild(1).GetChild(0);
    }

    private void OnEnable()
    {
        UI_Setting setting = GetComponentInParent<UI_Setting>();

        GameObject obj = Instantiate(menuPrefab, content);
        ContentUI ui = obj.GetComponent<ContentUI>();
        ui.text.text = "��üȭ�� ���";
        Resolution res = Screen.currentResolution;
        ui.onClick += () => Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);
        ui.onClick += () => setting.UpdateMenuName();
        ui.onClick += () => Destroy(this.gameObject);        


        obj = Instantiate(menuPrefab, content);
        ui = obj.GetComponent<ContentUI>();
        ui.text.text = "�׵θ� ���� â ���";
        res = Screen.resolutions[Screen.resolutions.Length - 1];
        ui.onClick += () => Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);
        ui.onClick += () => setting.UpdateMenuName();
        ui.onClick += () => Destroy(this.gameObject);


        obj = Instantiate(menuPrefab, content);
        ui = obj.GetComponent<ContentUI>();
        ui.text.text = "â ���";
        res = Screen.currentResolution;
        ui.onClick += () => Screen.SetResolution(res.width, res.height, false);
        ui.onClick += () => setting.UpdateMenuName();
        ui.onClick += () => Destroy(this.gameObject);

        transform.parent = UIManager.Inst.mainCanvas.transform; 
    }

    private void OnDisable()
    {
        Destroy(this.gameObject);
    }
}
