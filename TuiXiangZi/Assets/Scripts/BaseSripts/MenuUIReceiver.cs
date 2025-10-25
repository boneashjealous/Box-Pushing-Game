using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIReceiver : MonoBehaviour
{
    //单例
    private static MenuUIReceiver instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    private MenuUIInteraction menuUIInteraction;
    private void Start()
    {
        try
        {
            MenuUIInteraction menuUIInteraction = GameObject.Find("MenuManager").GetComponent<MenuUIInteraction>();
            this.menuUIInteraction = menuUIInteraction ?? throw new Exception("menuUIInteraction参数不能为null");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    public void SetUIData(UIData uiData)
    {
        uiData._continue.GetComponent<Button>().onClick.AddListener(delegate () { ContinueGame(); });
        uiData._selectArchive.GetComponent<Button>().onClick.AddListener(delegate () { SelectSaveSlot(); });
        uiData._closeButton_0.GetComponent<Button>().onClick.AddListener(delegate () { CloseAllPanel(); });
        uiData._newGame.GetComponent<Button>().onClick.AddListener(delegate () { StartNewGame(); });
        uiData._settings.GetComponent<Button>().onClick.AddListener(delegate () { OpenSettings(); });
        uiData._quit.GetComponent<Button>().onClick.AddListener(delegate () { QuitGame(); });
        uiData._archiveBar_0.GetComponent<Button>().onClick.AddListener(delegate () { SelectSaveSlot0(); });
        uiData._archiveBar_1.GetComponent<Button>().onClick.AddListener(delegate () { SelectSaveSlot1(); });
        uiData._archiveBar_2.GetComponent<Button>().onClick.AddListener(delegate () { SelectSaveSlot2(); });
        uiData._archiveRename_0.GetComponent<Button>().onClick.AddListener(delegate () { RenameSaveSlot0(); });
        uiData._archiveRename_1.GetComponent<Button>().onClick.AddListener(delegate () { RenameSaveSlot1(); });
        uiData._archiveRename_2.GetComponent<Button>().onClick.AddListener(delegate () { RenameSaveSlot2(); });
        uiData._archiveTMP[0].GetComponent<TMP_InputField>().onEndEdit.AddListener(delegate (string name) { EndInput0(name); });
        uiData._archiveTMP[1].GetComponent<TMP_InputField>().onEndEdit.AddListener(delegate (string name) { EndInput1(name); });
        uiData._archiveTMP[2].GetComponent<TMP_InputField>().onEndEdit.AddListener(delegate (string name) { EndInput2(name); });
        uiData._archiveDelete_0.GetComponent<Button>().onClick.AddListener(delegate () { DeleteSaveSlot0(); });
        uiData._archiveDelete_1.GetComponent<Button>().onClick.AddListener(delegate () { DeleteSaveSlot1(); });
        uiData._archiveDelete_2.GetComponent<Button>().onClick.AddListener(delegate () { DeleteSaveSlot2(); });
        uiData._applyButton.GetComponent<Button>().onClick.AddListener(delegate () { ApplySetting(); });
    }
    // 主菜单功能
    public void ContinueGame()// 继续游戏
    {
        menuUIInteraction.ContinueGame();
    }
    public void SelectSaveSlot()//选择存档
    {
        menuUIInteraction.SelectSaveSlot();
    }
    public void StartNewGame()// 新游戏
    {
        menuUIInteraction.StartNewGame();
    }
    public void OpenSettings()// 设置
    {
        menuUIInteraction.OpenSettings();
    }
    public void QuitGame()// 退出游戏
    {
        menuUIInteraction.QuitGame();
    }
    public void CloseAllPanel()//关闭所有面板
    {
        menuUIInteraction.CloseAllPanel();
    }

    // 存档选择功能
    public void SelectSaveSlot0()// 选择存档0
    {
        menuUIInteraction.SelectArchiveData(0);
    }
    public void SelectSaveSlot1()// 选择存档1
    {
        menuUIInteraction.SelectArchiveData(1);
    }
    public void SelectSaveSlot2()// 选择存档2
    {
        menuUIInteraction.SelectArchiveData(2);
    }

    // 存档管理功能
    public void RenameSaveSlot0()// 重命名存档0
    {
        menuUIInteraction.RenameSaveSlot(0);
    }
    public void RenameSaveSlot1()// 重命名存档1
    {
        menuUIInteraction.RenameSaveSlot(1);
    }
    public void RenameSaveSlot2()// 重命名存档2
    {
        menuUIInteraction.RenameSaveSlot(2);
    }
    public void EndInput0(string name)//结束输入0
    {
        menuUIInteraction.EndInput(name,0);
    }
    public void EndInput1(string name)//结束输入1
    {
        menuUIInteraction.EndInput(name, 1);
    }
    public void EndInput2(string name)//结束输入2
    {
        menuUIInteraction.EndInput(name, 2);
    }
    public void DeleteSaveSlot0()// 删除存档0
    {
        menuUIInteraction.DeleteSaveSlot(0);
    }
    public void DeleteSaveSlot1()// 删除存档1
    {
        menuUIInteraction.DeleteSaveSlot(1);
    }
    public void DeleteSaveSlot2()// 删除存档2
    {
        menuUIInteraction.DeleteSaveSlot(2);
    }
    public void ApplySetting()//应用设置
    {
        menuUIInteraction.ApplySetting();
    }
}
