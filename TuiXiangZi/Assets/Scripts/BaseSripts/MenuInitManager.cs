using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//定义主菜单的初始化
public class MenuInitManager : MonoBehaviour
{
    private UIData uiData;

    private void Awake()
    {
        uiData = new UIData();
        uiData._continue = GameObject.Find("Continue");
        uiData._saveSlotPanel = GameObject.Find("ArchivePanel");
        uiData._settingsPanel = GameObject.Find("SettingsPanel");
        uiData._waringPanel = GameObject.Find("WaringPanel");
        uiData._waringText = GameObject.Find("WaringText");
        uiData._archiveBar_0 = GameObject.Find("ArchiveBar_0");
        uiData._archiveBar_1 = GameObject.Find("ArchiveBar_1");
        uiData._archiveBar_2 = GameObject.Find("ArchiveBar_2");
        uiData._archiveTMP = new GameObject[3] { GameObject.Find("ArchiveInputField_0"), GameObject.Find("ArchiveInputField_1"), GameObject.Find("ArchiveInputField_2") };
        uiData._archiveTimeTMP =new GameObject[3] { GameObject.Find("ArchiveTimeText_0"), GameObject.Find("ArchiveTimeText_1"), GameObject.Find("ArchiveTimeText_2") };
        uiData._archiveDelete_0 = GameObject.Find("ArchiveDelete_0");
        uiData._archiveDelete_1 = GameObject.Find("ArchiveDelete_1");
        uiData._archiveDelete_2 = GameObject.Find("ArchiveDelete_2");
        uiData._archiveRename_0 = GameObject.Find("ArchiveRename_0");
        uiData._archiveRename_1 = GameObject.Find("ArchiveRename_1");
        uiData._archiveRename_2 = GameObject.Find("ArchiveRename_2");
        uiData._selectArchive =GameObject.Find("SelectArchive");
        uiData._closeButton_0 = GameObject.Find("CloseButton_0");
        uiData._newGame = GameObject.Find("NewGame");
        uiData._settings = GameObject.Find("Settings");
        uiData._quit = GameObject.Find("Quit");
        uiData._applyButton = GameObject.Find("ApplyButton");
        uiData._resDropdown = GameObject.Find("ResDropdown");
        uiData._fullWinToggle = GameObject.Find("FullWinToggle");
        GameObject.Find("MenuManager").GetComponent<MenuUIInteraction>().SetUIData(uiData);
        GameObject.Find("MenuManager").GetComponent<MenuUIReceiver>().SetUIData(uiData);
    }
}

public class UIData 
{
    public GameObject _continue;
    public GameObject _saveSlotPanel;
    public GameObject _settingsPanel;
    public GameObject _waringPanel;
    public GameObject _waringText;
    public GameObject _archiveBar_0;
    public GameObject _archiveBar_1;
    public GameObject _archiveBar_2;
    public GameObject[] _archiveTMP;
    public GameObject[] _archiveTimeTMP;
    public GameObject _archiveDelete_0;
    public GameObject _archiveDelete_1;
    public GameObject _archiveDelete_2;
    public GameObject _archiveRename_0;
    public GameObject _archiveRename_1;
    public GameObject _archiveRename_2;
    public GameObject _selectArchive;
    public GameObject _closeButton_0;
    public GameObject _newGame;
    public GameObject _settings;
    public GameObject _quit;
    public GameObject _applyButton;
    public GameObject _resDropdown;
    public GameObject _fullWinToggle;
}

