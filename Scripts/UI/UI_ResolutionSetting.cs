using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ResolutionSetting : MonoBehaviour
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
        int prew = 0;
        int preh = 0;
        foreach (var res in Screen.resolutions)
        {
            if (prew == res.width && preh == res.height)
            {
                continue;
            }
            GameObject obj = Instantiate(menuPrefab, content);
            ContentUI ui = obj.GetComponentInParent<ContentUI>();
            ui.text.text = $"{res.width} * {res.height}";
            ui.onClick += () => Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
            ui.onClick += () => setting.UpdateMenuName();
            ui.onClick += () => Destroy(this.gameObject);
            prew = res.width;
            preh = res.height;
        }

        transform.parent = UIManager.Inst.mainCanvas.transform; 
    }

    private void OnDisable()
    {
        Destroy(this.gameObject);
    }
}
