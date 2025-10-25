using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//定义主菜单UI处理系统
public class MenuUIInteraction : MonoBehaviour
{
    //单例
    private static MenuUIInteraction instance;
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
    //基础管理器接口引用
    private BaseManager baseManager;
    private ViewUpdate viewUpdate;
    //初始化
    private void Start()
    {
        BaseManager baseManager = GameObject.Find("BaseManager").GetComponent<BaseManager>();
        this.baseManager = baseManager ?? throw new Exception("baseManager参数不能为null");
        CloseAllPanel();
        UpdateArchiveUI(baseManager.QueryArchiveMetaDatas());
    }
    public void SetUIData(UIData uiData)
    {
        viewUpdate = new ViewUpdate(uiData);
    }
    //逻辑处理
    //更新存档相关UI显示
    private void UpdateArchiveUI(ArchiveMetaData[] archiveMetaDatas)
    {
        if (archiveMetaDatas[0] == null)
        {
            viewUpdate.UpdateContinueButton(false);
            viewUpdate.UpdateArchiveBar0(false);
            viewUpdate.UpdateArchiveText("无存档", "",0);
        }
        else
        {
            viewUpdate.UpdateArchiveBar0(true);
            viewUpdate.UpdateArchiveText(archiveMetaDatas[0].saveName, archiveMetaDatas[0].lastModified.ToString(),0);
        }
        if (archiveMetaDatas[1] == null)
        {
            viewUpdate.UpdateArchiveBar1(false);
            viewUpdate.UpdateArchiveText("无存档", "", 1);
        }
        else
        {
            viewUpdate.UpdateArchiveBar1(true);
            viewUpdate.UpdateArchiveText(archiveMetaDatas[1].saveName, archiveMetaDatas[1].lastModified.ToString(), 1);
        }
        if (archiveMetaDatas[2] == null)
        {
            viewUpdate.UpdateArchiveBar2(false);
            viewUpdate.UpdateArchiveText("无存档", "", 2);
        }
        else
        {
            viewUpdate.UpdateArchiveBar2(true);
            viewUpdate.UpdateArchiveText(archiveMetaDatas[2].saveName, archiveMetaDatas[2].lastModified.ToString(),2);
        }
    }
    //检查存档是否丢失
    private ArchiveMetaData CheckArchiveMetaDatas(int index)
    {
        if (index < 3)
        {
            ArchiveMetaData[] archiveMetaDatas = baseManager.GetArchiveMetaDatas();
            if (archiveMetaDatas[index] != null)
            {
                return archiveMetaDatas[index];
            }
            else
            {
                UpdateArchiveUI(archiveMetaDatas);
                throw new Exception("存档意外丢失");
            }
        }
        else
        {
            throw new Exception("索引非法");
        }
    }
    // 继续游戏
    public void ContinueGame()
    {
        try
        {
            //加载存档
            baseManager.LoadGame(CheckArchiveMetaDatas(0));
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    //选择存档
    public void SelectSaveSlot()
    {
        viewUpdate.UpdateWaringPanel();
        viewUpdate.UpdateSaveSlotPanel(true);
        viewUpdate.UpdateSettingsPanel(false);
    }
    //关闭所有面板
    public void CloseAllPanel()//关闭所有面板
    {
        viewUpdate.UpdateWaringPanel();
        viewUpdate.UpdateSaveSlotPanel(false);
        viewUpdate.UpdateSettingsPanel(false);
    }
    //新游戏
    public void StartNewGame()// 新游戏
    {
        CloseAllPanel();
        if (baseManager.GetArchiveMetaDatas()[2] == null)
        {
            baseManager.NewGame();
        }
        else
        {
            viewUpdate.UpdateWaringPanel("存档栏位已满");
        }
    }
    //打开设置面板
    public void OpenSettings()
    {
        viewUpdate.UpdateWaringPanel();
        viewUpdate.UpdateSaveSlotPanel(false);
        viewUpdate.UpdateSettingsPanel(true);
    }
    //退出游戏
    public void QuitGame()
    {
        Application.Quit();
    }
    //选择存档
    public void SelectArchiveData(int index)
    {
        //加载存档
        baseManager.LoadGame(CheckArchiveMetaDatas(index));
    }
    //重命名存档
    public void RenameSaveSlot(int index)
    {
        viewUpdate._archiveTMP[index].interactable = true;
        viewUpdate._archiveTMP[index].ActivateInputField();
    }
    //结束输入
    public void EndInput(string name,int index)
    {
        ArchiveMetaData archiveMetaData = baseManager.GetArchiveMetaDatas()[index];
        archiveMetaData.saveName = name;
        baseManager.SaveMetaData(archiveMetaData);
        StartCoroutine(Wait(index));
    }
    //删除存档
    public void DeleteSaveSlot(int index)
    {
        baseManager.DeleteArchive(baseManager.GetArchiveMetaDatas()[index].id);
        UpdateArchiveUI(baseManager.QueryArchiveMetaDatas());
    }
    //应用设置
    public void ApplySetting()
    {
        int res = viewUpdate.GetRes();
        int w;
        int h;
        switch (res)
        {
            case 0:
                w = 1920; h = 1080;
                break;
            case 1:
                w = 1280; h = 720;
                break;
            case 2:
                w = 1440; h = 1080;
                break;
            default:
                w = Screen.height; h = Screen.width;
                break;
        }
        baseManager.ChangeResolution(w, h, viewUpdate.GetFullWin());
    }

    IEnumerator Wait(int index)
    {
        yield return  new WaitForEndOfFrame();
        viewUpdate._archiveTMP[index].interactable = false;
    }









    //定义视图更新系统
    public class ViewUpdate
    {
        //UI数据引用
        private GameObject _continue;
        private GameObject _saveSlotPanel;
        private GameObject _settingsPanel;
        private GameObject _waringPanel;
        private TextMeshProUGUI _waringText;
        private Button _archiveBar_0;
        private Button _archiveBar_1;
        private Button _archiveBar_2;
        public readonly TMP_InputField[] _archiveTMP;
        public readonly TextMeshProUGUI[] _archiveTimeTMP;
        private GameObject _archiveDelete_0;
        private GameObject _archiveDelete_1;
        private GameObject _archiveDelete_2;
        private GameObject _archiveRename_0;
        private GameObject _archiveRename_1;
        private GameObject _archiveRename_2;
        private TMP_Dropdown _resDropdown;
        private Toggle _fullWinToggle;
        public ViewUpdate(UIData uiData)
        {
            _continue = uiData._continue;
            _saveSlotPanel = uiData._saveSlotPanel;
            _settingsPanel = uiData._settingsPanel;
            _waringPanel = uiData._waringPanel;
            _waringText = uiData._waringText.GetComponent<TextMeshProUGUI>();
            _archiveBar_0 = uiData._archiveBar_0.GetComponent<Button>();
            _archiveBar_1 = uiData._archiveBar_1.GetComponent<Button>();
            _archiveBar_2 = uiData._archiveBar_2.GetComponent<Button>();
            _archiveTMP = new TMP_InputField[3] { uiData._archiveTMP[0].GetComponent<TMP_InputField>(), uiData._archiveTMP[1].GetComponent<TMP_InputField>(), uiData._archiveTMP[2].GetComponent<TMP_InputField>() };
            _archiveTimeTMP = new TextMeshProUGUI[3] { uiData._archiveTimeTMP[0].GetComponent<TextMeshProUGUI>(), uiData._archiveTimeTMP[1].GetComponent<TextMeshProUGUI>(), uiData._archiveTimeTMP[2].GetComponent<TextMeshProUGUI>() };
            _archiveDelete_0 = uiData._archiveDelete_0;
            _archiveDelete_1 = uiData._archiveDelete_1;
            _archiveDelete_2 = uiData._archiveDelete_2;
            _archiveRename_0 = uiData._archiveRename_0;
            _archiveRename_1 = uiData._archiveRename_1;
            _archiveRename_2 = uiData._archiveRename_2;
            _resDropdown = uiData._resDropdown.GetComponent<TMP_Dropdown>();
            _fullWinToggle = uiData._fullWinToggle.GetComponent<Toggle>();
        }
        //操作继续按钮
        public void UpdateContinueButton(bool b)
        {
            _continue.SetActive(b);
        }
        //更新存档面板
        public void UpdateSaveSlotPanel(bool b)
        {
            _saveSlotPanel.SetActive(b);
        }
        //更新设置面板
        public void UpdateSettingsPanel(bool b)
        {
            _settingsPanel.SetActive(b);
        }
        //关闭警告面板
        public void UpdateWaringPanel()
        {
            _waringPanel.SetActive(false);
        }
        //提示警告
        public void UpdateWaringPanel(string waring)
        {
            _waringPanel.SetActive(true);
            _waringText.SetText(waring);
        }
        //更新存档栏
        public void UpdateArchiveBar0(bool b)
        {
            _archiveBar_0.enabled = b;
            _archiveDelete_0.SetActive(b);
            _archiveRename_0.SetActive(b);
        }
        public void UpdateArchiveBar1(bool b)
        {
            _archiveBar_1.enabled = b;
            _archiveDelete_1.SetActive(b);
            _archiveRename_1.SetActive(b);
        }
        public void UpdateArchiveBar2(bool b)
        {
            _archiveBar_2.enabled = b;
            _archiveDelete_2.SetActive(b);
            _archiveRename_2.SetActive(b);
        }
        //更新存档栏信息
        public void UpdateArchiveText(string name, string time,int index)
        {
            _archiveTMP[index].text = name;
            _archiveTimeTMP[index].text = time;
        }
        //分辨率取值
        public int GetRes()
        {
            return _resDropdown.value;
        }
        //全屏取值
        public bool GetFullWin()
        {
            return _fullWinToggle.isOn;
        }
    }
}
