using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIReceiver : MonoBehaviour
{
    //����
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
            this.menuUIInteraction = menuUIInteraction ?? throw new Exception("menuUIInteraction��������Ϊnull");
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
    // ���˵�����
    public void ContinueGame()// ������Ϸ
    {
        menuUIInteraction.ContinueGame();
    }
    public void SelectSaveSlot()//ѡ��浵
    {
        menuUIInteraction.SelectSaveSlot();
    }
    public void StartNewGame()// ����Ϸ
    {
        menuUIInteraction.StartNewGame();
    }
    public void OpenSettings()// ����
    {
        menuUIInteraction.OpenSettings();
    }
    public void QuitGame()// �˳���Ϸ
    {
        menuUIInteraction.QuitGame();
    }
    public void CloseAllPanel()//�ر��������
    {
        menuUIInteraction.CloseAllPanel();
    }

    // �浵ѡ����
    public void SelectSaveSlot0()// ѡ��浵0
    {
        menuUIInteraction.SelectArchiveData(0);
    }
    public void SelectSaveSlot1()// ѡ��浵1
    {
        menuUIInteraction.SelectArchiveData(1);
    }
    public void SelectSaveSlot2()// ѡ��浵2
    {
        menuUIInteraction.SelectArchiveData(2);
    }

    // �浵������
    public void RenameSaveSlot0()// �������浵0
    {
        menuUIInteraction.RenameSaveSlot(0);
    }
    public void RenameSaveSlot1()// �������浵1
    {
        menuUIInteraction.RenameSaveSlot(1);
    }
    public void RenameSaveSlot2()// �������浵2
    {
        menuUIInteraction.RenameSaveSlot(2);
    }
    public void EndInput0(string name)//��������0
    {
        menuUIInteraction.EndInput(name,0);
    }
    public void EndInput1(string name)//��������1
    {
        menuUIInteraction.EndInput(name, 1);
    }
    public void EndInput2(string name)//��������2
    {
        menuUIInteraction.EndInput(name, 2);
    }
    public void DeleteSaveSlot0()// ɾ���浵0
    {
        menuUIInteraction.DeleteSaveSlot(0);
    }
    public void DeleteSaveSlot1()// ɾ���浵1
    {
        menuUIInteraction.DeleteSaveSlot(1);
    }
    public void DeleteSaveSlot2()// ɾ���浵2
    {
        menuUIInteraction.DeleteSaveSlot(2);
    }
    public void ApplySetting()//Ӧ������
    {
        menuUIInteraction.ApplySetting();
    }
}
