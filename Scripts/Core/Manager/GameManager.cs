using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class GameManager : Singleton<GameManager>
{
    Player player;
    InventoryUI inventoryUI;
    [SerializeField]
    ItemDataManager itemData;

    public float clearTime = 0.0f;

    public Player MainPlayer
    {
        get => player;
    }

    public ItemDataManager ItemData
    {
        get => itemData;
    }

    public InventoryUI InvenUI => inventoryUI;

    protected override void Initialize()
    {
        player = FindObjectOfType<Player>();
        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    private void Start()
    {
        AudioManager.Inst.StopAllSFX();
        AudioManager.Inst.PlayBGM("BGM_Normal", MusicTransition.LinearFade);
        clearTime = 0.0f;
    }

    private void Update()
    {
        clearTime += Time.deltaTime;
    }
}
